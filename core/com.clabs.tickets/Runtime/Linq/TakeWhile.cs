using System;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static ITicketAsyncEnumerable<TSource> TakeWhile<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Boolean> predicate)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return new TakeWhile<TSource>(source, predicate);
        }

        public static ITicketAsyncEnumerable<TSource> TakeWhile<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Int32, Boolean> predicate)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return new TakeWhileInt<TSource>(source, predicate);
        }

        public static ITicketAsyncEnumerable<TSource> TakeWhileAwait<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Boolean>> predicate)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return new TakeWhileAwait<TSource>(source, predicate);
        }

        public static ITicketAsyncEnumerable<TSource> TakeWhileAwait<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Int32, Ticket<Boolean>> predicate)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return new TakeWhileIntAwait<TSource>(source, predicate);
        }

        public static ITicketAsyncEnumerable<TSource> TakeWhileAwaitWithCancellation<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Boolean>> predicate)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return new TakeWhileAwaitWithCancellation<TSource>(source, predicate);
        }

        public static ITicketAsyncEnumerable<TSource> TakeWhileAwaitWithCancellation<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Int32, CancellationToken, Ticket<Boolean>> predicate)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return new TakeWhileIntAwaitWithCancellation<TSource>(source, predicate);
        }
    }

    internal sealed class TakeWhile<TSource> : ITicketAsyncEnumerable<TSource>
    {
        readonly ITicketAsyncEnumerable<TSource> source;
        readonly Func<TSource, bool> predicate;

        public TakeWhile(ITicketAsyncEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            this.source = source;
            this.predicate = predicate;
        }

        public ITicketAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _TakeWhile(source, predicate, cancellationToken);
        }

        class _TakeWhile : AsyncEnumeratorBase<TSource, TSource>
        {
            Func<TSource, bool> predicate;

            public _TakeWhile(ITicketAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken)

                : base(source, cancellationToken)
            {
                this.predicate = predicate;
            }

            protected override bool TryMoveNextCore(bool sourceHasCurrent, out bool result)
            {
                if (sourceHasCurrent)
                {
                    if (predicate(SourceCurrent))
                    {
                        Current = SourceCurrent;
                        result = true;
                        return true;
                    }
                }

                result = false;
                return true;
            }
        }
    }

    internal sealed class TakeWhileInt<TSource> : ITicketAsyncEnumerable<TSource>
    {
        readonly ITicketAsyncEnumerable<TSource> source;
        readonly Func<TSource, int, bool> predicate;

        public TakeWhileInt(ITicketAsyncEnumerable<TSource> source, Func<TSource, int, bool> predicate)
        {
            this.source = source;
            this.predicate = predicate;
        }

        public ITicketAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _TakeWhileInt(source, predicate, cancellationToken);
        }

        class _TakeWhileInt : AsyncEnumeratorBase<TSource, TSource>
        {
            readonly Func<TSource, int, bool> predicate;
            int index;

            public _TakeWhileInt(ITicketAsyncEnumerable<TSource> source, Func<TSource, int, bool> predicate, CancellationToken cancellationToken)

                : base(source, cancellationToken)
            {
                this.predicate = predicate;
            }

            protected override bool TryMoveNextCore(bool sourceHasCurrent, out bool result)
            {
                if (sourceHasCurrent)
                {
                    if (predicate(SourceCurrent, checked(index++)))
                    {
                        Current = SourceCurrent;
                        result = true;
                        return true;
                    }
                }

                result = false;
                return true;
            }
        }
    }

    internal sealed class TakeWhileAwait<TSource> : ITicketAsyncEnumerable<TSource>
    {
        readonly ITicketAsyncEnumerable<TSource> source;
        readonly Func<TSource, Ticket<bool>> predicate;

        public TakeWhileAwait(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<bool>> predicate)
        {
            this.source = source;
            this.predicate = predicate;
        }

        public ITicketAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _TakeWhileAwait(source, predicate, cancellationToken);
        }

        class _TakeWhileAwait : AsyncEnumeratorAwaitSelectorBase<TSource, TSource, bool>
        {
            Func<TSource, Ticket<bool>> predicate;

            public _TakeWhileAwait(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<bool>> predicate, CancellationToken cancellationToken)
                : base(source, cancellationToken)
            {
                this.predicate = predicate;
            }

            protected override Ticket<bool> TransformAsync(TSource sourceCurrent)
            {
                return predicate(sourceCurrent);
            }

            protected override bool TrySetCurrentCore(bool awaitResult, out bool terminateIteration)
            {
                if (awaitResult)
                {
                    Current = SourceCurrent;
                    terminateIteration = false;
                    return true;
                }
                else
                {
                    terminateIteration = true;
                    return false;
                }
            }
        }
    }

    internal sealed class TakeWhileIntAwait<TSource> : ITicketAsyncEnumerable<TSource>
    {
        readonly ITicketAsyncEnumerable<TSource> source;
        readonly Func<TSource, int, Ticket<bool>> predicate;

        public TakeWhileIntAwait(ITicketAsyncEnumerable<TSource> source, Func<TSource, int, Ticket<bool>> predicate)
        {
            this.source = source;
            this.predicate = predicate;
        }

        public ITicketAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _TakeWhileIntAwait(source, predicate, cancellationToken);
        }

        class _TakeWhileIntAwait : AsyncEnumeratorAwaitSelectorBase<TSource, TSource, bool>
        {
            readonly Func<TSource, int, Ticket<bool>> predicate;
            int index;

            public _TakeWhileIntAwait(ITicketAsyncEnumerable<TSource> source, Func<TSource, int, Ticket<bool>> predicate, CancellationToken cancellationToken)
                : base(source, cancellationToken)
            {
                this.predicate = predicate;
            }

            protected override Ticket<bool> TransformAsync(TSource sourceCurrent)
            {
                return predicate(sourceCurrent, checked(index++));
            }

            protected override bool TrySetCurrentCore(bool awaitResult, out bool terminateIteration)
            {
                if (awaitResult)
                {
                    Current = SourceCurrent;
                    terminateIteration = false;
                    return true;
                }
                else
                {
                    terminateIteration = true;
                    return false;
                }
            }
        }
    }

    internal sealed class TakeWhileAwaitWithCancellation<TSource> : ITicketAsyncEnumerable<TSource>
    {
        readonly ITicketAsyncEnumerable<TSource> source;
        readonly Func<TSource, CancellationToken, Ticket<bool>> predicate;

        public TakeWhileAwaitWithCancellation(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<bool>> predicate)
        {
            this.source = source;
            this.predicate = predicate;
        }

        public ITicketAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _TakeWhileAwaitWithCancellation(source, predicate, cancellationToken);
        }

        class _TakeWhileAwaitWithCancellation : AsyncEnumeratorAwaitSelectorBase<TSource, TSource, bool>
        {
            Func<TSource, CancellationToken, Ticket<bool>> predicate;

            public _TakeWhileAwaitWithCancellation(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<bool>> predicate, CancellationToken cancellationToken)
                : base(source, cancellationToken)
            {
                this.predicate = predicate;
            }

            protected override Ticket<bool> TransformAsync(TSource sourceCurrent)
            {
                return predicate(sourceCurrent, cancellationToken);
            }

            protected override bool TrySetCurrentCore(bool awaitResult, out bool terminateIteration)
            {
                if (awaitResult)
                {
                    Current = SourceCurrent;
                    terminateIteration = false;
                    return true;
                }
                else
                {
                    terminateIteration = true;
                    return false;
                }
            }
        }
    }

    internal sealed class TakeWhileIntAwaitWithCancellation<TSource> : ITicketAsyncEnumerable<TSource>
    {
        readonly ITicketAsyncEnumerable<TSource> source;
        readonly Func<TSource, int, CancellationToken, Ticket<bool>> predicate;

        public TakeWhileIntAwaitWithCancellation(ITicketAsyncEnumerable<TSource> source, Func<TSource, int, CancellationToken, Ticket<bool>> predicate)
        {
            this.source = source;
            this.predicate = predicate;
        }

        public ITicketAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _TakeWhileIntAwaitWithCancellation(source, predicate, cancellationToken);
        }

        class _TakeWhileIntAwaitWithCancellation : AsyncEnumeratorAwaitSelectorBase<TSource, TSource, bool>
        {
            readonly Func<TSource, int, CancellationToken, Ticket<bool>> predicate;
            int index;

            public _TakeWhileIntAwaitWithCancellation(ITicketAsyncEnumerable<TSource> source, Func<TSource, int, CancellationToken, Ticket<bool>> predicate, CancellationToken cancellationToken)
                : base(source, cancellationToken)
            {
                this.predicate = predicate;
            }

            protected override Ticket<bool> TransformAsync(TSource sourceCurrent)
            {
                return predicate(sourceCurrent, checked(index++), cancellationToken);
            }

            protected override bool TrySetCurrentCore(bool awaitResult, out bool terminateIteration)
            {
                if (awaitResult)
                {
                    Current = SourceCurrent;
                    terminateIteration = false;
                    return true;
                }
                else
                {
                    terminateIteration = true;
                    return false;
                }
            }
        }
    }
}