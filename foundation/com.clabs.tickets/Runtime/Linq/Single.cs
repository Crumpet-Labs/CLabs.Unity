using System;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static Ticket<TSource> SingleAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return SingleOperator.SingleAsync(source, cancellationToken, false);
        }

        public static Ticket<TSource> SingleAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Boolean> predicate, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return SingleOperator.SingleAsync(source, predicate, cancellationToken, false);
        }

        public static Ticket<TSource> SingleAwaitAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Boolean>> predicate, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return SingleOperator.SingleAwaitAsync(source, predicate, cancellationToken, false);
        }

        public static Ticket<TSource> SingleAwaitWithCancellationAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Boolean>> predicate, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return SingleOperator.SingleAwaitWithCancellationAsync(source, predicate, cancellationToken, false);
        }

        public static Ticket<TSource> SingleOrDefaultAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return SingleOperator.SingleAsync(source, cancellationToken, true);
        }

        public static Ticket<TSource> SingleOrDefaultAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Boolean> predicate, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return SingleOperator.SingleAsync(source, predicate, cancellationToken, true);
        }

        public static Ticket<TSource> SingleOrDefaultAwaitAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Boolean>> predicate, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return SingleOperator.SingleAwaitAsync(source, predicate, cancellationToken, true);
        }

        public static Ticket<TSource> SingleOrDefaultAwaitWithCancellationAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Boolean>> predicate, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return SingleOperator.SingleAwaitWithCancellationAsync(source, predicate, cancellationToken, true);
        }
    }

    internal static class SingleOperator
    {
        public static async Ticket<TSource> SingleAsync<TSource>(ITicketAsyncEnumerable<TSource> source, CancellationToken cancellationToken, bool defaultIfEmpty)
        {
            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                if (await e.MoveNextAsync())
                {
                    var v = e.Current;
                    if (!await e.MoveNextAsync())
                    {
                        return v;
                    }

                    throw Error.MoreThanOneElement();
                }
                else
                {
                    if (defaultIfEmpty)
                    {
                        return default;
                    }
                    else
                    {
                        throw Error.NoElements();
                    }
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }
        }

        public static async Ticket<TSource> SingleAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Boolean> predicate, CancellationToken cancellationToken, bool defaultIfEmpty)
        {
            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                TSource value = default;
                bool found = false;
                while (await e.MoveNextAsync())
                {
                    var v = e.Current;
                    if (predicate(v))
                    {
                        if (found)
                        {
                            throw Error.MoreThanOneElement();
                        }
                        else
                        {
                            found = true;
                            value = v;
                        }
                    }
                }

                if (found || defaultIfEmpty)
                {
                    return value;
                }

                throw Error.NoElements();
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }
        }

        public static async Ticket<TSource> SingleAwaitAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Boolean>> predicate, CancellationToken cancellationToken, bool defaultIfEmpty)
        {
            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                TSource value = default;
                bool found = false;
                while (await e.MoveNextAsync())
                {
                    var v = e.Current;
                    if (await predicate(v))
                    {
                        if (found)
                        {
                            throw Error.MoreThanOneElement();
                        }
                        else
                        {
                            found = true;
                            value = v;
                        }
                    }
                }

                if (found || defaultIfEmpty)
                {
                    return value;
                }

                throw Error.NoElements();
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }
        }

        public static async Ticket<TSource> SingleAwaitWithCancellationAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Boolean>> predicate, CancellationToken cancellationToken, bool defaultIfEmpty)
        {
            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                TSource value = default;
                bool found = false;
                while (await e.MoveNextAsync())
                {
                    var v = e.Current;
                    if (await predicate(v, cancellationToken))
                    {
                        if (found)
                        {
                            throw Error.MoreThanOneElement();
                        }
                        else
                        {
                            found = true;
                            value = v;
                        }
                    }
                }

                if (found || defaultIfEmpty)
                {
                    return value;
                }

                throw Error.NoElements();
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }
        }
    }
}