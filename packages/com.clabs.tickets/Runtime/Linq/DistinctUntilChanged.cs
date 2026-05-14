using System;
using System.Collections.Generic;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static ITicketAsyncEnumerable<TSource> DistinctUntilChanged<TSource>(this ITicketAsyncEnumerable<TSource> source)
        {
            return DistinctUntilChanged(source, EqualityComparer<TSource>.Default);
        }

        public static ITicketAsyncEnumerable<TSource> DistinctUntilChanged<TSource>(this ITicketAsyncEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(comparer, nameof(comparer));

            return new DistinctUntilChanged<TSource>(source, comparer);
        }

        public static ITicketAsyncEnumerable<TSource> DistinctUntilChanged<TSource, TKey>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return DistinctUntilChanged(source, keySelector, EqualityComparer<TKey>.Default);
        }

        public static ITicketAsyncEnumerable<TSource> DistinctUntilChanged<TSource, TKey>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(keySelector, nameof(keySelector));
            Error.ThrowArgumentNullException(comparer, nameof(comparer));

            return new DistinctUntilChanged<TSource, TKey>(source, keySelector, comparer);
        }

        public static ITicketAsyncEnumerable<TSource> DistinctUntilChangedAwait<TSource, TKey>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<TKey>> keySelector)
        {
            return DistinctUntilChangedAwait(source, keySelector, EqualityComparer<TKey>.Default);
        }

        public static ITicketAsyncEnumerable<TSource> DistinctUntilChangedAwait<TSource, TKey>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<TKey>> keySelector, IEqualityComparer<TKey> comparer)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(keySelector, nameof(keySelector));
            Error.ThrowArgumentNullException(comparer, nameof(comparer));

            return new DistinctUntilChangedAwait<TSource, TKey>(source, keySelector, comparer);
        }

        public static ITicketAsyncEnumerable<TSource> DistinctUntilChangedAwaitWithCancellation<TSource, TKey>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<TKey>> keySelector)
        {
            return DistinctUntilChangedAwaitWithCancellation(source, keySelector, EqualityComparer<TKey>.Default);
        }

        public static ITicketAsyncEnumerable<TSource> DistinctUntilChangedAwaitWithCancellation<TSource, TKey>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<TKey>> keySelector, IEqualityComparer<TKey> comparer)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(keySelector, nameof(keySelector));
            Error.ThrowArgumentNullException(comparer, nameof(comparer));

            return new DistinctUntilChangedAwaitWithCancellation<TSource, TKey>(source, keySelector, comparer);
        }
    }

    internal sealed class DistinctUntilChanged<TSource> : ITicketAsyncEnumerable<TSource>
    {
        readonly ITicketAsyncEnumerable<TSource> source;
        readonly IEqualityComparer<TSource> comparer;

        public DistinctUntilChanged(ITicketAsyncEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
        {
            this.source = source;
            this.comparer = comparer;
        }

        public ITicketAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _DistinctUntilChanged(source, comparer, cancellationToken);
        }

        sealed class _DistinctUntilChanged : MoveNextSource, ITicketAsyncEnumerator<TSource>
        {
            readonly ITicketAsyncEnumerable<TSource> source;
            readonly IEqualityComparer<TSource> comparer;
            readonly CancellationToken cancellationToken;

            int state = -1;
            ITicketAsyncEnumerator<TSource> enumerator;
            Ticket<bool>.Awaiter awaiter;
            Action moveNextAction;

            public _DistinctUntilChanged(ITicketAsyncEnumerable<TSource> source, IEqualityComparer<TSource> comparer, CancellationToken cancellationToken)
            {
                this.source = source;
                this.comparer = comparer;
                this.cancellationToken = cancellationToken;
                this.moveNextAction = MoveNext;
            }

            public TSource Current { get; private set; }

            public Ticket<bool> MoveNextAsync()
            {
                if (state == -2) return default;

                completionSource.Reset();
                MoveNext();
                return new Ticket<bool>(this, completionSource.Version);
            }

            void MoveNext()
            {
                REPEAT:
                try
                {
                    switch (state)
                    {
                        case -1: // init
                            enumerator = source.GetAsyncEnumerator(cancellationToken);
                            awaiter = enumerator.MoveNextAsync().GetAwaiter();
                            if (awaiter.IsCompleted)
                            {
                                goto case -3;
                            }
                            else
                            {
                                state = -3;
                                awaiter.UnsafeOnCompleted(moveNextAction);
                                return;
                            }
                        case -3: // first
                            if (awaiter.GetResult())
                            {
                                Current = enumerator.Current;
                                goto CONTINUE;
                            }
                            else
                            {
                                goto DONE;
                            }
                        case 0: // normal
                            awaiter = enumerator.MoveNextAsync().GetAwaiter();
                            if (awaiter.IsCompleted)
                            {
                                goto case 1;
                            }
                            else
                            {
                                state = 1;
                                awaiter.UnsafeOnCompleted(moveNextAction);
                                return;
                            }
                        case 1:
                            if (awaiter.GetResult())
                            {
                                var v = enumerator.Current;
                                if (!comparer.Equals(Current, v))
                                {
                                    Current = v;
                                    goto CONTINUE;
                                }
                                else
                                {
                                    state = 0;
                                    goto REPEAT;
                                }
                            }
                            else
                            {
                                goto DONE;
                            }
                        case -2:
                        default:
                            goto DONE;
                    }
                }
                catch (Exception ex)
                {
                    state = -2;
                    completionSource.TrySetException(ex);
                    return;
                }

                DONE:
                state = -2;
                completionSource.TrySetResult(false);
                return;

                CONTINUE:
                state = 0;
                completionSource.TrySetResult(true);
                return;
            }

            public Ticket DisposeAsync()
            {
                return enumerator.DisposeAsync();
            }
        }
    }

    internal sealed class DistinctUntilChanged<TSource, TKey> : ITicketAsyncEnumerable<TSource>
    {
        readonly ITicketAsyncEnumerable<TSource> source;
        readonly Func<TSource, TKey> keySelector;
        readonly IEqualityComparer<TKey> comparer;

        public DistinctUntilChanged(ITicketAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            this.source = source;
            this.keySelector = keySelector;
            this.comparer = comparer;
        }

        public ITicketAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _DistinctUntilChanged(source, keySelector, comparer, cancellationToken);
        }

        sealed class _DistinctUntilChanged : MoveNextSource, ITicketAsyncEnumerator<TSource>
        {
            readonly ITicketAsyncEnumerable<TSource> source;
            readonly Func<TSource, TKey> keySelector;
            readonly IEqualityComparer<TKey> comparer;
            readonly CancellationToken cancellationToken;

            int state = -1;
            ITicketAsyncEnumerator<TSource> enumerator;
            Ticket<bool>.Awaiter awaiter;
            Action moveNextAction;
            TKey prev;

            public _DistinctUntilChanged(ITicketAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
            {
                this.source = source;
                this.keySelector = keySelector;
                this.comparer = comparer;
                this.cancellationToken = cancellationToken;
                this.moveNextAction = MoveNext;
            }

            public TSource Current { get; private set; }

            public Ticket<bool> MoveNextAsync()
            {
                if (state == -2) return default;

                completionSource.Reset();
                MoveNext();
                return new Ticket<bool>(this, completionSource.Version);
            }

            void MoveNext()
            {
                REPEAT:
                try
                {
                    switch (state)
                    {
                        case -1: // init
                            enumerator = source.GetAsyncEnumerator(cancellationToken);
                            awaiter = enumerator.MoveNextAsync().GetAwaiter();
                            if (awaiter.IsCompleted)
                            {
                                goto case -3;
                            }
                            else
                            {
                                state = -3;
                                awaiter.UnsafeOnCompleted(moveNextAction);
                                return;
                            }
                        case -3: // first
                            if (awaiter.GetResult())
                            {
                                Current = enumerator.Current;
                                goto CONTINUE;
                            }
                            else
                            {
                                goto DONE;
                            }
                        case 0: // normal
                            awaiter = enumerator.MoveNextAsync().GetAwaiter();
                            if (awaiter.IsCompleted)
                            {
                                goto case 1;
                            }
                            else
                            {
                                state = 1;
                                awaiter.UnsafeOnCompleted(moveNextAction);
                                return;
                            }
                        case 1:
                            if (awaiter.GetResult())
                            {
                                var v = enumerator.Current;
                                var key = keySelector(v);
                                if (!comparer.Equals(prev, key))
                                {
                                    prev = key;
                                    Current = v;
                                    goto CONTINUE;
                                }
                                else
                                {
                                    state = 0;
                                    goto REPEAT;
                                }
                            }
                            else
                            {
                                goto DONE;
                            }
                        case -2:
                        default:
                            goto DONE;
                    }
                }
                catch (Exception ex)
                {
                    state = -2;
                    completionSource.TrySetException(ex);
                    return;
                }

                DONE:
                state = -2;
                completionSource.TrySetResult(false);
                return;

                CONTINUE:
                state = 0;
                completionSource.TrySetResult(true);
                return;
            }

            public Ticket DisposeAsync()
            {
                return enumerator.DisposeAsync();
            }
        }
    }

    internal sealed class DistinctUntilChangedAwait<TSource, TKey> : ITicketAsyncEnumerable<TSource>
    {
        readonly ITicketAsyncEnumerable<TSource> source;
        readonly Func<TSource, Ticket<TKey>> keySelector;
        readonly IEqualityComparer<TKey> comparer;

        public DistinctUntilChangedAwait(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<TKey>> keySelector, IEqualityComparer<TKey> comparer)
        {
            this.source = source;
            this.keySelector = keySelector;
            this.comparer = comparer;
        }

        public ITicketAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _DistinctUntilChangedAwait(source, keySelector, comparer, cancellationToken);
        }

        sealed class _DistinctUntilChangedAwait : MoveNextSource, ITicketAsyncEnumerator<TSource>
        {
            readonly ITicketAsyncEnumerable<TSource> source;
            readonly Func<TSource, Ticket<TKey>> keySelector;
            readonly IEqualityComparer<TKey> comparer;
            readonly CancellationToken cancellationToken;

            int state = -1;
            ITicketAsyncEnumerator<TSource> enumerator;
            Ticket<bool>.Awaiter awaiter;
            Ticket<TKey>.Awaiter awaiter2;
            Action moveNextAction;
            TSource enumeratorCurrent;
            TKey prev;

            public _DistinctUntilChangedAwait(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<TKey>> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
            {
                this.source = source;
                this.keySelector = keySelector;
                this.comparer = comparer;
                this.cancellationToken = cancellationToken;
                this.moveNextAction = MoveNext;
            }

            public TSource Current { get; private set; }

            public Ticket<bool> MoveNextAsync()
            {
                if (state == -2) return default;

                completionSource.Reset();
                MoveNext();
                return new Ticket<bool>(this, completionSource.Version);
            }

            void MoveNext()
            {
                REPEAT:
                try
                {
                    switch (state)
                    {
                        case -1: // init
                            enumerator = source.GetAsyncEnumerator(cancellationToken);
                            awaiter = enumerator.MoveNextAsync().GetAwaiter();
                            if (awaiter.IsCompleted)
                            {
                                goto case -3;
                            }
                            else
                            {
                                state = -3;
                                awaiter.UnsafeOnCompleted(moveNextAction);
                                return;
                            }
                        case -3: // first
                            if (awaiter.GetResult())
                            {
                                Current = enumerator.Current;
                                goto CONTINUE;
                            }
                            else
                            {
                                goto DONE;
                            }
                        case 0: // normal
                            awaiter = enumerator.MoveNextAsync().GetAwaiter();
                            if (awaiter.IsCompleted)
                            {
                                goto case 1;
                            }
                            else
                            {
                                state = 1;
                                awaiter.UnsafeOnCompleted(moveNextAction);
                                return;
                            }
                        case 1:
                            if (awaiter.GetResult())
                            {
                                enumeratorCurrent = enumerator.Current;
                                awaiter2 = keySelector(enumeratorCurrent).GetAwaiter();
                                if (awaiter2.IsCompleted)
                                {
                                    goto case 2;
                                }
                                else
                                {
                                    state = 2;
                                    awaiter2.UnsafeOnCompleted(moveNextAction);
                                    return;
                                }
                            }
                            else
                            {
                                goto DONE;
                            }
                        case 2:
                            var key = awaiter2.GetResult();
                            if (!comparer.Equals(prev, key))
                            {
                                prev = key;
                                Current = enumeratorCurrent;
                                goto CONTINUE;
                            }
                            else
                            {
                                state = 0;
                                goto REPEAT;
                            }
                        case -2:
                        default:
                            goto DONE;
                    }
                }
                catch (Exception ex)
                {
                    state = -2;
                    completionSource.TrySetException(ex);
                    return;
                }

                DONE:
                state = -2;
                completionSource.TrySetResult(false);
                return;

                CONTINUE:
                state = 0;
                completionSource.TrySetResult(true);
                return;
            }

            public Ticket DisposeAsync()
            {
                return enumerator.DisposeAsync();
            }
        }
    }

    internal sealed class DistinctUntilChangedAwaitWithCancellation<TSource, TKey> : ITicketAsyncEnumerable<TSource>
    {
        readonly ITicketAsyncEnumerable<TSource> source;
        readonly Func<TSource, CancellationToken, Ticket<TKey>> keySelector;
        readonly IEqualityComparer<TKey> comparer;

        public DistinctUntilChangedAwaitWithCancellation(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<TKey>> keySelector, IEqualityComparer<TKey> comparer)
        {
            this.source = source;
            this.keySelector = keySelector;
            this.comparer = comparer;
        }

        public ITicketAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _DistinctUntilChangedAwaitWithCancellation(source, keySelector, comparer, cancellationToken);
        }

        sealed class _DistinctUntilChangedAwaitWithCancellation : MoveNextSource, ITicketAsyncEnumerator<TSource>
        {
            readonly ITicketAsyncEnumerable<TSource> source;
            readonly Func<TSource, CancellationToken, Ticket<TKey>> keySelector;
            readonly IEqualityComparer<TKey> comparer;
            readonly CancellationToken cancellationToken;

            int state = -1;
            ITicketAsyncEnumerator<TSource> enumerator;
            Ticket<bool>.Awaiter awaiter;
            Ticket<TKey>.Awaiter awaiter2;
            Action moveNextAction;
            TSource enumeratorCurrent;
            TKey prev;

            public _DistinctUntilChangedAwaitWithCancellation(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<TKey>> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
            {
                this.source = source;
                this.keySelector = keySelector;
                this.comparer = comparer;
                this.cancellationToken = cancellationToken;
                this.moveNextAction = MoveNext;
            }

            public TSource Current { get; private set; }

            public Ticket<bool> MoveNextAsync()
            {
                if (state == -2) return default;

                completionSource.Reset();
                MoveNext();
                return new Ticket<bool>(this, completionSource.Version);
            }

            void MoveNext()
            {
                REPEAT:
                try
                {
                    switch (state)
                    {
                        case -1: // init
                            enumerator = source.GetAsyncEnumerator(cancellationToken);
                            awaiter = enumerator.MoveNextAsync().GetAwaiter();
                            if (awaiter.IsCompleted)
                            {
                                goto case -3;
                            }
                            else
                            {
                                state = -3;
                                awaiter.UnsafeOnCompleted(moveNextAction);
                                return;
                            }
                        case -3: // first
                            if (awaiter.GetResult())
                            {
                                Current = enumerator.Current;
                                goto CONTINUE;
                            }
                            else
                            {
                                goto DONE;
                            }
                        case 0: // normal
                            awaiter = enumerator.MoveNextAsync().GetAwaiter();
                            if (awaiter.IsCompleted)
                            {
                                goto case 1;
                            }
                            else
                            {
                                state = 1;
                                awaiter.UnsafeOnCompleted(moveNextAction);
                                return;
                            }
                        case 1:
                            if (awaiter.GetResult())
                            {
                                enumeratorCurrent = enumerator.Current;
                                awaiter2 = keySelector(enumeratorCurrent, cancellationToken).GetAwaiter();
                                if (awaiter2.IsCompleted)
                                {
                                    goto case 2;
                                }
                                else
                                {
                                    state = 2;
                                    awaiter2.UnsafeOnCompleted(moveNextAction);
                                    return;
                                }
                            }
                            else
                            {
                                goto DONE;
                            }
                        case 2:
                            var key = awaiter2.GetResult();
                            if (!comparer.Equals(prev, key))
                            {
                                prev = key;
                                Current = enumeratorCurrent;
                                goto CONTINUE;
                            }
                            else
                            {
                                state = 0;
                                goto REPEAT;
                            }
                        case -2:
                        default:
                            goto DONE;
                    }
                }
                catch (Exception ex)
                {
                    state = -2;
                    completionSource.TrySetException(ex);
                    return;
                }

                DONE:
                state = -2;
                completionSource.TrySetResult(false);
                return;

                CONTINUE:
                state = 0;
                completionSource.TrySetResult(true);
                return;
            }

            public Ticket DisposeAsync()
            {
                return enumerator.DisposeAsync();
            }
        }
    }
}