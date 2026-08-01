using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static ITicketAsyncEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this ITicketAsyncEnumerable<TOuter> outer, ITicketAsyncEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
        {
            Error.ThrowArgumentNullException(outer, nameof(outer));
            Error.ThrowArgumentNullException(inner, nameof(inner));
            Error.ThrowArgumentNullException(outerKeySelector, nameof(outerKeySelector));
            Error.ThrowArgumentNullException(innerKeySelector, nameof(innerKeySelector));
            Error.ThrowArgumentNullException(resultSelector, nameof(resultSelector));

            return new GroupJoin<TOuter, TInner, TKey, TResult>(outer, inner, outerKeySelector, innerKeySelector, resultSelector, EqualityComparer<TKey>.Default);
        }

        public static ITicketAsyncEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this ITicketAsyncEnumerable<TOuter> outer, ITicketAsyncEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            Error.ThrowArgumentNullException(outer, nameof(outer));
            Error.ThrowArgumentNullException(inner, nameof(inner));
            Error.ThrowArgumentNullException(outerKeySelector, nameof(outerKeySelector));
            Error.ThrowArgumentNullException(innerKeySelector, nameof(innerKeySelector));
            Error.ThrowArgumentNullException(resultSelector, nameof(resultSelector));
            Error.ThrowArgumentNullException(comparer, nameof(comparer));

            return new GroupJoin<TOuter, TInner, TKey, TResult>(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
        }

        public static ITicketAsyncEnumerable<TResult> GroupJoinAwait<TOuter, TInner, TKey, TResult>(this ITicketAsyncEnumerable<TOuter> outer, ITicketAsyncEnumerable<TInner> inner, Func<TOuter, Ticket<TKey>> outerKeySelector, Func<TInner, Ticket<TKey>> innerKeySelector, Func<TOuter, IEnumerable<TInner>, Ticket<TResult>> resultSelector)
        {
            Error.ThrowArgumentNullException(outer, nameof(outer));
            Error.ThrowArgumentNullException(inner, nameof(inner));
            Error.ThrowArgumentNullException(outerKeySelector, nameof(outerKeySelector));
            Error.ThrowArgumentNullException(innerKeySelector, nameof(innerKeySelector));
            Error.ThrowArgumentNullException(resultSelector, nameof(resultSelector));

            return new GroupJoinAwait<TOuter, TInner, TKey, TResult>(outer, inner, outerKeySelector, innerKeySelector, resultSelector, EqualityComparer<TKey>.Default);
        }

        public static ITicketAsyncEnumerable<TResult> GroupJoinAwait<TOuter, TInner, TKey, TResult>(this ITicketAsyncEnumerable<TOuter> outer, ITicketAsyncEnumerable<TInner> inner, Func<TOuter, Ticket<TKey>> outerKeySelector, Func<TInner, Ticket<TKey>> innerKeySelector, Func<TOuter, IEnumerable<TInner>, Ticket<TResult>> resultSelector, IEqualityComparer<TKey> comparer)
        {
            Error.ThrowArgumentNullException(outer, nameof(outer));
            Error.ThrowArgumentNullException(inner, nameof(inner));
            Error.ThrowArgumentNullException(outerKeySelector, nameof(outerKeySelector));
            Error.ThrowArgumentNullException(innerKeySelector, nameof(innerKeySelector));
            Error.ThrowArgumentNullException(resultSelector, nameof(resultSelector));
            Error.ThrowArgumentNullException(comparer, nameof(comparer));

            return new GroupJoinAwait<TOuter, TInner, TKey, TResult>(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
        }

        public static ITicketAsyncEnumerable<TResult> GroupJoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>(this ITicketAsyncEnumerable<TOuter> outer, ITicketAsyncEnumerable<TInner> inner, Func<TOuter, CancellationToken, Ticket<TKey>> outerKeySelector, Func<TInner, CancellationToken, Ticket<TKey>> innerKeySelector, Func<TOuter, IEnumerable<TInner>, CancellationToken, Ticket<TResult>> resultSelector)
        {
            Error.ThrowArgumentNullException(outer, nameof(outer));
            Error.ThrowArgumentNullException(inner, nameof(inner));
            Error.ThrowArgumentNullException(outerKeySelector, nameof(outerKeySelector));
            Error.ThrowArgumentNullException(innerKeySelector, nameof(innerKeySelector));
            Error.ThrowArgumentNullException(resultSelector, nameof(resultSelector));

            return new GroupJoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>(outer, inner, outerKeySelector, innerKeySelector, resultSelector, EqualityComparer<TKey>.Default);
        }

        public static ITicketAsyncEnumerable<TResult> GroupJoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>(this ITicketAsyncEnumerable<TOuter> outer, ITicketAsyncEnumerable<TInner> inner, Func<TOuter, CancellationToken, Ticket<TKey>> outerKeySelector, Func<TInner, CancellationToken, Ticket<TKey>> innerKeySelector, Func<TOuter, IEnumerable<TInner>, CancellationToken, Ticket<TResult>> resultSelector, IEqualityComparer<TKey> comparer)
        {
            Error.ThrowArgumentNullException(outer, nameof(outer));
            Error.ThrowArgumentNullException(inner, nameof(inner));
            Error.ThrowArgumentNullException(outerKeySelector, nameof(outerKeySelector));
            Error.ThrowArgumentNullException(innerKeySelector, nameof(innerKeySelector));
            Error.ThrowArgumentNullException(resultSelector, nameof(resultSelector));
            Error.ThrowArgumentNullException(comparer, nameof(comparer));

            return new GroupJoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
        }

    }

    internal sealed class GroupJoin<TOuter, TInner, TKey, TResult> : ITicketAsyncEnumerable<TResult>
    {
        readonly ITicketAsyncEnumerable<TOuter> outer;
        readonly ITicketAsyncEnumerable<TInner> inner;
        readonly Func<TOuter, TKey> outerKeySelector;
        readonly Func<TInner, TKey> innerKeySelector;
        readonly Func<TOuter, IEnumerable<TInner>, TResult> resultSelector;
        readonly IEqualityComparer<TKey> comparer;

        public GroupJoin(ITicketAsyncEnumerable<TOuter> outer, ITicketAsyncEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            this.outer = outer;
            this.inner = inner;
            this.outerKeySelector = outerKeySelector;
            this.innerKeySelector = innerKeySelector;
            this.resultSelector = resultSelector;
            this.comparer = comparer;
        }

        public ITicketAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _GroupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer, cancellationToken);
        }

        sealed class _GroupJoin : MoveNextSource, ITicketAsyncEnumerator<TResult>
        {
            static readonly Action<object> MoveNextCoreDelegate = MoveNextCore;

            readonly ITicketAsyncEnumerable<TOuter> outer;
            readonly ITicketAsyncEnumerable<TInner> inner;
            readonly Func<TOuter, TKey> outerKeySelector;
            readonly Func<TInner, TKey> innerKeySelector;
            readonly Func<TOuter, IEnumerable<TInner>, TResult> resultSelector;
            readonly IEqualityComparer<TKey> comparer;
            CancellationToken cancellationToken;

            ILookup<TKey, TInner> lookup;
            ITicketAsyncEnumerator<TOuter> enumerator;
            Ticket<bool>.Awaiter awaiter;


            public _GroupJoin(ITicketAsyncEnumerable<TOuter> outer, ITicketAsyncEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
            {
                this.outer = outer;
                this.inner = inner;
                this.outerKeySelector = outerKeySelector;
                this.innerKeySelector = innerKeySelector;
                this.resultSelector = resultSelector;
                this.comparer = comparer;
                this.cancellationToken = cancellationToken;
                TaskTracker.TrackActiveTask(this, 3);
            }

            public TResult Current { get; private set; }

            public Ticket<bool> MoveNextAsync()
            {
                cancellationToken.ThrowIfCancellationRequested();
                completionSource.Reset();

                if (lookup == null)
                {
                    CreateLookup().Forget();
                }
                else
                {
                    SourceMoveNext();
                }
                return new Ticket<bool>(this, completionSource.Version);
            }

            async TicketVoid CreateLookup()
            {
                try
                {
                    lookup = await inner.ToLookupAsync(innerKeySelector, comparer, cancellationToken);
                    enumerator = outer.GetAsyncEnumerator(cancellationToken);
                }
                catch (Exception ex)
                {
                    completionSource.TrySetException(ex);
                    return;
                }
                SourceMoveNext();
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
                var self = (_GroupJoin)state;

                if (self.TryGetResult(self.awaiter, out var result))
                {
                    if (result)
                    {
                        var outer = self.enumerator.Current;
                        var key = self.outerKeySelector(outer);
                        var values = self.lookup[key];

                        self.Current = self.resultSelector(outer, values);
                        self.completionSource.TrySetResult(true);
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

    internal sealed class GroupJoinAwait<TOuter, TInner, TKey, TResult> : ITicketAsyncEnumerable<TResult>
    {
        readonly ITicketAsyncEnumerable<TOuter> outer;
        readonly ITicketAsyncEnumerable<TInner> inner;
        readonly Func<TOuter, Ticket<TKey>> outerKeySelector;
        readonly Func<TInner, Ticket<TKey>> innerKeySelector;
        readonly Func<TOuter, IEnumerable<TInner>, Ticket<TResult>> resultSelector;
        readonly IEqualityComparer<TKey> comparer;

        public GroupJoinAwait(ITicketAsyncEnumerable<TOuter> outer, ITicketAsyncEnumerable<TInner> inner, Func<TOuter, Ticket<TKey>> outerKeySelector, Func<TInner, Ticket<TKey>> innerKeySelector, Func<TOuter, IEnumerable<TInner>, Ticket<TResult>> resultSelector, IEqualityComparer<TKey> comparer)
        {
            this.outer = outer;
            this.inner = inner;
            this.outerKeySelector = outerKeySelector;
            this.innerKeySelector = innerKeySelector;
            this.resultSelector = resultSelector;
            this.comparer = comparer;
        }

        public ITicketAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _GroupJoinAwait(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer, cancellationToken);
        }

        sealed class _GroupJoinAwait : MoveNextSource, ITicketAsyncEnumerator<TResult>
        {
            static readonly Action<object> MoveNextCoreDelegate = MoveNextCore;
            readonly static Action<object> ResultSelectCoreDelegate = ResultSelectCore;
            readonly static Action<object> OuterKeySelectCoreDelegate = OuterKeySelectCore;

            readonly ITicketAsyncEnumerable<TOuter> outer;
            readonly ITicketAsyncEnumerable<TInner> inner;
            readonly Func<TOuter, Ticket<TKey>> outerKeySelector;
            readonly Func<TInner, Ticket<TKey>> innerKeySelector;
            readonly Func<TOuter, IEnumerable<TInner>, Ticket<TResult>> resultSelector;
            readonly IEqualityComparer<TKey> comparer;
            CancellationToken cancellationToken;

            ILookup<TKey, TInner> lookup;
            ITicketAsyncEnumerator<TOuter> enumerator;
            TOuter outerValue;
            Ticket<bool>.Awaiter awaiter;
            Ticket<TKey>.Awaiter outerKeyAwaiter;
            Ticket<TResult>.Awaiter resultAwaiter;


            public _GroupJoinAwait(ITicketAsyncEnumerable<TOuter> outer, ITicketAsyncEnumerable<TInner> inner, Func<TOuter, Ticket<TKey>> outerKeySelector, Func<TInner, Ticket<TKey>> innerKeySelector, Func<TOuter, IEnumerable<TInner>, Ticket<TResult>> resultSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
            {
                this.outer = outer;
                this.inner = inner;
                this.outerKeySelector = outerKeySelector;
                this.innerKeySelector = innerKeySelector;
                this.resultSelector = resultSelector;
                this.comparer = comparer;
                this.cancellationToken = cancellationToken;
                TaskTracker.TrackActiveTask(this, 3);
            }

            public TResult Current { get; private set; }

            public Ticket<bool> MoveNextAsync()
            {
                cancellationToken.ThrowIfCancellationRequested();
                completionSource.Reset();

                if (lookup == null)
                {
                    CreateLookup().Forget();
                }
                else
                {
                    SourceMoveNext();
                }
                return new Ticket<bool>(this, completionSource.Version);
            }

            async TicketVoid CreateLookup()
            {
                try
                {
                    lookup = await inner.ToLookupAwaitAsync(innerKeySelector, comparer, cancellationToken);
                    enumerator = outer.GetAsyncEnumerator(cancellationToken);
                }
                catch (Exception ex)
                {
                    completionSource.TrySetException(ex);
                    return;
                }
                SourceMoveNext();
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
                var self = (_GroupJoinAwait)state;

                if (self.TryGetResult(self.awaiter, out var result))
                {
                    if (result)
                    {
                        try
                        {

                            self.outerValue = self.enumerator.Current;
                            self.outerKeyAwaiter = self.outerKeySelector(self.outerValue).GetAwaiter();
                            if (self.outerKeyAwaiter.IsCompleted)
                            {
                                OuterKeySelectCore(self);
                            }
                            else
                            {
                                self.outerKeyAwaiter.SourceOnCompleted(OuterKeySelectCoreDelegate, self);
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

            static void OuterKeySelectCore(object state)
            {
                var self = (_GroupJoinAwait)state;

                if (self.TryGetResult(self.outerKeyAwaiter, out var result))
                {
                    try
                    {
                        var values = self.lookup[result];
                        self.resultAwaiter = self.resultSelector(self.outerValue, values).GetAwaiter();
                        if (self.resultAwaiter.IsCompleted)
                        {
                            ResultSelectCore(self);
                        }
                        else
                        {
                            self.resultAwaiter.SourceOnCompleted(ResultSelectCoreDelegate, self);
                        }
                    }
                    catch (Exception ex)
                    {
                        self.completionSource.TrySetException(ex);
                    }
                }
            }

            static void ResultSelectCore(object state)
            {
                var self = (_GroupJoinAwait)state;

                if (self.TryGetResult(self.resultAwaiter, out var result))
                {
                    self.Current = result;
                    self.completionSource.TrySetResult(true);
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

    internal sealed class GroupJoinAwaitWithCancellation<TOuter, TInner, TKey, TResult> : ITicketAsyncEnumerable<TResult>
    {
        readonly ITicketAsyncEnumerable<TOuter> outer;
        readonly ITicketAsyncEnumerable<TInner> inner;
        readonly Func<TOuter, CancellationToken, Ticket<TKey>> outerKeySelector;
        readonly Func<TInner, CancellationToken, Ticket<TKey>> innerKeySelector;
        readonly Func<TOuter, IEnumerable<TInner>, CancellationToken, Ticket<TResult>> resultSelector;
        readonly IEqualityComparer<TKey> comparer;

        public GroupJoinAwaitWithCancellation(ITicketAsyncEnumerable<TOuter> outer, ITicketAsyncEnumerable<TInner> inner, Func<TOuter, CancellationToken, Ticket<TKey>> outerKeySelector, Func<TInner, CancellationToken, Ticket<TKey>> innerKeySelector, Func<TOuter, IEnumerable<TInner>, CancellationToken, Ticket<TResult>> resultSelector, IEqualityComparer<TKey> comparer)
        {
            this.outer = outer;
            this.inner = inner;
            this.outerKeySelector = outerKeySelector;
            this.innerKeySelector = innerKeySelector;
            this.resultSelector = resultSelector;
            this.comparer = comparer;
        }

        public ITicketAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _GroupJoinAwaitWithCancellation(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer, cancellationToken);
        }

        sealed class _GroupJoinAwaitWithCancellation : MoveNextSource, ITicketAsyncEnumerator<TResult>
        {
            static readonly Action<object> MoveNextCoreDelegate = MoveNextCore;
            readonly static Action<object> ResultSelectCoreDelegate = ResultSelectCore;
            readonly static Action<object> OuterKeySelectCoreDelegate = OuterKeySelectCore;

            readonly ITicketAsyncEnumerable<TOuter> outer;
            readonly ITicketAsyncEnumerable<TInner> inner;
            readonly Func<TOuter, CancellationToken, Ticket<TKey>> outerKeySelector;
            readonly Func<TInner, CancellationToken, Ticket<TKey>> innerKeySelector;
            readonly Func<TOuter, IEnumerable<TInner>, CancellationToken, Ticket<TResult>> resultSelector;
            readonly IEqualityComparer<TKey> comparer;
            CancellationToken cancellationToken;

            ILookup<TKey, TInner> lookup;
            ITicketAsyncEnumerator<TOuter> enumerator;
            TOuter outerValue;
            Ticket<bool>.Awaiter awaiter;
            Ticket<TKey>.Awaiter outerKeyAwaiter;
            Ticket<TResult>.Awaiter resultAwaiter;


            public _GroupJoinAwaitWithCancellation(ITicketAsyncEnumerable<TOuter> outer, ITicketAsyncEnumerable<TInner> inner, Func<TOuter, CancellationToken, Ticket<TKey>> outerKeySelector, Func<TInner, CancellationToken, Ticket<TKey>> innerKeySelector, Func<TOuter, IEnumerable<TInner>, CancellationToken, Ticket<TResult>> resultSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
            {
                this.outer = outer;
                this.inner = inner;
                this.outerKeySelector = outerKeySelector;
                this.innerKeySelector = innerKeySelector;
                this.resultSelector = resultSelector;
                this.comparer = comparer;
                this.cancellationToken = cancellationToken;
                TaskTracker.TrackActiveTask(this, 3);
            }

            public TResult Current { get; private set; }

            public Ticket<bool> MoveNextAsync()
            {
                cancellationToken.ThrowIfCancellationRequested();
                completionSource.Reset();

                if (lookup == null)
                {
                    CreateLookup().Forget();
                }
                else
                {
                    SourceMoveNext();
                }
                return new Ticket<bool>(this, completionSource.Version);
            }

            async TicketVoid CreateLookup()
            {
                try
                {
                    lookup = await inner.ToLookupAwaitWithCancellationAsync(innerKeySelector, comparer, cancellationToken);
                    enumerator = outer.GetAsyncEnumerator(cancellationToken);
                }
                catch (Exception ex)
                {
                    completionSource.TrySetException(ex);
                    return;
                }
                SourceMoveNext();
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
                var self = (_GroupJoinAwaitWithCancellation)state;

                if (self.TryGetResult(self.awaiter, out var result))
                {
                    if (result)
                    {
                        try
                        {

                            self.outerValue = self.enumerator.Current;
                            self.outerKeyAwaiter = self.outerKeySelector(self.outerValue, self.cancellationToken).GetAwaiter();
                            if (self.outerKeyAwaiter.IsCompleted)
                            {
                                OuterKeySelectCore(self);
                            }
                            else
                            {
                                self.outerKeyAwaiter.SourceOnCompleted(OuterKeySelectCoreDelegate, self);
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

            static void OuterKeySelectCore(object state)
            {
                var self = (_GroupJoinAwaitWithCancellation)state;

                if (self.TryGetResult(self.outerKeyAwaiter, out var result))
                {
                    try
                    {
                        var values = self.lookup[result];
                        self.resultAwaiter = self.resultSelector(self.outerValue, values, self.cancellationToken).GetAwaiter();
                        if (self.resultAwaiter.IsCompleted)
                        {
                            ResultSelectCore(self);
                        }
                        else
                        {
                            self.resultAwaiter.SourceOnCompleted(ResultSelectCoreDelegate, self);
                        }
                    }
                    catch (Exception ex)
                    {
                        self.completionSource.TrySetException(ex);
                    }
                }
            }

            static void ResultSelectCore(object state)
            {
                var self = (_GroupJoinAwaitWithCancellation)state;

                if (self.TryGetResult(self.resultAwaiter, out var result))
                {
                    self.Current = result;
                    self.completionSource.TrySetResult(true);
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