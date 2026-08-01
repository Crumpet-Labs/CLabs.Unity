using System.Collections.Generic;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static ITicketAsyncEnumerable<TSource> Union<TSource>(this ITicketAsyncEnumerable<TSource> first, ITicketAsyncEnumerable<TSource> second)
        {
            Error.ThrowArgumentNullException(first, nameof(first));
            Error.ThrowArgumentNullException(second, nameof(second));

            return Union<TSource>(first, second, EqualityComparer<TSource>.Default);
        }

        public static ITicketAsyncEnumerable<TSource> Union<TSource>(this ITicketAsyncEnumerable<TSource> first, ITicketAsyncEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            Error.ThrowArgumentNullException(first, nameof(first));
            Error.ThrowArgumentNullException(second, nameof(second));
            Error.ThrowArgumentNullException(comparer, nameof(comparer));

            // improv without combinate?
            return first.Concat(second).Distinct(comparer);
        }
    }
}