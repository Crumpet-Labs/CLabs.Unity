using System.Collections.Generic;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static Ticket<List<TSource>> ToListAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return ToList.ToListAsync(source, cancellationToken);
        }
    }

    internal static class ToList
    {
        internal static async Ticket<List<TSource>> ToListAsync<TSource>(ITicketAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
        {
            var list = new List<TSource>();

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    list.Add(e.Current);
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return list;
        }
    }
}