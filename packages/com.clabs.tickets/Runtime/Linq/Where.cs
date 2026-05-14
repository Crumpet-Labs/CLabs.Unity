using System;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static ITicketAsyncEnumerable<TSource> Where<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Boolean> predicate)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return new Where<TSource>(source, predicate);
        }

        public static ITicketAsyncEnumerable<TSource> Where<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Int32, Boolean> predicate)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return new WhereInt<TSource>(source, predicate);
        }

        public static ITicketAsyncEnumerable<TSource> WhereAwait<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Boolean>> predicate)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return new WhereAwait<TSource>(source, predicate);
        }

        public static ITicketAsyncEnumerable<TSource> WhereAwait<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Int32, Ticket<Boolean>> predicate)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return new WhereIntAwait<TSource>(source, predicate);
        }

        public static ITicketAsyncEnumerable<TSource> WhereAwaitWithCancellation<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Boolean>> predicate)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return new WhereAwaitWithCancellation<TSource>(source, predicate);
        }

        public static ITicketAsyncEnumerable<TSource> WhereAwaitWithCancellation<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Int32, CancellationToken, Ticket<Boolean>> predicate)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return new WhereIntAwaitWithCancellation<TSource>(source, predicate);
        }
    }

    internal sealed class Where<TSource> : ITicketAsyncEnumerable<TSource>
    {
        readonly ITicketAsyncEnumerable<TSource> source;
        readonly Func<TSource, bool> predicate;

        public Where(ITicketAsyncEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            this.source = source;
            this.predicate = predicate;
        }

        public ITicketAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _Where(source, predicate, cancellationToken);
        }

        sealed class _Where : MoveNextSource, ITicketAsyncEnumerator<TSource>
        {
            readonly ITicketAsyncEnumerable<TSource> source;
            readonly Func<TSource, bool> predicate;
            readonly CancellationToken cancellationToken;

            int state = -1;
            ITicketAsyncEnumerator<TSource> enumerator;
            Ticket<bool>.Awaiter awaiter;
            Action moveNextAction;

            public _Where(ITicketAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken)
            {
                this.source = source;
                this.predicate = predicate;
                this.cancellationToken = cancellationToken;
                this.moveNextAction = MoveNext;
                TaskTracker.TrackActiveTask(this, 3);
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
                            goto case 0;
                        case 0:
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
                                Current = enumerator.Current;
                                if (predicate(Current))
                                {
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
                TaskTracker.RemoveTracking(this);
                return enumerator.DisposeAsync();
            }
        }
    }

    internal sealed class WhereInt<TSource> : ITicketAsyncEnumerable<TSource>
    {
        readonly ITicketAsyncEnumerable<TSource> source;
        readonly Func<TSource, int, bool> predicate;

        public WhereInt(ITicketAsyncEnumerable<TSource> source, Func<TSource, int, bool> predicate)
        {
            this.source = source;
            this.predicate = predicate;
        }

        public ITicketAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _Where(source, predicate, cancellationToken);
        }

        sealed class _Where : MoveNextSource, ITicketAsyncEnumerator<TSource>
        {
            readonly ITicketAsyncEnumerable<TSource> source;
            readonly Func<TSource, int, bool> predicate;
            readonly CancellationToken cancellationToken;

            int state = -1;
            ITicketAsyncEnumerator<TSource> enumerator;
            Ticket<bool>.Awaiter awaiter;
            Action moveNextAction;
            int index;

            public _Where(ITicketAsyncEnumerable<TSource> source, Func<TSource, int, bool> predicate, CancellationToken cancellationToken)
            {
                this.source = source;
                this.predicate = predicate;
                this.cancellationToken = cancellationToken;
                this.moveNextAction = MoveNext;
                TaskTracker.TrackActiveTask(this, 3);
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
                            goto case 0;
                        case 0:
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
                                Current = enumerator.Current;
                                if (predicate(Current, checked(index++)))
                                {
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
                TaskTracker.RemoveTracking(this);
                return enumerator.DisposeAsync();
            }
        }
    }

    internal sealed class WhereAwait<TSource> : ITicketAsyncEnumerable<TSource>
    {
        readonly ITicketAsyncEnumerable<TSource> source;
        readonly Func<TSource, Ticket<bool>> predicate;

        public WhereAwait(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<bool>> predicate)
        {
            this.source = source;
            this.predicate = predicate;
        }

        public ITicketAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _WhereAwait(source, predicate, cancellationToken);
        }

        sealed class _WhereAwait : MoveNextSource, ITicketAsyncEnumerator<TSource>
        {
            readonly ITicketAsyncEnumerable<TSource> source;
            readonly Func<TSource, Ticket<bool>> predicate;
            readonly CancellationToken cancellationToken;

            int state = -1;
            ITicketAsyncEnumerator<TSource> enumerator;
            Ticket<bool>.Awaiter awaiter;
            Ticket<bool>.Awaiter awaiter2;
            Action moveNextAction;

            public _WhereAwait(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<bool>> predicate, CancellationToken cancellationToken)
            {
                this.source = source;
                this.predicate = predicate;
                this.cancellationToken = cancellationToken;
                this.moveNextAction = MoveNext;
                TaskTracker.TrackActiveTask(this, 3);
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
                            goto case 0;
                        case 0:
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
                                Current = enumerator.Current;

                                awaiter2 = predicate(Current).GetAwaiter();
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
                            if (awaiter2.GetResult())
                            {
                                goto CONTINUE;
                            }
                            else
                            {
                                state = 0;
                                goto REPEAT;
                            }
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
                TaskTracker.RemoveTracking(this);
                return enumerator.DisposeAsync();
            }
        }
    }

    internal sealed class WhereIntAwait<TSource> : ITicketAsyncEnumerable<TSource>
    {
        readonly ITicketAsyncEnumerable<TSource> source;
        readonly Func<TSource, int, Ticket<bool>> predicate;

        public WhereIntAwait(ITicketAsyncEnumerable<TSource> source, Func<TSource, int, Ticket<bool>> predicate)
        {
            this.source = source;
            this.predicate = predicate;
        }

        public ITicketAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _WhereAwait(source, predicate, cancellationToken);
        }

        sealed class _WhereAwait : MoveNextSource, ITicketAsyncEnumerator<TSource>
        {
            readonly ITicketAsyncEnumerable<TSource> source;
            readonly Func<TSource, int, Ticket<bool>> predicate;
            readonly CancellationToken cancellationToken;

            int state = -1;
            ITicketAsyncEnumerator<TSource> enumerator;
            Ticket<bool>.Awaiter awaiter;
            Ticket<bool>.Awaiter awaiter2;
            Action moveNextAction;
            int index;

            public _WhereAwait(ITicketAsyncEnumerable<TSource> source, Func<TSource, int, Ticket<bool>> predicate, CancellationToken cancellationToken)
            {
                this.source = source;
                this.predicate = predicate;
                this.cancellationToken = cancellationToken;
                this.moveNextAction = MoveNext;
                TaskTracker.TrackActiveTask(this, 3);
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
                            goto case 0;
                        case 0:
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
                                Current = enumerator.Current;

                                awaiter2 = predicate(Current, checked(index++)).GetAwaiter();
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
                            if (awaiter2.GetResult())
                            {
                                goto CONTINUE;
                            }
                            else
                            {
                                state = 0;
                                goto REPEAT;
                            }
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
                TaskTracker.RemoveTracking(this);
                return enumerator.DisposeAsync();
            }
        }
    }

    internal sealed class WhereAwaitWithCancellation<TSource> : ITicketAsyncEnumerable<TSource>
    {
        readonly ITicketAsyncEnumerable<TSource> source;
        readonly Func<TSource, CancellationToken, Ticket<bool>> predicate;

        public WhereAwaitWithCancellation(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<bool>> predicate)
        {
            this.source = source;
            this.predicate = predicate;
        }

        public ITicketAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _WhereAwaitWithCancellation(source, predicate, cancellationToken);
        }

        sealed class _WhereAwaitWithCancellation : MoveNextSource, ITicketAsyncEnumerator<TSource>
        {
            readonly ITicketAsyncEnumerable<TSource> source;
            readonly Func<TSource, CancellationToken, Ticket<bool>> predicate;
            readonly CancellationToken cancellationToken;

            int state = -1;
            ITicketAsyncEnumerator<TSource> enumerator;
            Ticket<bool>.Awaiter awaiter;
            Ticket<bool>.Awaiter awaiter2;
            Action moveNextAction;

            public _WhereAwaitWithCancellation(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<bool>> predicate, CancellationToken cancellationToken)
            {
                this.source = source;
                this.predicate = predicate;
                this.cancellationToken = cancellationToken;
                this.moveNextAction = MoveNext;
                TaskTracker.TrackActiveTask(this, 3);
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
                            goto case 0;
                        case 0:
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
                                Current = enumerator.Current;

                                awaiter2 = predicate(Current, cancellationToken).GetAwaiter();
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
                            if (awaiter2.GetResult())
                            {
                                goto CONTINUE;
                            }
                            else
                            {
                                state = 0;
                                goto REPEAT;
                            }
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
                TaskTracker.RemoveTracking(this);
                return enumerator.DisposeAsync();
            }
        }
    }

    internal sealed class WhereIntAwaitWithCancellation<TSource> : ITicketAsyncEnumerable<TSource>
    {
        readonly ITicketAsyncEnumerable<TSource> source;
        readonly Func<TSource, int, CancellationToken, Ticket<bool>> predicate;

        public WhereIntAwaitWithCancellation(ITicketAsyncEnumerable<TSource> source, Func<TSource, int, CancellationToken, Ticket<bool>> predicate)
        {
            this.source = source;
            this.predicate = predicate;
        }

        public ITicketAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _WhereAwaitWithCancellation(source, predicate, cancellationToken);
        }

        sealed class _WhereAwaitWithCancellation : MoveNextSource, ITicketAsyncEnumerator<TSource>
        {
            readonly ITicketAsyncEnumerable<TSource> source;
            readonly Func<TSource, int, CancellationToken, Ticket<bool>> predicate;
            readonly CancellationToken cancellationToken;

            int state = -1;
            ITicketAsyncEnumerator<TSource> enumerator;
            Ticket<bool>.Awaiter awaiter;
            Ticket<bool>.Awaiter awaiter2;
            Action moveNextAction;
            int index;

            public _WhereAwaitWithCancellation(ITicketAsyncEnumerable<TSource> source, Func<TSource, int, CancellationToken, Ticket<bool>> predicate, CancellationToken cancellationToken)
            {
                this.source = source;
                this.predicate = predicate;
                this.cancellationToken = cancellationToken;
                this.moveNextAction = MoveNext;
                TaskTracker.TrackActiveTask(this, 3);
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
                            goto case 0;
                        case 0:
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
                                Current = enumerator.Current;

                                awaiter2 = predicate(Current, checked(index++), cancellationToken).GetAwaiter();
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
                            if (awaiter2.GetResult())
                            {
                                goto CONTINUE;
                            }
                            else
                            {
                                state = 0;
                                goto REPEAT;
                            }
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
                TaskTracker.RemoveTracking(this);
                return enumerator.DisposeAsync();
            }
        }
    }
}