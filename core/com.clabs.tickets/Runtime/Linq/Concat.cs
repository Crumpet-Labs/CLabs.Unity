using System;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static ITicketAsyncEnumerable<TSource> Concat<TSource>(this ITicketAsyncEnumerable<TSource> first, ITicketAsyncEnumerable<TSource> second)
        {
            Error.ThrowArgumentNullException(first, nameof(first));
            Error.ThrowArgumentNullException(second, nameof(second));

            return new Concat<TSource>(first, second);
        }
    }

    internal sealed class Concat<TSource> : ITicketAsyncEnumerable<TSource>
    {
        readonly ITicketAsyncEnumerable<TSource> first;
        readonly ITicketAsyncEnumerable<TSource> second;

        public Concat(ITicketAsyncEnumerable<TSource> first, ITicketAsyncEnumerable<TSource> second)
        {
            this.first = first;
            this.second = second;
        }

        public ITicketAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _Concat(first, second, cancellationToken);
        }

        sealed class _Concat : MoveNextSource, ITicketAsyncEnumerator<TSource>
        {
            static readonly Action<object> MoveNextCoreDelegate = MoveNextCore;

            enum IteratingState
            {
                IteratingFirst,
                IteratingSecond,
                Complete
            }

            readonly ITicketAsyncEnumerable<TSource> first;
            readonly ITicketAsyncEnumerable<TSource> second;
            CancellationToken cancellationToken;

            IteratingState iteratingState;

            ITicketAsyncEnumerator<TSource> enumerator;
            Ticket<bool>.Awaiter awaiter;

            public _Concat(ITicketAsyncEnumerable<TSource> first, ITicketAsyncEnumerable<TSource> second, CancellationToken cancellationToken)
            {
                this.first = first;
                this.second = second;
                this.cancellationToken = cancellationToken;
                this.iteratingState = IteratingState.IteratingFirst;
                TaskTracker.TrackActiveTask(this, 3);
            }

            public TSource Current { get; private set; }

            public Ticket<bool> MoveNextAsync()
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (iteratingState == IteratingState.Complete) return CompletedTasks.False;

                completionSource.Reset();
                StartIterate();
                return new Ticket<bool>(this, completionSource.Version);
            }

            void StartIterate()
            {
                if (enumerator == null)
                {
                    if (iteratingState == IteratingState.IteratingFirst)
                    {
                        enumerator = first.GetAsyncEnumerator(cancellationToken);
                    }
                    else if (iteratingState == IteratingState.IteratingSecond)
                    {
                        enumerator = second.GetAsyncEnumerator(cancellationToken);
                    }
                }

                try
                {
                    awaiter = enumerator.MoveNextAsync().GetAwaiter();
                }
                catch (Exception ex)
                {
                    completionSource.TrySetException(ex);
                    return;
                }

                if (awaiter.IsCompleted)
                {
                    MoveNextCoreDelegate(this);
                }
                else
                {
                    awaiter.SourceOnCompleted(MoveNextCoreDelegate, this);
                }
            }

            static void MoveNextCore(object state)
            {
                var self = (_Concat)state;

                if (self.TryGetResult(self.awaiter, out var result))
                {
                    if (result)
                    {
                        self.Current = self.enumerator.Current;
                        self.completionSource.TrySetResult(true);
                    }
                    else
                    {
                        if (self.iteratingState == IteratingState.IteratingFirst)
                        {
                            self.RunSecondAfterDisposeAsync().Forget();
                            return;
                        }

                        self.iteratingState = IteratingState.Complete;
                        self.completionSource.TrySetResult(false);
                    }
                }
            }

            async TicketVoid RunSecondAfterDisposeAsync()
            {
                try
                {
                    await enumerator.DisposeAsync();
                    enumerator = null;
                    awaiter = default;
                    iteratingState = IteratingState.IteratingSecond;
                }
                catch (Exception ex)
                {
                    completionSource.TrySetException(ex);
                }

                StartIterate();
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