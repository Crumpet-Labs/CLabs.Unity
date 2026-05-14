using System;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static ITicketAsyncEnumerable<TSource> SkipWhile<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Boolean> predicate)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return new SkipWhile<TSource>(source, predicate);
        }

        public static ITicketAsyncEnumerable<TSource> SkipWhile<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Int32, Boolean> predicate)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return new SkipWhileInt<TSource>(source, predicate);
        }

        public static ITicketAsyncEnumerable<TSource> SkipWhileAwait<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Boolean>> predicate)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return new SkipWhileAwait<TSource>(source, predicate);
        }

        public static ITicketAsyncEnumerable<TSource> SkipWhileAwait<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Int32, Ticket<Boolean>> predicate)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return new SkipWhileIntAwait<TSource>(source, predicate);
        }

        public static ITicketAsyncEnumerable<TSource> SkipWhileAwaitWithCancellation<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Boolean>> predicate)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return new SkipWhileAwaitWithCancellation<TSource>(source, predicate);
        }

        public static ITicketAsyncEnumerable<TSource> SkipWhileAwaitWithCancellation<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Int32, CancellationToken, Ticket<Boolean>> predicate)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return new SkipWhileIntAwaitWithCancellation<TSource>(source, predicate);
        }
    }

    internal sealed class SkipWhile<TSource> : ITicketAsyncEnumerable<TSource>
    {
        readonly ITicketAsyncEnumerable<TSource> source;
        readonly Func<TSource, bool> predicate;

        public SkipWhile(ITicketAsyncEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            this.source = source;
            this.predicate = predicate;
        }

        public ITicketAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _SkipWhile(source, predicate, cancellationToken);
        }

        class _SkipWhile : AsyncEnumeratorBase<TSource, TSource>
        {
            Func<TSource, bool> predicate;

            public _SkipWhile(ITicketAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken)

                : base(source, cancellationToken)
            {
                this.predicate = predicate;
            }

            protected override bool TryMoveNextCore(bool sourceHasCurrent, out bool result)
            {
                if (sourceHasCurrent)
                {
                    if (predicate == null || !predicate(SourceCurrent))
                    {
                        predicate = null;
                        Current = SourceCurrent;
                        result = true;
                        return true;
                    }
                    else
                    {
                        result = default;
                        return false;
                    }
                }

                result = false;
                return true;
            }
        }
    }

    internal sealed class SkipWhileInt<TSource> : ITicketAsyncEnumerable<TSource>
    {
        readonly ITicketAsyncEnumerable<TSource> source;
        readonly Func<TSource, int, bool> predicate;

        public SkipWhileInt(ITicketAsyncEnumerable<TSource> source, Func<TSource, int, bool> predicate)
        {
            this.source = source;
            this.predicate = predicate;
        }

        public ITicketAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _SkipWhileInt(source, predicate, cancellationToken);
        }

        class _SkipWhileInt : AsyncEnumeratorBase<TSource, TSource>
        {
            Func<TSource, int, bool> predicate;
            int index;

            public _SkipWhileInt(ITicketAsyncEnumerable<TSource> source, Func<TSource, int, bool> predicate, CancellationToken cancellationToken)

                : base(source, cancellationToken)
            {
                this.predicate = predicate;
            }

            protected override bool TryMoveNextCore(bool sourceHasCurrent, out bool result)
            {
                if (sourceHasCurrent)
                {
                    if (predicate == null || !predicate(SourceCurrent, checked(index++)))
                    {
                        predicate = null;
                        Current = SourceCurrent;
                        result = true;
                        return true;
                    }
                    else
                    {
                        result = default;
                        return false;
                    }
                }

                result = false;
                return true;
            }
        }
    }

    internal sealed class SkipWhileAwait<TSource> : ITicketAsyncEnumerable<TSource>
    {
        readonly ITicketAsyncEnumerable<TSource> source;
        readonly Func<TSource, Ticket<bool>> predicate;

        public SkipWhileAwait(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<bool>> predicate)
        {
            this.source = source;
            this.predicate = predicate;
        }

        public ITicketAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _SkipWhileAwait(source, predicate, cancellationToken);
        }

        class _SkipWhileAwait : AsyncEnumeratorAwaitSelectorBase<TSource, TSource, bool>
        {
            Func<TSource, Ticket<bool>> predicate;

            public _SkipWhileAwait(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<bool>> predicate, CancellationToken cancellationToken)

                : base(source, cancellationToken)
            {
                this.predicate = predicate;
            }

            protected override Ticket<bool> TransformAsync(TSource sourceCurrent)
            {
                if (predicate == null)
                {
                    return CompletedTasks.False;
                }

                return predicate(sourceCurrent);
            }

            protected override bool TrySetCurrentCore(bool awaitResult, out bool terminateIteration)
            {
                if (!awaitResult)
                {
                    predicate = null;
                    Current = SourceCurrent;
                    terminateIteration= false;
                    return true;
                }
                else
                {
                    terminateIteration= false;
                    return false;
                }
            }
        }
    }

    internal sealed class SkipWhileIntAwait<TSource> : ITicketAsyncEnumerable<TSource>
    {
        readonly ITicketAsyncEnumerable<TSource> source;
        readonly Func<TSource, int, Ticket<bool>> predicate;

        public SkipWhileIntAwait(ITicketAsyncEnumerable<TSource> source, Func<TSource, int, Ticket<bool>> predicate)
        {
            this.source = source;
            this.predicate = predicate;
        }

        public ITicketAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _SkipWhileIntAwait(source, predicate, cancellationToken);
        }

        class _SkipWhileIntAwait : AsyncEnumeratorAwaitSelectorBase<TSource, TSource, bool>
        {
            Func<TSource, int, Ticket<bool>> predicate;
            int index;

            public _SkipWhileIntAwait(ITicketAsyncEnumerable<TSource> source, Func<TSource, int, Ticket<bool>> predicate, CancellationToken cancellationToken)

                : base(source, cancellationToken)
            {
                this.predicate = predicate;
            }

            protected override Ticket<bool> TransformAsync(TSource sourceCurrent)
            {
                if (predicate == null)
                {
                    return CompletedTasks.False;
                }

                return predicate(sourceCurrent, checked(index++));
            }

            protected override bool TrySetCurrentCore(bool awaitResult, out bool terminateIteration)
            {
                terminateIteration= false;
                if (!awaitResult)
                {
                    predicate = null;
                    Current = SourceCurrent;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }

    internal sealed class SkipWhileAwaitWithCancellation<TSource> : ITicketAsyncEnumerable<TSource>
    {
        readonly ITicketAsyncEnumerable<TSource> source;
        readonly Func<TSource, CancellationToken, Ticket<bool>> predicate;

        public SkipWhileAwaitWithCancellation(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<bool>> predicate)
        {
            this.source = source;
            this.predicate = predicate;
        }

        public ITicketAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _SkipWhileAwaitWithCancellation(source, predicate, cancellationToken);
        }

        class _SkipWhileAwaitWithCancellation : AsyncEnumeratorAwaitSelectorBase<TSource, TSource, bool>
        {
            Func<TSource, CancellationToken, Ticket<bool>> predicate;

            public _SkipWhileAwaitWithCancellation(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<bool>> predicate, CancellationToken cancellationToken)

                : base(source, cancellationToken)
            {
                this.predicate = predicate;
            }

            protected override Ticket<bool> TransformAsync(TSource sourceCurrent)
            {
                if (predicate == null)
                {
                    return CompletedTasks.False;
                }

                return predicate(sourceCurrent, cancellationToken);
            }

            protected override bool TrySetCurrentCore(bool awaitResult, out bool terminateIteration)
            {
                terminateIteration= false;
                if (!awaitResult)
                {
                    predicate = null;
                    Current = SourceCurrent;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }

    internal sealed class SkipWhileIntAwaitWithCancellation<TSource> : ITicketAsyncEnumerable<TSource>
    {
        readonly ITicketAsyncEnumerable<TSource> source;
        readonly Func<TSource, int, CancellationToken, Ticket<bool>> predicate;

        public SkipWhileIntAwaitWithCancellation(ITicketAsyncEnumerable<TSource> source, Func<TSource, int, CancellationToken, Ticket<bool>> predicate)
        {
            this.source = source;
            this.predicate = predicate;
        }

        public ITicketAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _SkipWhileIntAwaitWithCancellation(source, predicate, cancellationToken);
        }

        class _SkipWhileIntAwaitWithCancellation : AsyncEnumeratorAwaitSelectorBase<TSource, TSource, bool>
        {
            Func<TSource, int, CancellationToken, Ticket<bool>> predicate;
            int index;

            public _SkipWhileIntAwaitWithCancellation(ITicketAsyncEnumerable<TSource> source, Func<TSource, int, CancellationToken, Ticket<bool>> predicate, CancellationToken cancellationToken)

                : base(source, cancellationToken)
            {
                this.predicate = predicate;
            }

            protected override Ticket<bool> TransformAsync(TSource sourceCurrent)
            {
                if (predicate == null)
                {
                    return CompletedTasks.False;
                }

                return predicate(sourceCurrent, checked(index++), cancellationToken);
            }

            protected override bool TrySetCurrentCore(bool awaitResult, out bool terminateIteration)
            {
                terminateIteration= false;
                if (!awaitResult)
                {
                    predicate = null;
                    Current = SourceCurrent;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}