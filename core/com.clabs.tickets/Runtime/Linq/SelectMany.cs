using System;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {

        public static ITicketAsyncEnumerable<TResult> SelectMany<TSource, TResult>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, ITicketAsyncEnumerable<TResult>> selector)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(selector, nameof(selector));

            return new SelectMany<TSource, TResult, TResult>(source, selector, (x, y) => y);
        }

        public static ITicketAsyncEnumerable<TResult> SelectMany<TSource, TResult>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Int32, ITicketAsyncEnumerable<TResult>> selector)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(selector, nameof(selector));

            return new SelectMany<TSource, TResult, TResult>(source, selector, (x, y) => y);
        }

        public static ITicketAsyncEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, ITicketAsyncEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(collectionSelector, nameof(collectionSelector));

            return new SelectMany<TSource, TCollection, TResult>(source, collectionSelector, resultSelector);
        }

        public static ITicketAsyncEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Int32, ITicketAsyncEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(collectionSelector, nameof(collectionSelector));

            return new SelectMany<TSource, TCollection, TResult>(source, collectionSelector, resultSelector);
        }

        public static ITicketAsyncEnumerable<TResult> SelectManyAwait<TSource, TResult>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<ITicketAsyncEnumerable<TResult>>> selector)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(selector, nameof(selector));

            return new SelectManyAwait<TSource, TResult, TResult>(source, selector, (x, y) => Ticket.FromResult(y));
        }

        public static ITicketAsyncEnumerable<TResult> SelectManyAwait<TSource, TResult>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Int32, Ticket<ITicketAsyncEnumerable<TResult>>> selector)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(selector, nameof(selector));

            return new SelectManyAwait<TSource, TResult, TResult>(source, selector, (x, y) => Ticket.FromResult(y));
        }

        public static ITicketAsyncEnumerable<TResult> SelectManyAwait<TSource, TCollection, TResult>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<ITicketAsyncEnumerable<TCollection>>> collectionSelector, Func<TSource, TCollection, Ticket<TResult>> resultSelector)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(collectionSelector, nameof(collectionSelector));

            return new SelectManyAwait<TSource, TCollection, TResult>(source, collectionSelector, resultSelector);
        }

        public static ITicketAsyncEnumerable<TResult> SelectManyAwait<TSource, TCollection, TResult>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Int32, Ticket<ITicketAsyncEnumerable<TCollection>>> collectionSelector, Func<TSource, TCollection, Ticket<TResult>> resultSelector)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(collectionSelector, nameof(collectionSelector));

            return new SelectManyAwait<TSource, TCollection, TResult>(source, collectionSelector, resultSelector);
        }

        public static ITicketAsyncEnumerable<TResult> SelectManyAwaitWithCancellation<TSource, TResult>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<ITicketAsyncEnumerable<TResult>>> selector)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(selector, nameof(selector));

            return new SelectManyAwaitWithCancellation<TSource, TResult, TResult>(source, selector, (x, y, c) => Ticket.FromResult(y));
        }

        public static ITicketAsyncEnumerable<TResult> SelectManyAwaitWithCancellation<TSource, TResult>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Int32, CancellationToken, Ticket<ITicketAsyncEnumerable<TResult>>> selector)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(selector, nameof(selector));

            return new SelectManyAwaitWithCancellation<TSource, TResult, TResult>(source, selector, (x, y, c) => Ticket.FromResult(y));
        }

        public static ITicketAsyncEnumerable<TResult> SelectManyAwaitWithCancellation<TSource, TCollection, TResult>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<ITicketAsyncEnumerable<TCollection>>> collectionSelector, Func<TSource, TCollection, CancellationToken, Ticket<TResult>> resultSelector)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(collectionSelector, nameof(collectionSelector));

            return new SelectManyAwaitWithCancellation<TSource, TCollection, TResult>(source, collectionSelector, resultSelector);
        }

        public static ITicketAsyncEnumerable<TResult> SelectManyAwaitWithCancellation<TSource, TCollection, TResult>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Int32, CancellationToken, Ticket<ITicketAsyncEnumerable<TCollection>>> collectionSelector, Func<TSource, TCollection, CancellationToken, Ticket<TResult>> resultSelector)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(collectionSelector, nameof(collectionSelector));

            return new SelectManyAwaitWithCancellation<TSource, TCollection, TResult>(source, collectionSelector, resultSelector);
        }
    }

    internal sealed class SelectMany<TSource, TCollection, TResult> : ITicketAsyncEnumerable<TResult>
    {
        readonly ITicketAsyncEnumerable<TSource> source;
        readonly Func<TSource, ITicketAsyncEnumerable<TCollection>> selector1;
        readonly Func<TSource, int, ITicketAsyncEnumerable<TCollection>> selector2;
        readonly Func<TSource, TCollection, TResult> resultSelector;

        public SelectMany(ITicketAsyncEnumerable<TSource> source, Func<TSource, ITicketAsyncEnumerable<TCollection>> selector, Func<TSource, TCollection, TResult> resultSelector)
        {
            this.source = source;
            this.selector1 = selector;
            this.selector2 = null;
            this.resultSelector = resultSelector;
        }

        public SelectMany(ITicketAsyncEnumerable<TSource> source, Func<TSource, int, ITicketAsyncEnumerable<TCollection>> selector, Func<TSource, TCollection, TResult> resultSelector)
        {
            this.source = source;
            this.selector1 = null;
            this.selector2 = selector;
            this.resultSelector = resultSelector;
        }

        public ITicketAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _SelectMany(source, selector1, selector2, resultSelector, cancellationToken);
        }

        sealed class _SelectMany : MoveNextSource, ITicketAsyncEnumerator<TResult>
        {
            static readonly Action<object> sourceMoveNextCoreDelegate = SourceMoveNextCore;
            static readonly Action<object> selectedSourceMoveNextCoreDelegate = SeletedSourceMoveNextCore;
            static readonly Action<object> selectedEnumeratorDisposeAsyncCoreDelegate = SelectedEnumeratorDisposeAsyncCore;

            readonly ITicketAsyncEnumerable<TSource> source;

            readonly Func<TSource, ITicketAsyncEnumerable<TCollection>> selector1;
            readonly Func<TSource, int, ITicketAsyncEnumerable<TCollection>> selector2;
            readonly Func<TSource, TCollection, TResult> resultSelector;
            CancellationToken cancellationToken;

            TSource sourceCurrent;
            int sourceIndex;
            ITicketAsyncEnumerator<TSource> sourceEnumerator;
            ITicketAsyncEnumerator<TCollection> selectedEnumerator;
            Ticket<bool>.Awaiter sourceAwaiter;
            Ticket<bool>.Awaiter selectedAwaiter;
            Ticket.Awaiter selectedDisposeAsyncAwaiter;

            public _SelectMany(ITicketAsyncEnumerable<TSource> source, Func<TSource, ITicketAsyncEnumerable<TCollection>> selector1, Func<TSource, int, ITicketAsyncEnumerable<TCollection>> selector2, Func<TSource, TCollection, TResult> resultSelector, CancellationToken cancellationToken)
            {
                this.source = source;
                this.selector1 = selector1;
                this.selector2 = selector2;
                this.resultSelector = resultSelector;
                this.cancellationToken = cancellationToken;
                TaskTracker.TrackActiveTask(this, 3);
            }

            public TResult Current { get; private set; }

            public Ticket<bool> MoveNextAsync()
            {
                completionSource.Reset();

                // iterate selected field
                if (selectedEnumerator != null)
                {
                    MoveNextSelected();
                }
                else
                {
                    // iterate source field
                    if (sourceEnumerator == null)
                    {
                        sourceEnumerator = source.GetAsyncEnumerator(cancellationToken);
                    }
                    MoveNextSource();
                }

                return new Ticket<bool>(this, completionSource.Version);
            }

            void MoveNextSource()
            {
                try
                {
                    sourceAwaiter = sourceEnumerator.MoveNextAsync().GetAwaiter();
                }
                catch (Exception ex)
                {
                    completionSource.TrySetException(ex);
                    return;
                }

                if (sourceAwaiter.IsCompleted)
                {
                    SourceMoveNextCore(this);
                }
                else
                {
                    sourceAwaiter.SourceOnCompleted(sourceMoveNextCoreDelegate, this);
                }
            }

            void MoveNextSelected()
            {
                try
                {
                    selectedAwaiter = selectedEnumerator.MoveNextAsync().GetAwaiter();
                }
                catch (Exception ex)
                {
                    completionSource.TrySetException(ex);
                    return;
                }

                if (selectedAwaiter.IsCompleted)
                {
                    SeletedSourceMoveNextCore(this);
                }
                else
                {
                    selectedAwaiter.SourceOnCompleted(selectedSourceMoveNextCoreDelegate, this);
                }
            }

            static void SourceMoveNextCore(object state)
            {
                var self = (_SelectMany)state;

                if (self.TryGetResult(self.sourceAwaiter, out var result))
                {
                    if (result)
                    {
                        try
                        {
                            self.sourceCurrent = self.sourceEnumerator.Current;
                            if (self.selector1 != null)
                            {
                                self.selectedEnumerator = self.selector1(self.sourceCurrent).GetAsyncEnumerator(self.cancellationToken);
                            }
                            else
                            {
                                self.selectedEnumerator = self.selector2(self.sourceCurrent, checked(self.sourceIndex++)).GetAsyncEnumerator(self.cancellationToken);
                            }
                        }
                        catch (Exception ex)
                        {
                            self.completionSource.TrySetException(ex);
                            return;
                        }

                        self.MoveNextSelected(); // iterated selected source.
                    }
                    else
                    {
                        self.completionSource.TrySetResult(false);
                    }
                }
            }

            static void SeletedSourceMoveNextCore(object state)
            {
                var self = (_SelectMany)state;

                if (self.TryGetResult(self.selectedAwaiter, out var result))
                {
                    if (result)
                    {
                        try
                        {
                            self.Current = self.resultSelector(self.sourceCurrent, self.selectedEnumerator.Current);
                        }
                        catch (Exception ex)
                        {
                            self.completionSource.TrySetException(ex);
                            return;
                        }

                        self.completionSource.TrySetResult(true);
                    }
                    else
                    {
                        // dispose selected source and try iterate source.
                        try
                        {
                            self.selectedDisposeAsyncAwaiter = self.selectedEnumerator.DisposeAsync().GetAwaiter();
                        }
                        catch (Exception ex)
                        {
                            self.completionSource.TrySetException(ex);
                            return;
                        }
                        if (self.selectedDisposeAsyncAwaiter.IsCompleted)
                        {
                            SelectedEnumeratorDisposeAsyncCore(self);
                        }
                        else
                        {
                            self.selectedDisposeAsyncAwaiter.SourceOnCompleted(selectedEnumeratorDisposeAsyncCoreDelegate, self);
                        }
                    }
                }
            }

            static void SelectedEnumeratorDisposeAsyncCore(object state)
            {
                var self = (_SelectMany)state;

                if (self.TryGetResult(self.selectedDisposeAsyncAwaiter))
                {
                    self.selectedEnumerator = null;
                    self.selectedAwaiter = default;

                    self.MoveNextSource(); // iterate next source
                }
            }

            public async Ticket DisposeAsync()
            {
                TaskTracker.RemoveTracking(this);
                if (selectedEnumerator != null)
                {
                    await selectedEnumerator.DisposeAsync();
                }
                if (sourceEnumerator != null)
                {
                    await sourceEnumerator.DisposeAsync();
                }
            }
        }
    }

    internal sealed class SelectManyAwait<TSource, TCollection, TResult> : ITicketAsyncEnumerable<TResult>
    {
        readonly ITicketAsyncEnumerable<TSource> source;
        readonly Func<TSource, Ticket<ITicketAsyncEnumerable<TCollection>>> selector1;
        readonly Func<TSource, int, Ticket<ITicketAsyncEnumerable<TCollection>>> selector2;
        readonly Func<TSource, TCollection, Ticket<TResult>> resultSelector;

        public SelectManyAwait(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<ITicketAsyncEnumerable<TCollection>>> selector, Func<TSource, TCollection, Ticket<TResult>> resultSelector)
        {
            this.source = source;
            this.selector1 = selector;
            this.selector2 = null;
            this.resultSelector = resultSelector;
        }

        public SelectManyAwait(ITicketAsyncEnumerable<TSource> source, Func<TSource, int, Ticket<ITicketAsyncEnumerable<TCollection>>> selector, Func<TSource, TCollection, Ticket<TResult>> resultSelector)
        {
            this.source = source;
            this.selector1 = null;
            this.selector2 = selector;
            this.resultSelector = resultSelector;
        }

        public ITicketAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _SelectManyAwait(source, selector1, selector2, resultSelector, cancellationToken);
        }

        sealed class _SelectManyAwait : MoveNextSource, ITicketAsyncEnumerator<TResult>
        {
            static readonly Action<object> sourceMoveNextCoreDelegate = SourceMoveNextCore;
            static readonly Action<object> selectedSourceMoveNextCoreDelegate = SeletedSourceMoveNextCore;
            static readonly Action<object> selectedEnumeratorDisposeAsyncCoreDelegate = SelectedEnumeratorDisposeAsyncCore;
            static readonly Action<object> selectorAwaitCoreDelegate = SelectorAwaitCore;
            static readonly Action<object> resultSelectorAwaitCoreDelegate = ResultSelectorAwaitCore;

            readonly ITicketAsyncEnumerable<TSource> source;

            readonly Func<TSource, Ticket<ITicketAsyncEnumerable<TCollection>>> selector1;
            readonly Func<TSource, int, Ticket<ITicketAsyncEnumerable<TCollection>>> selector2;
            readonly Func<TSource, TCollection, Ticket<TResult>> resultSelector;
            CancellationToken cancellationToken;

            TSource sourceCurrent;
            int sourceIndex;
            ITicketAsyncEnumerator<TSource> sourceEnumerator;
            ITicketAsyncEnumerator<TCollection> selectedEnumerator;
            Ticket<bool>.Awaiter sourceAwaiter;
            Ticket<bool>.Awaiter selectedAwaiter;
            Ticket.Awaiter selectedDisposeAsyncAwaiter;

            // await additional
            Ticket<ITicketAsyncEnumerable<TCollection>>.Awaiter collectionSelectorAwaiter;
            Ticket<TResult>.Awaiter resultSelectorAwaiter;

            public _SelectManyAwait(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<ITicketAsyncEnumerable<TCollection>>> selector1, Func<TSource, int, Ticket<ITicketAsyncEnumerable<TCollection>>> selector2, Func<TSource, TCollection, Ticket<TResult>> resultSelector, CancellationToken cancellationToken)
            {
                this.source = source;
                this.selector1 = selector1;
                this.selector2 = selector2;
                this.resultSelector = resultSelector;
                this.cancellationToken = cancellationToken;
                TaskTracker.TrackActiveTask(this, 3);
            }

            public TResult Current { get; private set; }

            public Ticket<bool> MoveNextAsync()
            {
                completionSource.Reset();

                // iterate selected field
                if (selectedEnumerator != null)
                {
                    MoveNextSelected();
                }
                else
                {
                    // iterate source field
                    if (sourceEnumerator == null)
                    {
                        sourceEnumerator = source.GetAsyncEnumerator(cancellationToken);
                    }
                    MoveNextSource();
                }

                return new Ticket<bool>(this, completionSource.Version);
            }

            void MoveNextSource()
            {
                try
                {
                    sourceAwaiter = sourceEnumerator.MoveNextAsync().GetAwaiter();
                }
                catch (Exception ex)
                {
                    completionSource.TrySetException(ex);
                    return;
                }

                if (sourceAwaiter.IsCompleted)
                {
                    SourceMoveNextCore(this);
                }
                else
                {
                    sourceAwaiter.SourceOnCompleted(sourceMoveNextCoreDelegate, this);
                }
            }

            void MoveNextSelected()
            {
                try
                {
                    selectedAwaiter = selectedEnumerator.MoveNextAsync().GetAwaiter();
                }
                catch (Exception ex)
                {
                    completionSource.TrySetException(ex);
                    return;
                }

                if (selectedAwaiter.IsCompleted)
                {
                    SeletedSourceMoveNextCore(this);
                }
                else
                {
                    selectedAwaiter.SourceOnCompleted(selectedSourceMoveNextCoreDelegate, this);
                }
            }

            static void SourceMoveNextCore(object state)
            {
                var self = (_SelectManyAwait)state;

                if (self.TryGetResult(self.sourceAwaiter, out var result))
                {
                    if (result)
                    {
                        try
                        {
                            self.sourceCurrent = self.sourceEnumerator.Current;

                            if (self.selector1 != null)
                            {
                                self.collectionSelectorAwaiter = self.selector1(self.sourceCurrent).GetAwaiter();
                            }
                            else
                            {
                                self.collectionSelectorAwaiter = self.selector2(self.sourceCurrent, checked(self.sourceIndex++)).GetAwaiter();
                            }

                            if (self.collectionSelectorAwaiter.IsCompleted)
                            {
                                SelectorAwaitCore(self);
                            }
                            else
                            {
                                self.collectionSelectorAwaiter.SourceOnCompleted(selectorAwaitCoreDelegate, self);
                            }
                        }
                        catch (Exception ex)
                        {
                            self.completionSource.TrySetException(ex);
                            return;
                        }
                    }
                    else
                    {
                        self.completionSource.TrySetResult(false);
                    }
                }
            }

            static void SeletedSourceMoveNextCore(object state)
            {
                var self = (_SelectManyAwait)state;

                if (self.TryGetResult(self.selectedAwaiter, out var result))
                {
                    if (result)
                    {
                        try
                        {
                            self.resultSelectorAwaiter = self.resultSelector(self.sourceCurrent, self.selectedEnumerator.Current).GetAwaiter();
                            if (self.resultSelectorAwaiter.IsCompleted)
                            {
                                ResultSelectorAwaitCore(self);
                            }
                            else
                            {
                                self.resultSelectorAwaiter.SourceOnCompleted(resultSelectorAwaitCoreDelegate, self);
                            }
                        }
                        catch (Exception ex)
                        {
                            self.completionSource.TrySetException(ex);
                            return;
                        }
                    }
                    else
                    {
                        // dispose selected source and try iterate source.
                        try
                        {
                            self.selectedDisposeAsyncAwaiter = self.selectedEnumerator.DisposeAsync().GetAwaiter();
                        }
                        catch (Exception ex)
                        {
                            self.completionSource.TrySetException(ex);
                            return;
                        }
                        if (self.selectedDisposeAsyncAwaiter.IsCompleted)
                        {
                            SelectedEnumeratorDisposeAsyncCore(self);
                        }
                        else
                        {
                            self.selectedDisposeAsyncAwaiter.SourceOnCompleted(selectedEnumeratorDisposeAsyncCoreDelegate, self);
                        }
                    }
                }
            }

            static void SelectedEnumeratorDisposeAsyncCore(object state)
            {
                var self = (_SelectManyAwait)state;

                if (self.TryGetResult(self.selectedDisposeAsyncAwaiter))
                {
                    self.selectedEnumerator = null;
                    self.selectedAwaiter = default;

                    self.MoveNextSource(); // iterate next source
                }
            }

            static void SelectorAwaitCore(object state)
            {
                var self = (_SelectManyAwait)state;

                if (self.TryGetResult(self.collectionSelectorAwaiter, out var result))
                {
                    self.selectedEnumerator = result.GetAsyncEnumerator(self.cancellationToken);
                    self.MoveNextSelected(); // iterated selected source.
                }
            }

            static void ResultSelectorAwaitCore(object state)
            {
                var self = (_SelectManyAwait)state;

                if (self.TryGetResult(self.resultSelectorAwaiter, out var result))
                {
                    self.Current = result;
                    self.completionSource.TrySetResult(true);
                }
            }

            public async Ticket DisposeAsync()
            {
                TaskTracker.RemoveTracking(this);
                if (selectedEnumerator != null)
                {
                    await selectedEnumerator.DisposeAsync();
                }
                if (sourceEnumerator != null)
                {
                    await sourceEnumerator.DisposeAsync();
                }
            }
        }
    }

    internal sealed class SelectManyAwaitWithCancellation<TSource, TCollection, TResult> : ITicketAsyncEnumerable<TResult>
    {
        readonly ITicketAsyncEnumerable<TSource> source;
        readonly Func<TSource, CancellationToken, Ticket<ITicketAsyncEnumerable<TCollection>>> selector1;
        readonly Func<TSource, int, CancellationToken, Ticket<ITicketAsyncEnumerable<TCollection>>> selector2;
        readonly Func<TSource, TCollection, CancellationToken, Ticket<TResult>> resultSelector;

        public SelectManyAwaitWithCancellation(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<ITicketAsyncEnumerable<TCollection>>> selector, Func<TSource, TCollection, CancellationToken, Ticket<TResult>> resultSelector)
        {
            this.source = source;
            this.selector1 = selector;
            this.selector2 = null;
            this.resultSelector = resultSelector;
        }

        public SelectManyAwaitWithCancellation(ITicketAsyncEnumerable<TSource> source, Func<TSource, int, CancellationToken, Ticket<ITicketAsyncEnumerable<TCollection>>> selector, Func<TSource, TCollection, CancellationToken, Ticket<TResult>> resultSelector)
        {
            this.source = source;
            this.selector1 = null;
            this.selector2 = selector;
            this.resultSelector = resultSelector;
        }

        public ITicketAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _SelectManyAwaitWithCancellation(source, selector1, selector2, resultSelector, cancellationToken);
        }

        sealed class _SelectManyAwaitWithCancellation : MoveNextSource, ITicketAsyncEnumerator<TResult>
        {
            static readonly Action<object> sourceMoveNextCoreDelegate = SourceMoveNextCore;
            static readonly Action<object> selectedSourceMoveNextCoreDelegate = SeletedSourceMoveNextCore;
            static readonly Action<object> selectedEnumeratorDisposeAsyncCoreDelegate = SelectedEnumeratorDisposeAsyncCore;
            static readonly Action<object> selectorAwaitCoreDelegate = SelectorAwaitCore;
            static readonly Action<object> resultSelectorAwaitCoreDelegate = ResultSelectorAwaitCore;

            readonly ITicketAsyncEnumerable<TSource> source;

            readonly Func<TSource, CancellationToken, Ticket<ITicketAsyncEnumerable<TCollection>>> selector1;
            readonly Func<TSource, int, CancellationToken, Ticket<ITicketAsyncEnumerable<TCollection>>> selector2;
            readonly Func<TSource, TCollection, CancellationToken, Ticket<TResult>> resultSelector;
            CancellationToken cancellationToken;

            TSource sourceCurrent;
            int sourceIndex;
            ITicketAsyncEnumerator<TSource> sourceEnumerator;
            ITicketAsyncEnumerator<TCollection> selectedEnumerator;
            Ticket<bool>.Awaiter sourceAwaiter;
            Ticket<bool>.Awaiter selectedAwaiter;
            Ticket.Awaiter selectedDisposeAsyncAwaiter;

            // await additional
            Ticket<ITicketAsyncEnumerable<TCollection>>.Awaiter collectionSelectorAwaiter;
            Ticket<TResult>.Awaiter resultSelectorAwaiter;

            public _SelectManyAwaitWithCancellation(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<ITicketAsyncEnumerable<TCollection>>> selector1, Func<TSource, int, CancellationToken, Ticket<ITicketAsyncEnumerable<TCollection>>> selector2, Func<TSource, TCollection, CancellationToken, Ticket<TResult>> resultSelector, CancellationToken cancellationToken)
            {
                this.source = source;
                this.selector1 = selector1;
                this.selector2 = selector2;
                this.resultSelector = resultSelector;
                this.cancellationToken = cancellationToken;
                TaskTracker.TrackActiveTask(this, 3);
            }

            public TResult Current { get; private set; }

            public Ticket<bool> MoveNextAsync()
            {
                completionSource.Reset();

                // iterate selected field
                if (selectedEnumerator != null)
                {
                    MoveNextSelected();
                }
                else
                {
                    // iterate source field
                    if (sourceEnumerator == null)
                    {
                        sourceEnumerator = source.GetAsyncEnumerator(cancellationToken);
                    }
                    MoveNextSource();
                }

                return new Ticket<bool>(this, completionSource.Version);
            }

            void MoveNextSource()
            {
                try
                {
                    sourceAwaiter = sourceEnumerator.MoveNextAsync().GetAwaiter();
                }
                catch (Exception ex)
                {
                    completionSource.TrySetException(ex);
                    return;
                }

                if (sourceAwaiter.IsCompleted)
                {
                    SourceMoveNextCore(this);
                }
                else
                {
                    sourceAwaiter.SourceOnCompleted(sourceMoveNextCoreDelegate, this);
                }
            }

            void MoveNextSelected()
            {
                try
                {
                    selectedAwaiter = selectedEnumerator.MoveNextAsync().GetAwaiter();
                }
                catch (Exception ex)
                {
                    completionSource.TrySetException(ex);
                    return;
                }

                if (selectedAwaiter.IsCompleted)
                {
                    SeletedSourceMoveNextCore(this);
                }
                else
                {
                    selectedAwaiter.SourceOnCompleted(selectedSourceMoveNextCoreDelegate, this);
                }
            }

            static void SourceMoveNextCore(object state)
            {
                var self = (_SelectManyAwaitWithCancellation)state;

                if (self.TryGetResult(self.sourceAwaiter, out var result))
                {
                    if (result)
                    {
                        try
                        {
                            self.sourceCurrent = self.sourceEnumerator.Current;

                            if (self.selector1 != null)
                            {
                                self.collectionSelectorAwaiter = self.selector1(self.sourceCurrent, self.cancellationToken).GetAwaiter();
                            }
                            else
                            {
                                self.collectionSelectorAwaiter = self.selector2(self.sourceCurrent, checked(self.sourceIndex++), self.cancellationToken).GetAwaiter();
                            }

                            if (self.collectionSelectorAwaiter.IsCompleted)
                            {
                                SelectorAwaitCore(self);
                            }
                            else
                            {
                                self.collectionSelectorAwaiter.SourceOnCompleted(selectorAwaitCoreDelegate, self);
                            }
                        }
                        catch (Exception ex)
                        {
                            self.completionSource.TrySetException(ex);
                            return;
                        }
                    }
                    else
                    {
                        self.completionSource.TrySetResult(false);
                    }
                }
            }

            static void SeletedSourceMoveNextCore(object state)
            {
                var self = (_SelectManyAwaitWithCancellation)state;

                if (self.TryGetResult(self.selectedAwaiter, out var result))
                {
                    if (result)
                    {
                        try
                        {
                            self.resultSelectorAwaiter = self.resultSelector(self.sourceCurrent, self.selectedEnumerator.Current, self.cancellationToken).GetAwaiter();
                            if (self.resultSelectorAwaiter.IsCompleted)
                            {
                                ResultSelectorAwaitCore(self);
                            }
                            else
                            {
                                self.resultSelectorAwaiter.SourceOnCompleted(resultSelectorAwaitCoreDelegate, self);
                            }
                        }
                        catch (Exception ex)
                        {
                            self.completionSource.TrySetException(ex);
                            return;
                        }
                    }
                    else
                    {
                        // dispose selected source and try iterate source.
                        try
                        {
                            self.selectedDisposeAsyncAwaiter = self.selectedEnumerator.DisposeAsync().GetAwaiter();
                        }
                        catch (Exception ex)
                        {
                            self.completionSource.TrySetException(ex);
                            return;
                        }
                        if (self.selectedDisposeAsyncAwaiter.IsCompleted)
                        {
                            SelectedEnumeratorDisposeAsyncCore(self);
                        }
                        else
                        {
                            self.selectedDisposeAsyncAwaiter.SourceOnCompleted(selectedEnumeratorDisposeAsyncCoreDelegate, self);
                        }
                    }
                }
            }

            static void SelectedEnumeratorDisposeAsyncCore(object state)
            {
                var self = (_SelectManyAwaitWithCancellation)state;

                if (self.TryGetResult(self.selectedDisposeAsyncAwaiter))
                {
                    self.selectedEnumerator = null;
                    self.selectedAwaiter = default;

                    self.MoveNextSource(); // iterate next source
                }
            }

            static void SelectorAwaitCore(object state)
            {
                var self = (_SelectManyAwaitWithCancellation)state;

                if (self.TryGetResult(self.collectionSelectorAwaiter, out var result))
                {
                    self.selectedEnumerator = result.GetAsyncEnumerator(self.cancellationToken);
                    self.MoveNextSelected(); // iterated selected source.
                }
            }

            static void ResultSelectorAwaitCore(object state)
            {
                var self = (_SelectManyAwaitWithCancellation)state;

                if (self.TryGetResult(self.resultSelectorAwaiter, out var result))
                {
                    self.Current = result;
                    self.completionSource.TrySetResult(true);
                }
            }

            public async Ticket DisposeAsync()
            {
                TaskTracker.RemoveTracking(this);
                if (selectedEnumerator != null)
                {
                    await selectedEnumerator.DisposeAsync();
                }
                if (sourceEnumerator != null)
                {
                    await sourceEnumerator.DisposeAsync();
                }
            }
        }
    }
}