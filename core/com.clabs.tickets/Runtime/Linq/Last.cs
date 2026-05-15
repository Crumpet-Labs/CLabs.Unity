using System;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static Ticket<TSource> LastAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return Last.LastAsync(source, cancellationToken, false);
        }

        public static Ticket<TSource> LastAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Boolean> predicate, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return Last.LastAsync(source, predicate, cancellationToken, false);
        }

        public static Ticket<TSource> LastAwaitAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Boolean>> predicate, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return Last.LastAwaitAsync(source, predicate, cancellationToken, false);
        }

        public static Ticket<TSource> LastAwaitWithCancellationAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Boolean>> predicate, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return Last.LastAwaitWithCancellationAsync(source, predicate, cancellationToken, false);
        }

        public static Ticket<TSource> LastOrDefaultAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return Last.LastAsync(source, cancellationToken, true);
        }

        public static Ticket<TSource> LastOrDefaultAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Boolean> predicate, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return Last.LastAsync(source, predicate, cancellationToken, true);
        }

        public static Ticket<TSource> LastOrDefaultAwaitAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Boolean>> predicate, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return Last.LastAwaitAsync(source, predicate, cancellationToken, true);
        }

        public static Ticket<TSource> LastOrDefaultAwaitWithCancellationAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Boolean>> predicate, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return Last.LastAwaitWithCancellationAsync(source, predicate, cancellationToken, true);
        }
    }

    internal static class Last
    {
        public static async Ticket<TSource> LastAsync<TSource>(ITicketAsyncEnumerable<TSource> source, CancellationToken cancellationToken, bool defaultIfEmpty)
        {
            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                TSource value = default;
                if (await e.MoveNextAsync())
                {
                    value = e.Current;
                }
                else
                {
                    if (defaultIfEmpty)
                    {
                        return value;
                    }
                    else
                    {
                        throw Error.NoElements();
                    }
                }

                while (await e.MoveNextAsync())
                {
                    value = e.Current;
                }
                return value;
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }
        }

        public static async Ticket<TSource> LastAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Boolean> predicate, CancellationToken cancellationToken, bool defaultIfEmpty)
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
                        found = true;
                        value = v;
                    }
                }

                if (defaultIfEmpty)
                {
                    return value;
                }
                else
                {
                    if (found)
                    {
                        return value;
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

        public static async Ticket<TSource> LastAwaitAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Boolean>> predicate, CancellationToken cancellationToken, bool defaultIfEmpty)
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
                        found = true;
                        value = v;
                    }
                }

                if (defaultIfEmpty)
                {
                    return value;
                }
                else
                {
                    if (found)
                    {
                        return value;
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

        public static async Ticket<TSource> LastAwaitWithCancellationAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Boolean>> predicate, CancellationToken cancellationToken, bool defaultIfEmpty)
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
                        found = true;
                        value = v;
                    }
                }

                if (defaultIfEmpty)
                {
                    return value;
                }
                else
                {
                    if (found)
                    {
                        return value;
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
    }
}