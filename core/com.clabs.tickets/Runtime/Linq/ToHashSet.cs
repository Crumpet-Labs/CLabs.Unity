using System.Collections.Generic;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static Ticket<HashSet<TSource>> ToHashSetAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return ToHashSet.ToHashSetAsync(source, EqualityComparer<TSource>.Default, cancellationToken);
        }

        public static Ticket<HashSet<TSource>> ToHashSetAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, IEqualityComparer<TSource> comparer, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(comparer, nameof(comparer));

            return ToHashSet.ToHashSetAsync(source, comparer, cancellationToken);
        }
    }

    internal static class ToHashSet
    {
        internal static async Ticket<HashSet<TSource>> ToHashSetAsync<TSource>(ITicketAsyncEnumerable<TSource> source, IEqualityComparer<TSource> comparer, CancellationToken cancellationToken)
        {
            var set = new HashSet<TSource>(comparer);

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    set.Add(e.Current);
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return set;
        }
    }
}