using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static ITicketAsyncEnumerable<T> Merge<T>(this ITicketAsyncEnumerable<T> first, ITicketAsyncEnumerable<T> second)
        {
            Error.ThrowArgumentNullException(first, nameof(first));
            Error.ThrowArgumentNullException(second, nameof(second));

            return new Merge<T>(new [] { first, second });
        }

        public static ITicketAsyncEnumerable<T> Merge<T>(this ITicketAsyncEnumerable<T> first, ITicketAsyncEnumerable<T> second, ITicketAsyncEnumerable<T> third)
        {
            Error.ThrowArgumentNullException(first, nameof(first));
            Error.ThrowArgumentNullException(second, nameof(second));
            Error.ThrowArgumentNullException(third, nameof(third));

            return new Merge<T>(new[] { first, second, third });
        }

        public static ITicketAsyncEnumerable<T> Merge<T>(this IEnumerable<ITicketAsyncEnumerable<T>> sources)
        {
            return sources is ITicketAsyncEnumerable<T>[] array
                ? new Merge<T>(array)
                : new Merge<T>(sources.ToArray());
        }

        public static ITicketAsyncEnumerable<T> Merge<T>(params ITicketAsyncEnumerable<T>[] sources)
        {
            return new Merge<T>(sources);
        }
    }

    internal sealed class Merge<T> : ITicketAsyncEnumerable<T>
    {
        readonly ITicketAsyncEnumerable<T>[] sources;

        public Merge(ITicketAsyncEnumerable<T>[] sources)
        {
            if (sources.Length <= 0)
            {
                Error.ThrowArgumentException("No source async enumerable to merge");
            }
            this.sources = sources;
        }

        public ITicketAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => new _Merge(sources, cancellationToken);

        enum MergeSourceState
        {
            Pending,
            Running,
            Completed,
        }

        sealed class _Merge : MoveNextSource, ITicketAsyncEnumerator<T>
        {
            static readonly Action<object> GetResultAtAction = GetResultAt;

            readonly int length;
            readonly ITicketAsyncEnumerator<T>[] enumerators;
            readonly MergeSourceState[] states;
            readonly Queue<(T, Exception, bool)> queuedResult = new Queue<(T, Exception, bool)>();
            readonly CancellationToken cancellationToken;

            int moveNextCompleted;

            public T Current { get; private set; }

            public _Merge(ITicketAsyncEnumerable<T>[] sources, CancellationToken cancellationToken)
            {
                this.cancellationToken = cancellationToken;
                length = sources.Length;
                states = ArrayPool<MergeSourceState>.Shared.Rent(length);
                enumerators = ArrayPool<ITicketAsyncEnumerator<T>>.Shared.Rent(length);
                for (var i = 0; i < length; i++)
                {
                    enumerators[i] = sources[i].GetAsyncEnumerator(cancellationToken);
                    states[i] = (int)MergeSourceState.Pending;;
                }
            }

            public Ticket<bool> MoveNextAsync()
            {
                cancellationToken.ThrowIfCancellationRequested();
                completionSource.Reset();
                Interlocked.Exchange(ref moveNextCompleted, 0);

                if (HasQueuedResult() && Interlocked.CompareExchange(ref moveNextCompleted, 1, 0) == 0)
                {
                    (T, Exception, bool) value;
                    lock (states)
                    {
                        value = queuedResult.Dequeue();
                    }
                    var resultValue = value.Item1;
                    var exception = value.Item2;
                    var hasNext = value.Item3;
                    if (exception != null)
                    {
                        completionSource.TrySetException(exception);
                    }
                    else
                    {
                        Current = resultValue;
                        completionSource.TrySetResult(hasNext);
                    }
                    return new Ticket<bool>(this, completionSource.Version);
                }

                for (var i = 0; i < length; i++)
                {
                    lock (states)
                    {
                        if (states[i] == MergeSourceState.Pending)
                        {
                            states[i] = MergeSourceState.Running;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    var awaiter = enumerators[i].MoveNextAsync().GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        GetResultAt(i, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(GetResultAtAction, StateTuple.Create(this, i, awaiter));
                    }
                }
                return new Ticket<bool>(this, completionSource.Version);
            }

            public async Ticket DisposeAsync()
            {
                for (var i = 0; i < length; i++)
                {
                    await enumerators[i].DisposeAsync();
                }

                ArrayPool<MergeSourceState>.Shared.Return(states, true);
                ArrayPool<ITicketAsyncEnumerator<T>>.Shared.Return(enumerators, true);
            }

            static void GetResultAt(object state)
            {
                using (var tuple = (StateTuple<_Merge, int, Ticket<bool>.Awaiter>)state)
                {
                    tuple.Item1.GetResultAt(tuple.Item2, tuple.Item3);
                }
            }

            void GetResultAt(int index, Ticket<bool>.Awaiter awaiter)
            {
                bool hasNext;
                bool completedAll;
                try
                {
                    hasNext = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    if (Interlocked.CompareExchange(ref moveNextCompleted, 1, 0) == 0)
                    {
                        completionSource.TrySetException(ex);
                    }
                    else
                    {
                        lock (states)
                        {
                            queuedResult.Enqueue((default, ex, default));
                        }
                    }
                    return;
                }

                lock (states)
                {
                    states[index] = hasNext ? MergeSourceState.Pending : MergeSourceState.Completed;
                    completedAll = !hasNext && IsCompletedAll();
                }
                if (hasNext || completedAll)
                {
                    if (Interlocked.CompareExchange(ref moveNextCompleted, 1, 0) == 0)
                    {
                        Current = enumerators[index].Current;
                        completionSource.TrySetResult(!completedAll);
                    }
                    else
                    {
                        lock (states)
                        {
                            queuedResult.Enqueue((enumerators[index].Current, null, !completedAll));
                        }
                    }
                }
            }

            bool HasQueuedResult()
            {
                lock (states)
                {
                    return queuedResult.Count > 0;
                }
            }

            bool IsCompletedAll()
            {
                lock (states)
                {
                    for (var i = 0; i < length; i++)
                    {
                        if (states[i] != MergeSourceState.Completed)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }
    }
}