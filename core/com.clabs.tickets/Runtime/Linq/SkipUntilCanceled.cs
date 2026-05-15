using System;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static ITicketAsyncEnumerable<TSource> SkipUntilCanceled<TSource>(this ITicketAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return new SkipUntilCanceled<TSource>(source, cancellationToken);
        }
    }

    internal sealed class SkipUntilCanceled<TSource> : ITicketAsyncEnumerable<TSource>
    {
        readonly ITicketAsyncEnumerable<TSource> source;
        readonly CancellationToken cancellationToken;

        public SkipUntilCanceled(ITicketAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
        {
            this.source = source;
            this.cancellationToken = cancellationToken;
        }

        public ITicketAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _SkipUntilCanceled(source, this.cancellationToken, cancellationToken);
        }

        sealed class _SkipUntilCanceled : MoveNextSource, ITicketAsyncEnumerator<TSource>
        {
            static readonly Action<object> CancelDelegate1 = OnCanceled1;
            static readonly Action<object> CancelDelegate2 = OnCanceled2;
            static readonly Action<object> MoveNextCoreDelegate = MoveNextCore;

            readonly ITicketAsyncEnumerable<TSource> source;
            CancellationToken cancellationToken1;
            CancellationToken cancellationToken2;
            CancellationTokenRegistration cancellationTokenRegistration1;
            CancellationTokenRegistration cancellationTokenRegistration2;

            int isCanceled;
            ITicketAsyncEnumerator<TSource> enumerator;
            Ticket<bool>.Awaiter awaiter;
            bool continueNext;

            public _SkipUntilCanceled(ITicketAsyncEnumerable<TSource> source, CancellationToken cancellationToken1, CancellationToken cancellationToken2)
            {
                this.source = source;
                this.cancellationToken1 = cancellationToken1;
                this.cancellationToken2 = cancellationToken2;
                if (cancellationToken1.CanBeCanceled)
                {
                    this.cancellationTokenRegistration1 = cancellationToken1.RegisterWithoutCaptureExecutionContext(CancelDelegate1, this);
                }
                if (cancellationToken1 != cancellationToken2 && cancellationToken2.CanBeCanceled)
                {
                    this.cancellationTokenRegistration2 = cancellationToken2.RegisterWithoutCaptureExecutionContext(CancelDelegate2, this);
                }
                TaskTracker.TrackActiveTask(this, 3);
            }

            public TSource Current { get; private set; }

            public Ticket<bool> MoveNextAsync()
            {
                if (enumerator == null)
                {
                    if (cancellationToken1.IsCancellationRequested) isCanceled = 1;
                    if (cancellationToken2.IsCancellationRequested) isCanceled = 1;
                    enumerator = source.GetAsyncEnumerator(cancellationToken2); // use only AsyncEnumerator provided token.
                }
                completionSource.Reset();

                if (isCanceled != 0)
                {
                    SourceMoveNext();
                }
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
                            goto LOOP;
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
                var self = (_SkipUntilCanceled)state;

                if (self.TryGetResult(self.awaiter, out var result))
                {
                    if (result)
                    {
                        self.Current = self.enumerator.Current;
                        self.completionSource.TrySetResult(true);
                        if (self.continueNext)
                        {
                            self.SourceMoveNext();
                        }
                    }
                    else
                    {
                        self.completionSource.TrySetResult(false);
                    }
                }
            }

            static void OnCanceled1(object state)
            {
                var self = (_SkipUntilCanceled)state;
                if (self.isCanceled == 0)
                {
                    if (Interlocked.Increment(ref self.isCanceled) == 1)
                    {
                        self.cancellationTokenRegistration2.Dispose();
                        self.SourceMoveNext();
                    }
                }
            }

            static void OnCanceled2(object state)
            {
                var self = (_SkipUntilCanceled)state;
                if (self.isCanceled == 0)
                {
                    if (Interlocked.Increment(ref self.isCanceled) == 1)
                    {
                        self.cancellationTokenRegistration2.Dispose();
                        self.SourceMoveNext();
                    }
                }
            }

            public Ticket DisposeAsync()
            {
                TaskTracker.RemoveTracking(this);
                cancellationTokenRegistration1.Dispose();
                cancellationTokenRegistration2.Dispose();
                if (enumerator != null)
                {
                    return enumerator.DisposeAsync();
                }
                return default;
            }
        }
    }
}