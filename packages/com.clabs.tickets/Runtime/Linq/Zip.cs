using System;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {

        public static ITicketAsyncEnumerable<(TFirst First, TSecond Second)> Zip<TFirst, TSecond>(this ITicketAsyncEnumerable<TFirst> first, ITicketAsyncEnumerable<TSecond> second)
        {
            Error.ThrowArgumentNullException(first, nameof(first));
            Error.ThrowArgumentNullException(second, nameof(second));

            return Zip(first, second, (x, y) => (x, y));
        }

        public static ITicketAsyncEnumerable<TResult> Zip<TFirst, TSecond, TResult>(this ITicketAsyncEnumerable<TFirst> first, ITicketAsyncEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        {
            Error.ThrowArgumentNullException(first, nameof(first));
            Error.ThrowArgumentNullException(second, nameof(second));
            Error.ThrowArgumentNullException(resultSelector, nameof(resultSelector));

            return new Zip<TFirst, TSecond, TResult>(first, second, resultSelector);
        }

        public static ITicketAsyncEnumerable<TResult> ZipAwait<TFirst, TSecond, TResult>(this ITicketAsyncEnumerable<TFirst> first, ITicketAsyncEnumerable<TSecond> second, Func<TFirst, TSecond, Ticket<TResult>> selector)
        {
            Error.ThrowArgumentNullException(first, nameof(first));
            Error.ThrowArgumentNullException(second, nameof(second));
            Error.ThrowArgumentNullException(selector, nameof(selector));

            return new ZipAwait<TFirst, TSecond, TResult>(first, second, selector);
        }

        public static ITicketAsyncEnumerable<TResult> ZipAwaitWithCancellation<TFirst, TSecond, TResult>(this ITicketAsyncEnumerable<TFirst> first, ITicketAsyncEnumerable<TSecond> second, Func<TFirst, TSecond, CancellationToken, Ticket<TResult>> selector)
        {
            Error.ThrowArgumentNullException(first, nameof(first));
            Error.ThrowArgumentNullException(second, nameof(second));
            Error.ThrowArgumentNullException(selector, nameof(selector));

            return new ZipAwaitWithCancellation<TFirst, TSecond, TResult>(first, second, selector);
        }
    }

    internal sealed class Zip<TFirst, TSecond, TResult> : ITicketAsyncEnumerable<TResult>
    {
        readonly ITicketAsyncEnumerable<TFirst> first;
        readonly ITicketAsyncEnumerable<TSecond> second;
        readonly Func<TFirst, TSecond, TResult> resultSelector;

        public Zip(ITicketAsyncEnumerable<TFirst> first, ITicketAsyncEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        {
            this.first = first;
            this.second = second;
            this.resultSelector = resultSelector;
        }

        public ITicketAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _Zip(first, second, resultSelector, cancellationToken);
        }

        sealed class _Zip : MoveNextSource, ITicketAsyncEnumerator<TResult>
        {
            static readonly Action<object> firstMoveNextCoreDelegate = FirstMoveNextCore;
            static readonly Action<object> secondMoveNextCoreDelegate = SecondMoveNextCore;

            readonly ITicketAsyncEnumerable<TFirst> first;
            readonly ITicketAsyncEnumerable<TSecond> second;
            readonly Func<TFirst, TSecond, TResult> resultSelector;

            CancellationToken cancellationToken;

            ITicketAsyncEnumerator<TFirst> firstEnumerator;
            ITicketAsyncEnumerator<TSecond> secondEnumerator;

            Ticket<bool>.Awaiter firstAwaiter;
            Ticket<bool>.Awaiter secondAwaiter;

            public _Zip(ITicketAsyncEnumerable<TFirst> first, ITicketAsyncEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector, CancellationToken cancellationToken)
            {
                this.first = first;
                this.second = second;
                this.resultSelector = resultSelector;
                this.cancellationToken = cancellationToken;
                TaskTracker.TrackActiveTask(this, 3);
            }

            public TResult Current { get; private set; }

            public Ticket<bool> MoveNextAsync()
            {
                completionSource.Reset();

                if (firstEnumerator == null)
                {
                    firstEnumerator = first.GetAsyncEnumerator(cancellationToken);
                    secondEnumerator = second.GetAsyncEnumerator(cancellationToken);
                }

                firstAwaiter = firstEnumerator.MoveNextAsync().GetAwaiter();

                if (firstAwaiter.IsCompleted)
                {
                    FirstMoveNextCore(this);
                }
                else
                {
                    firstAwaiter.SourceOnCompleted(firstMoveNextCoreDelegate, this);
                }

                return new Ticket<bool>(this, completionSource.Version);
            }

            static void FirstMoveNextCore(object state)
            {
                var self = (_Zip)state;

                if (self.TryGetResult(self.firstAwaiter, out var result))
                {
                    if (result)
                    {
                        try
                        {
                            self.secondAwaiter = self.secondEnumerator.MoveNextAsync().GetAwaiter();
                        }
                        catch (Exception ex)
                        {
                            self.completionSource.TrySetException(ex);
                            return;
                        }

                        if (self.secondAwaiter.IsCompleted)
                        {
                            SecondMoveNextCore(self);
                        }
                        else
                        {
                            self.secondAwaiter.SourceOnCompleted(secondMoveNextCoreDelegate, self);
                        }
                    }
                    else
                    {
                        self.completionSource.TrySetResult(false);
                    }
                }
            }

            static void SecondMoveNextCore(object state)
            {
                var self = (_Zip)state;

                if (self.TryGetResult(self.secondAwaiter, out var result))
                {
                    if (result)
                    {
                        try
                        {
                            self.Current = self.resultSelector(self.firstEnumerator.Current, self.secondEnumerator.Current);
                        }
                        catch (Exception ex)
                        {
                            self.completionSource.TrySetException(ex);
                        }

                        if (self.cancellationToken.IsCancellationRequested)
                        {
                            self.completionSource.TrySetCanceled(self.cancellationToken);
                        }
                        else
                        {
                            self.completionSource.TrySetResult(true);
                        }
                    }
                    else
                    {
                        self.completionSource.TrySetResult(false);
                    }
                }
            }

            public async Ticket DisposeAsync()
            {
                TaskTracker.RemoveTracking(this);
                if (firstEnumerator != null)
                {
                    await firstEnumerator.DisposeAsync();
                }
                if (secondEnumerator != null)
                {
                    await secondEnumerator.DisposeAsync();
                }
            }
        }
    }

    internal sealed class ZipAwait<TFirst, TSecond, TResult> : ITicketAsyncEnumerable<TResult>
    {
        readonly ITicketAsyncEnumerable<TFirst> first;
        readonly ITicketAsyncEnumerable<TSecond> second;
        readonly Func<TFirst, TSecond, Ticket<TResult>> resultSelector;

        public ZipAwait(ITicketAsyncEnumerable<TFirst> first, ITicketAsyncEnumerable<TSecond> second, Func<TFirst, TSecond, Ticket<TResult>> resultSelector)
        {
            this.first = first;
            this.second = second;
            this.resultSelector = resultSelector;
        }

        public ITicketAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _ZipAwait(first, second, resultSelector, cancellationToken);
        }

        sealed class _ZipAwait : MoveNextSource, ITicketAsyncEnumerator<TResult>
        {
            static readonly Action<object> firstMoveNextCoreDelegate = FirstMoveNextCore;
            static readonly Action<object> secondMoveNextCoreDelegate = SecondMoveNextCore;
            static readonly Action<object> resultAwaitCoreDelegate = ResultAwaitCore;

            readonly ITicketAsyncEnumerable<TFirst> first;
            readonly ITicketAsyncEnumerable<TSecond> second;
            readonly Func<TFirst, TSecond, Ticket<TResult>> resultSelector;

            CancellationToken cancellationToken;

            ITicketAsyncEnumerator<TFirst> firstEnumerator;
            ITicketAsyncEnumerator<TSecond> secondEnumerator;

            Ticket<bool>.Awaiter firstAwaiter;
            Ticket<bool>.Awaiter secondAwaiter;
            Ticket<TResult>.Awaiter resultAwaiter;

            public _ZipAwait(ITicketAsyncEnumerable<TFirst> first, ITicketAsyncEnumerable<TSecond> second, Func<TFirst, TSecond, Ticket<TResult>> resultSelector, CancellationToken cancellationToken)
            {
                this.first = first;
                this.second = second;
                this.resultSelector = resultSelector;
                this.cancellationToken = cancellationToken;
                TaskTracker.TrackActiveTask(this, 3);
            }

            public TResult Current { get; private set; }

            public Ticket<bool> MoveNextAsync()
            {
                completionSource.Reset();

                if (firstEnumerator == null)
                {
                    firstEnumerator = first.GetAsyncEnumerator(cancellationToken);
                    secondEnumerator = second.GetAsyncEnumerator(cancellationToken);
                }

                firstAwaiter = firstEnumerator.MoveNextAsync().GetAwaiter();

                if (firstAwaiter.IsCompleted)
                {
                    FirstMoveNextCore(this);
                }
                else
                {
                    firstAwaiter.SourceOnCompleted(firstMoveNextCoreDelegate, this);
                }

                return new Ticket<bool>(this, completionSource.Version);
            }

            static void FirstMoveNextCore(object state)
            {
                var self = (_ZipAwait)state;

                if (self.TryGetResult(self.firstAwaiter, out var result))
                {
                    if (result)
                    {
                        try
                        {
                            self.secondAwaiter = self.secondEnumerator.MoveNextAsync().GetAwaiter();
                        }
                        catch (Exception ex)
                        {
                            self.completionSource.TrySetException(ex);
                            return;
                        }

                        if (self.secondAwaiter.IsCompleted)
                        {
                            SecondMoveNextCore(self);
                        }
                        else
                        {
                            self.secondAwaiter.SourceOnCompleted(secondMoveNextCoreDelegate, self);
                        }
                    }
                    else
                    {
                        self.completionSource.TrySetResult(false);
                    }
                }
            }

            static void SecondMoveNextCore(object state)
            {
                var self = (_ZipAwait)state;

                if (self.TryGetResult(self.secondAwaiter, out var result))
                {
                    if (result)
                    {
                        try
                        {
                            self.resultAwaiter = self.resultSelector(self.firstEnumerator.Current, self.secondEnumerator.Current).GetAwaiter();
                            if (self.resultAwaiter.IsCompleted)
                            {
                                ResultAwaitCore(self);
                            }
                            else
                            {
                                self.resultAwaiter.SourceOnCompleted(resultAwaitCoreDelegate, self);
                            }
                        }
                        catch (Exception ex)
                        {
                            self.completionSource.TrySetException(ex);
                        }
                    }
                    else
                    {
                        self.completionSource.TrySetResult(false);
                    }
                }
            }

            static void ResultAwaitCore(object state)
            {
                var self = (_ZipAwait)state;

                if (self.TryGetResult(self.resultAwaiter, out var result))
                {
                    self.Current = result;

                    if (self.cancellationToken.IsCancellationRequested)
                    {
                        self.completionSource.TrySetCanceled(self.cancellationToken);
                    }
                    else
                    {
                        self.completionSource.TrySetResult(true);
                    }
                }
            }

            public async Ticket DisposeAsync()
            {
                TaskTracker.RemoveTracking(this);
                if (firstEnumerator != null)
                {
                    await firstEnumerator.DisposeAsync();
                }
                if (secondEnumerator != null)
                {
                    await secondEnumerator.DisposeAsync();
                }
            }
        }
    }

    internal sealed class ZipAwaitWithCancellation<TFirst, TSecond, TResult> : ITicketAsyncEnumerable<TResult>
    {
        readonly ITicketAsyncEnumerable<TFirst> first;
        readonly ITicketAsyncEnumerable<TSecond> second;
        readonly Func<TFirst, TSecond, CancellationToken, Ticket<TResult>> resultSelector;

        public ZipAwaitWithCancellation(ITicketAsyncEnumerable<TFirst> first, ITicketAsyncEnumerable<TSecond> second, Func<TFirst, TSecond, CancellationToken, Ticket<TResult>> resultSelector)
        {
            this.first = first;
            this.second = second;
            this.resultSelector = resultSelector;
        }

        public ITicketAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _ZipAwaitWithCancellation(first, second, resultSelector, cancellationToken);
        }

        sealed class _ZipAwaitWithCancellation : MoveNextSource, ITicketAsyncEnumerator<TResult>
        {
            static readonly Action<object> firstMoveNextCoreDelegate = FirstMoveNextCore;
            static readonly Action<object> secondMoveNextCoreDelegate = SecondMoveNextCore;
            static readonly Action<object> resultAwaitCoreDelegate = ResultAwaitCore;

            readonly ITicketAsyncEnumerable<TFirst> first;
            readonly ITicketAsyncEnumerable<TSecond> second;
            readonly Func<TFirst, TSecond, CancellationToken, Ticket<TResult>> resultSelector;

            CancellationToken cancellationToken;

            ITicketAsyncEnumerator<TFirst> firstEnumerator;
            ITicketAsyncEnumerator<TSecond> secondEnumerator;

            Ticket<bool>.Awaiter firstAwaiter;
            Ticket<bool>.Awaiter secondAwaiter;
            Ticket<TResult>.Awaiter resultAwaiter;

            public _ZipAwaitWithCancellation(ITicketAsyncEnumerable<TFirst> first, ITicketAsyncEnumerable<TSecond> second, Func<TFirst, TSecond, CancellationToken, Ticket<TResult>> resultSelector, CancellationToken cancellationToken)
            {
                this.first = first;
                this.second = second;
                this.resultSelector = resultSelector;
                this.cancellationToken = cancellationToken;
                TaskTracker.TrackActiveTask(this, 3);
            }

            public TResult Current { get; private set; }

            public Ticket<bool> MoveNextAsync()
            {
                completionSource.Reset();

                if (firstEnumerator == null)
                {
                    firstEnumerator = first.GetAsyncEnumerator(cancellationToken);
                    secondEnumerator = second.GetAsyncEnumerator(cancellationToken);
                }

                firstAwaiter = firstEnumerator.MoveNextAsync().GetAwaiter();

                if (firstAwaiter.IsCompleted)
                {
                    FirstMoveNextCore(this);
                }
                else
                {
                    firstAwaiter.SourceOnCompleted(firstMoveNextCoreDelegate, this);
                }

                return new Ticket<bool>(this, completionSource.Version);
            }

            static void FirstMoveNextCore(object state)
            {
                var self = (_ZipAwaitWithCancellation)state;

                if (self.TryGetResult(self.firstAwaiter, out var result))
                {
                    if (result)
                    {
                        try
                        {
                            self.secondAwaiter = self.secondEnumerator.MoveNextAsync().GetAwaiter();
                        }
                        catch (Exception ex)
                        {
                            self.completionSource.TrySetException(ex);
                            return;
                        }

                        if (self.secondAwaiter.IsCompleted)
                        {
                            SecondMoveNextCore(self);
                        }
                        else
                        {
                            self.secondAwaiter.SourceOnCompleted(secondMoveNextCoreDelegate, self);
                        }
                    }
                    else
                    {
                        self.completionSource.TrySetResult(false);
                    }
                }
            }

            static void SecondMoveNextCore(object state)
            {
                var self = (_ZipAwaitWithCancellation)state;

                if (self.TryGetResult(self.secondAwaiter, out var result))
                {
                    if (result)
                    {
                        try
                        {
                            self.resultAwaiter = self.resultSelector(self.firstEnumerator.Current, self.secondEnumerator.Current, self.cancellationToken).GetAwaiter();
                            if (self.resultAwaiter.IsCompleted)
                            {
                                ResultAwaitCore(self);
                            }
                            else
                            {
                                self.resultAwaiter.SourceOnCompleted(resultAwaitCoreDelegate, self);
                            }
                        }
                        catch (Exception ex)
                        {
                            self.completionSource.TrySetException(ex);
                        }
                    }
                    else
                    {
                        self.completionSource.TrySetResult(false);
                    }
                }
            }

            static void ResultAwaitCore(object state)
            {
                var self = (_ZipAwaitWithCancellation)state;

                if (self.TryGetResult(self.resultAwaiter, out var result))
                {
                    self.Current = result;

                    if (self.cancellationToken.IsCancellationRequested)
                    {
                        self.completionSource.TrySetCanceled(self.cancellationToken);
                    }
                    else
                    {
                        self.completionSource.TrySetResult(true);
                    }
                }
            }

            public async Ticket DisposeAsync()
            {
                TaskTracker.RemoveTracking(this);
                if (firstEnumerator != null)
                {
                    await firstEnumerator.DisposeAsync();
                }
                if (secondEnumerator != null)
                {
                    await secondEnumerator.DisposeAsync();
                }
            }
        }
    }
}