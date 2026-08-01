using System;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static Ticket<TSource> ElementAtAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, int index, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return ElementAt.ElementAtAsync(source, index, cancellationToken, false);
        }

        public static Ticket<TSource> ElementAtOrDefaultAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, int index, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return ElementAt.ElementAtAsync(source, index, cancellationToken, true);
        }
    }

    internal static class ElementAt
    {
        public static async Ticket<TSource> ElementAtAsync<TSource>(ITicketAsyncEnumerable<TSource> source, int index, CancellationToken cancellationToken, bool defaultIfEmpty)
        {
            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                int i = 0;
                while (await e.MoveNextAsync())
                {
                    if (i++ == index)
                    {
                        return e.Current;
                    }
                }

                if (defaultIfEmpty)
                {
                    return default;
                }
                else
                {
                    throw Error.ArgumentOutOfRange(nameof(index));
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