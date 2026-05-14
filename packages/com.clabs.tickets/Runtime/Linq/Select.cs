using System;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static ITicketAsyncEnumerable<TResult> Select<TSource, TResult>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(selector, nameof(selector));

            return new Select<TSource, TResult>(source, selector);
        }

        public static ITicketAsyncEnumerable<TResult> Select<TSource, TResult>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Int32, TResult> selector)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(selector, nameof(selector));

            return new SelectInt<TSource, TResult>(source, selector);
        }

        public static ITicketAsyncEnumerable<TResult> SelectAwait<TSource, TResult>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<TResult>> selector)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(selector, nameof(selector));

            return new SelectAwait<TSource, TResult>(source, selector);
        }

        public static ITicketAsyncEnumerable<TResult> SelectAwait<TSource, TResult>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Int32, Ticket<TResult>> selector)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(selector, nameof(selector));

            return new SelectIntAwait<TSource, TResult>(source, selector);
        }

        public static ITicketAsyncEnumerable<TResult> SelectAwaitWithCancellation<TSource, TResult>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<TResult>> selector)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(selector, nameof(selector));

            return new SelectAwaitWithCancellation<TSource, TResult>(source, selector);
        }

        public static ITicketAsyncEnumerable<TResult> SelectAwaitWithCancellation<TSource, TResult>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Int32, CancellationToken, Ticket<TResult>> selector)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(selector, nameof(selector));

            return new SelectIntAwaitWithCancellation<TSource, TResult>(source, selector);
        }
    }

    internal sealed class Select<TSource, TResult> : ITicketAsyncEnumerable<TResult>
    {
        readonly ITicketAsyncEnumerable<TSource> source;
        readonly Func<TSource, TResult> selector;

        public Select(ITicketAsyncEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            this.source = source;
            this.selector = selector;
        }

        public ITicketAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _Select(source, selector, cancellationToken);
        }

        sealed class _Select : MoveNextSource, ITicketAsyncEnumerator<TResult>
        {
            readonly ITicketAsyncEnumerable<TSource> source;
            readonly Func<TSource, TResult> selector;
            readonly CancellationToken cancellationToken;

            int state = -1;
            ITicketAsyncEnumerator<TSource> enumerator;
            Ticket<bool>.Awaiter awaiter;
            Action moveNextAction;

            public _Select(ITicketAsyncEnumerable<TSource> source, Func<TSource, TResult> selector, CancellationToken cancellationToken)
            {
                this.source = source;
                this.selector = selector;
                this.cancellationToken = cancellationToken;
                this.moveNextAction = MoveNext;
                TaskTracker.TrackActiveTask(this, 3);
            }

            public TResult Current { get; private set; }

            public Ticket<bool> MoveNextAsync()
            {
                if (state == -2) return default;

                completionSource.Reset();
                MoveNext();
                return new Ticket<bool>(this, completionSource.Version);
            }

            void MoveNext()
            {
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
                                Current = selector(enumerator.Current);
                                goto CONTINUE;
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

    internal sealed class SelectInt<TSource, TResult> : ITicketAsyncEnumerable<TResult>
    {
        readonly ITicketAsyncEnumerable<TSource> source;
        readonly Func<TSource, int, TResult> selector;

        public SelectInt(ITicketAsyncEnumerable<TSource> source, Func<TSource, int, TResult> selector)
        {
            this.source = source;
            this.selector = selector;
        }

        public ITicketAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _Select(source, selector, cancellationToken);
        }

        sealed class _Select : MoveNextSource, ITicketAsyncEnumerator<TResult>
        {
            readonly ITicketAsyncEnumerable<TSource> source;
            readonly Func<TSource, int, TResult> selector;
            readonly CancellationToken cancellationToken;

            int state = -1;
            ITicketAsyncEnumerator<TSource> enumerator;
            Ticket<bool>.Awaiter awaiter;
            Action moveNextAction;
            int index;

            public _Select(ITicketAsyncEnumerable<TSource> source, Func<TSource, int, TResult> selector, CancellationToken cancellationToken)
            {
                this.source = source;
                this.selector = selector;
                this.cancellationToken = cancellationToken;
                this.moveNextAction = MoveNext;
                TaskTracker.TrackActiveTask(this, 3);
            }

            public TResult Current { get; private set; }

            public Ticket<bool> MoveNextAsync()
            {
                if (state == -2) return default;

                completionSource.Reset();
                MoveNext();
                return new Ticket<bool>(this, completionSource.Version);
            }

            void MoveNext()
            {
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
                                Current = selector(enumerator.Current, checked(index++));
                                goto CONTINUE;
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

    internal sealed class SelectAwait<TSource, TResult> : ITicketAsyncEnumerable<TResult>
    {
        readonly ITicketAsyncEnumerable<TSource> source;
        readonly Func<TSource, Ticket<TResult>> selector;

        public SelectAwait(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<TResult>> selector)
        {
            this.source = source;
            this.selector = selector;
        }

        public ITicketAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _SelectAwait(source, selector, cancellationToken);
        }

        sealed class _SelectAwait : MoveNextSource, ITicketAsyncEnumerator<TResult>
        {
            readonly ITicketAsyncEnumerable<TSource> source;
            readonly Func<TSource, Ticket<TResult>> selector;
            readonly CancellationToken cancellationToken;

            int state = -1;
            ITicketAsyncEnumerator<TSource> enumerator;
            Ticket<bool>.Awaiter awaiter;
            Ticket<TResult>.Awaiter awaiter2;
            Action moveNextAction;

            public _SelectAwait(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<TResult>> selector, CancellationToken cancellationToken)
            {
                this.source = source;
                this.selector = selector;
                this.cancellationToken = cancellationToken;
                this.moveNextAction = MoveNext;
                TaskTracker.TrackActiveTask(this, 3);
            }

            public TResult Current { get; private set; }

            public Ticket<bool> MoveNextAsync()
            {
                if (state == -2) return default;

                completionSource.Reset();
                MoveNext();
                return new Ticket<bool>(this, completionSource.Version);
            }

            void MoveNext()
            {
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
                                awaiter2 = selector(enumerator.Current).GetAwaiter();
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
                            Current = awaiter2.GetResult();
                            goto CONTINUE;
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

    internal sealed class SelectIntAwait<TSource, TResult> : ITicketAsyncEnumerable<TResult>
    {
        readonly ITicketAsyncEnumerable<TSource> source;
        readonly Func<TSource, int, Ticket<TResult>> selector;

        public SelectIntAwait(ITicketAsyncEnumerable<TSource> source, Func<TSource, int, Ticket<TResult>> selector)
        {
            this.source = source;
            this.selector = selector;
        }

        public ITicketAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _SelectAwait(source, selector, cancellationToken);
        }

        sealed class _SelectAwait : MoveNextSource, ITicketAsyncEnumerator<TResult>
        {
            readonly ITicketAsyncEnumerable<TSource> source;
            readonly Func<TSource, int, Ticket<TResult>> selector;
            readonly CancellationToken cancellationToken;

            int state = -1;
            ITicketAsyncEnumerator<TSource> enumerator;
            Ticket<bool>.Awaiter awaiter;
            Ticket<TResult>.Awaiter awaiter2;
            Action moveNextAction;
            int index;

            public _SelectAwait(ITicketAsyncEnumerable<TSource> source, Func<TSource, int, Ticket<TResult>> selector, CancellationToken cancellationToken)
            {
                this.source = source;
                this.selector = selector;
                this.cancellationToken = cancellationToken;
                this.moveNextAction = MoveNext;
                TaskTracker.TrackActiveTask(this, 3);
            }

            public TResult Current { get; private set; }

            public Ticket<bool> MoveNextAsync()
            {
                if (state == -2) return default;

                completionSource.Reset();
                MoveNext();
                return new Ticket<bool>(this, completionSource.Version);
            }

            void MoveNext()
            {
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
                                awaiter2 = selector(enumerator.Current, checked(index++)).GetAwaiter();
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
                            Current = awaiter2.GetResult();
                            goto CONTINUE;
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

    internal sealed class SelectAwaitWithCancellation<TSource, TResult> : ITicketAsyncEnumerable<TResult>
    {
        readonly ITicketAsyncEnumerable<TSource> source;
        readonly Func<TSource, CancellationToken, Ticket<TResult>> selector;

        public SelectAwaitWithCancellation(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<TResult>> selector)
        {
            this.source = source;
            this.selector = selector;
        }

        public ITicketAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _SelectAwaitWithCancellation(source, selector, cancellationToken);
        }

        sealed class _SelectAwaitWithCancellation : MoveNextSource, ITicketAsyncEnumerator<TResult>
        {
            readonly ITicketAsyncEnumerable<TSource> source;
            readonly Func<TSource, CancellationToken, Ticket<TResult>> selector;
            readonly CancellationToken cancellationToken;

            int state = -1;
            ITicketAsyncEnumerator<TSource> enumerator;
            Ticket<bool>.Awaiter awaiter;
            Ticket<TResult>.Awaiter awaiter2;
            Action moveNextAction;

            public _SelectAwaitWithCancellation(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<TResult>> selector, CancellationToken cancellationToken)
            {
                this.source = source;
                this.selector = selector;
                this.cancellationToken = cancellationToken;
                this.moveNextAction = MoveNext;
                TaskTracker.TrackActiveTask(this, 3);
            }

            public TResult Current { get; private set; }

            public Ticket<bool> MoveNextAsync()
            {
                if (state == -2) return default;

                completionSource.Reset();
                MoveNext();
                return new Ticket<bool>(this, completionSource.Version);
            }

            void MoveNext()
            {
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
                                awaiter2 = selector(enumerator.Current, cancellationToken).GetAwaiter();
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
                            Current = awaiter2.GetResult();
                            goto CONTINUE;
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

    internal sealed class SelectIntAwaitWithCancellation<TSource, TResult> : ITicketAsyncEnumerable<TResult>
    {
        readonly ITicketAsyncEnumerable<TSource> source;
        readonly Func<TSource, int, CancellationToken, Ticket<TResult>> selector;

        public SelectIntAwaitWithCancellation(ITicketAsyncEnumerable<TSource> source, Func<TSource, int, CancellationToken, Ticket<TResult>> selector)
        {
            this.source = source;
            this.selector = selector;
        }

        public ITicketAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _SelectAwaitWithCancellation(source, selector, cancellationToken);
        }

        sealed class _SelectAwaitWithCancellation : MoveNextSource, ITicketAsyncEnumerator<TResult>
        {
            readonly ITicketAsyncEnumerable<TSource> source;
            readonly Func<TSource, int, CancellationToken, Ticket<TResult>> selector;
            readonly CancellationToken cancellationToken;

            int state = -1;
            ITicketAsyncEnumerator<TSource> enumerator;
            Ticket<bool>.Awaiter awaiter;
            Ticket<TResult>.Awaiter awaiter2;
            Action moveNextAction;
            int index;

            public _SelectAwaitWithCancellation(ITicketAsyncEnumerable<TSource> source, Func<TSource, int, CancellationToken, Ticket<TResult>> selector, CancellationToken cancellationToken)
            {
                this.source = source;
                this.selector = selector;
                this.cancellationToken = cancellationToken;
                this.moveNextAction = MoveNext;
                TaskTracker.TrackActiveTask(this, 3);
            }

            public TResult Current { get; private set; }

            public Ticket<bool> MoveNextAsync()
            {
                if (state == -2) return default;

                completionSource.Reset();
                MoveNext();
                return new Ticket<bool>(this, completionSource.Version);
            }

            void MoveNext()
            {
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
                                awaiter2 = selector(enumerator.Current, checked(index++), cancellationToken).GetAwaiter();
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
                            Current = awaiter2.GetResult();
                            goto CONTINUE;
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