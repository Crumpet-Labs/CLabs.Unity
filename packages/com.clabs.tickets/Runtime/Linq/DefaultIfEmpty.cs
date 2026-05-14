using System;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static ITicketAsyncEnumerable<TSource> DefaultIfEmpty<TSource>(this ITicketAsyncEnumerable<TSource> source)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return new DefaultIfEmpty<TSource>(source, default);
        }

        public static ITicketAsyncEnumerable<TSource> DefaultIfEmpty<TSource>(this ITicketAsyncEnumerable<TSource> source, TSource defaultValue)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return new DefaultIfEmpty<TSource>(source, defaultValue);
        }
    }

    internal sealed class DefaultIfEmpty<TSource> : ITicketAsyncEnumerable<TSource>
    {
        readonly ITicketAsyncEnumerable<TSource> source;
        readonly TSource defaultValue;

        public DefaultIfEmpty(ITicketAsyncEnumerable<TSource> source, TSource defaultValue)
        {
            this.source = source;
            this.defaultValue = defaultValue;
        }

        public ITicketAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _DefaultIfEmpty(source, defaultValue, cancellationToken);
        }

        sealed class _DefaultIfEmpty : MoveNextSource, ITicketAsyncEnumerator<TSource>
        {
            enum IteratingState : byte
            {
                Empty,
                Iterating,
                Completed
            }

            static readonly Action<object> MoveNextCoreDelegate = MoveNextCore;

            readonly ITicketAsyncEnumerable<TSource> source;
            readonly TSource defaultValue;
            CancellationToken cancellationToken;

            IteratingState iteratingState;
            ITicketAsyncEnumerator<TSource> enumerator;
            Ticket<bool>.Awaiter awaiter;

            public _DefaultIfEmpty(ITicketAsyncEnumerable<TSource> source, TSource defaultValue, CancellationToken cancellationToken)
            {
                this.source = source;
                this.defaultValue = defaultValue;
                this.cancellationToken = cancellationToken;

                this.iteratingState = IteratingState.Empty;
                TaskTracker.TrackActiveTask(this, 3);
            }

            public TSource Current { get; private set; }


            public Ticket<bool> MoveNextAsync()
            {
                cancellationToken.ThrowIfCancellationRequested();
                completionSource.Reset();

                if (iteratingState == IteratingState.Completed)
                {
                    return CompletedTasks.False;
                }

                if (enumerator == null)
                {
                    enumerator = source.GetAsyncEnumerator(cancellationToken);
                }

                awaiter = enumerator.MoveNextAsync().GetAwaiter();

                if (awaiter.IsCompleted)
                {
                    MoveNextCore(this);
                }
                else
                {
                    awaiter.SourceOnCompleted(MoveNextCoreDelegate, this);
                }

                return new Ticket<bool>(this, completionSource.Version);
            }

            static void MoveNextCore(object state)
            {
                var self = (_DefaultIfEmpty)state;

                if (self.TryGetResult(self.awaiter, out var result))
                {
                    if (result)
                    {
                        self.iteratingState = IteratingState.Iterating;
                        self.Current = self.enumerator.Current;
                        self.completionSource.TrySetResult(true);
                    }
                    else
                    {
                        if (self.iteratingState == IteratingState.Empty)
                        {
                            self.iteratingState = IteratingState.Completed;

                            self.Current = self.defaultValue;
                            self.completionSource.TrySetResult(true);
                        }
                        else
                        {
                            self.completionSource.TrySetResult(false);
                        }
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
