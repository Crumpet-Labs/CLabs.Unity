namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static ITicketAsyncEnumerable<TSource> AsTicketAsyncEnumerable<TSource>(this ITicketAsyncEnumerable<TSource> source)
        {
            return source;
        }
    }
}
