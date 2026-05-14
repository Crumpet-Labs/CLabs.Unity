using System;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static ITicketAsyncEnumerable<(TSource, TSource)> Pairwise<TSource>(this ITicketAsyncEnumerable<TSource> source)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return new Pairwise<TSource>(source);
        }
    }

    internal sealed class Pairwise<TSource> : ITicketAsyncEnumerable<(TSource, TSource)>
    {
        readonly ITicketAsyncEnumerable<TSource> source;

        public Pairwise(ITicketAsyncEnumerable<TSource> source)
        {
            this.source = source;
        }

        public ITicketAsyncEnumerator<(TSource, TSource)> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _Pairwise(source, cancellationToken);
        }

        sealed class _Pairwise : MoveNextSource, ITicketAsyncEnumerator<(TSource, TSource)>
        {
            static readonly Action<object> MoveNextCoreDelegate = MoveNextCore;

            readonly ITicketAsyncEnumerable<TSource> source;
            CancellationToken cancellationToken;

            ITicketAsyncEnumerator<TSource> enumerator;
            Ticket<bool>.Awaiter awaiter;

            TSource prev;
            bool isFirst;

            public _Pairwise(ITicketAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
            {
                this.source = source;
                this.cancellationToken = cancellationToken;
                TaskTracker.TrackActiveTask(this, 3);
            }

            public (TSource, TSource) Current { get; private set; }

            public Ticket<bool> MoveNextAsync()
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (enumerator == null)
                {
                    isFirst = true;
                    enumerator = source.GetAsyncEnumerator(cancellationToken);
                }

                completionSource.Reset();
                SourceMoveNext();
                return new Ticket<bool>(this, completionSource.Version);
            }

            void SourceMoveNext()
            {
                try
                {
                    awaiter = enumerator.MoveNextAsync().GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        MoveNextCore(this);
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
                var self = (_Pairwise)state;

                if (self.TryGetResult(self.awaiter, out var result))
                {
                    if (result)
                    {
                        if (self.isFirst)
                        {
                            self.isFirst = false;
                            self.prev = self.enumerator.Current;
                            self.SourceMoveNext(); // run again. okay to use recursive(only one more).
                        }
                        else
                        {
                            var p = self.prev;
                            self.prev = self.enumerator.Current;
                            self.Current = (p, self.prev);
                            self.completionSource.TrySetResult(true);
                        }
                    }
                    else
                    {
                        self.completionSource.TrySetResult(false);
                    }
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