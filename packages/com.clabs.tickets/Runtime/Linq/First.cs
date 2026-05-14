using System;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static Ticket<TSource> FirstAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return First.FirstAsync(source, cancellationToken, false);
        }

        public static Ticket<TSource> FirstAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Boolean> predicate, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return First.FirstAsync(source, predicate, cancellationToken, false);
        }

        public static Ticket<TSource> FirstAwaitAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Boolean>> predicate, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return First.FirstAwaitAsync(source, predicate, cancellationToken, false);
        }

        public static Ticket<TSource> FirstAwaitWithCancellationAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Boolean>> predicate, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return First.FirstAwaitWithCancellationAsync(source, predicate, cancellationToken, false);
        }

        public static Ticket<TSource> FirstOrDefaultAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return First.FirstAsync(source, cancellationToken, true);
        }

        public static Ticket<TSource> FirstOrDefaultAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Boolean> predicate, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return First.FirstAsync(source, predicate, cancellationToken, true);
        }

        public static Ticket<TSource> FirstOrDefaultAwaitAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Boolean>> predicate, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return First.FirstAwaitAsync(source, predicate, cancellationToken, true);
        }

        public static Ticket<TSource> FirstOrDefaultAwaitWithCancellationAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Boolean>> predicate, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return First.FirstAwaitWithCancellationAsync(source, predicate, cancellationToken, true);
        }
    }

    internal static class First
    {
        public static async Ticket<TSource> FirstAsync<TSource>(ITicketAsyncEnumerable<TSource> source, CancellationToken cancellationToken, bool defaultIfEmpty)
        {
            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                if (await e.MoveNextAsync())
                {
                    return e.Current;
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

        public static async Ticket<TSource> FirstAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Boolean> predicate, CancellationToken cancellationToken, bool defaultIfEmpty)
        {
            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    var v = e.Current;
                    if (predicate(v))
                    {
                        return v;
                    }
                }

                if (defaultIfEmpty)
                {
                    return default;
                }
                else
                {
                    throw Error.NoElements();
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

        public static async Ticket<TSource> FirstAwaitAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Boolean>> predicate, CancellationToken cancellationToken, bool defaultIfEmpty)
        {
            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    var v = e.Current;
                    if (await predicate(v))
                    {
                        return v;
                    }
                }

                if (defaultIfEmpty)
                {
                    return default;
                }
                else
                {
                    throw Error.NoElements();
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

        public static async Ticket<TSource> FirstAwaitWithCancellationAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Boolean>> predicate, CancellationToken cancellationToken, bool defaultIfEmpty)
        {
            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    var v = e.Current;
                    if (await predicate(v, cancellationToken))
                    {
                        return v;
                    }
                }

                if (defaultIfEmpty)
                {
                    return default;
                }
                else
                {
                    throw Error.NoElements();
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