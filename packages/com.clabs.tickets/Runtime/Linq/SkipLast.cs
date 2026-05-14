using System;
using System.Collections.Generic;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static ITicketAsyncEnumerable<TSource> SkipLast<TSource>(this ITicketAsyncEnumerable<TSource> source, Int32 count)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            // non skip.
            if (count <= 0)
            {
                return source;
            }

            return new SkipLast<TSource>(source, count);
        }
    }

    internal sealed class SkipLast<TSource> : ITicketAsyncEnumerable<TSource>
    {
        readonly ITicketAsyncEnumerable<TSource> source;
        readonly int count;

        public SkipLast(ITicketAsyncEnumerable<TSource> source, int count)
        {
            this.source = source;
            this.count = count;
        }

        public ITicketAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _SkipLast(source, count, cancellationToken);
        }

        sealed class _SkipLast : MoveNextSource, ITicketAsyncEnumerator<TSource>
        {
            static readonly Action<object> MoveNextCoreDelegate = MoveNextCore;

            readonly ITicketAsyncEnumerable<TSource> source;
            readonly int count;
            CancellationToken cancellationToken;

            ITicketAsyncEnumerator<TSource> enumerator;
            Ticket<bool>.Awaiter awaiter;
            Queue<TSource> queue;

            bool continueNext;

            public _SkipLast(ITicketAsyncEnumerable<TSource> source, int count, CancellationToken cancellationToken)
            {
                this.source = source;
                this.count = count;
                this.cancellationToken = cancellationToken;
                TaskTracker.TrackActiveTask(this, 3);
            }

            public TSource Current { get; private set; }

            public Ticket<bool> MoveNextAsync()
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (enumerator == null)
                {
                    enumerator = source.GetAsyncEnumerator(cancellationToken);
                    queue = new Queue<TSource>();
                }

                completionSource.Reset();
                SourceMoveNext();
                return new Ticket<bool>(this, completionSource.Version);
            }

            void SourceMoveNext()
            {
                try
                {

                    LOOP:
                    awaiter = enumerator.MoveNextAsync().GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        continueNext = true;
                        MoveNextCore(this);
                        if (continueNext)
                        {
                            continueNext = false;
                            goto LOOP; // avoid recursive
                        }
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(MoveNextCoreDelegate, this);
                    }
                }
                catch (Exception ex)
                {
                    completionSource.TrySetException(ex);
                }
            }


            static void MoveNextCore(object state)
            {
                var self = (_SkipLast)state;

                if (self.TryGetResult(self.awaiter, out var result))
                {
                    if (result)
                    {
                        if (self.queue.Count == self.count)
                        {
                            self.continueNext = false;

                            var deq = self.queue.Dequeue();
                            self.Current = deq;
                            self.queue.Enqueue(self.enumerator.Current);

                            self.completionSource.TrySetResult(true);
                        }
                        else
                        {
                            self.queue.Enqueue(self.enumerator.Current);

                            if (!self.continueNext)
                            {
                                self.SourceMoveNext();
                            }
                        }
                    }
                    else
                    {
                        self.continueNext = false;
                        self.completionSource.TrySetResult(false);
                    }
                }
                else
                {
                    self.continueNext = false;
                }
            }

            public Ticket DisposeAsync()
            {
                TaskTracker.RemoveTracking(this);
                if (enumerator != null)
                {
                    return enumerator.DisposeAsync();
                }
                return default;
            }
        }
    }
}