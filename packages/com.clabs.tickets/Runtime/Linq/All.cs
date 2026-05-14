using System;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static Ticket<Boolean> AllAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Boolean> predicate, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return All.AllAsync(source, predicate, cancellationToken);
        }

        public static Ticket<Boolean> AllAwaitAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Boolean>> predicate, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return All.AllAwaitAsync(source, predicate, cancellationToken);
        }

        public static Ticket<Boolean> AllAwaitWithCancellationAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Boolean>> predicate, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return All.AllAwaitWithCancellationAsync(source, predicate, cancellationToken);
        }
    }

    internal static class All
    {
        internal static async Ticket<bool> AllAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Boolean> predicate, CancellationToken cancellationToken)
        {
            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    if (!predicate(e.Current))
                    {
                        return false;
                    }
                }

                return true;
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }
        }

        internal static async Ticket<bool> AllAwaitAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Boolean>> predicate, CancellationToken cancellationToken)
        {
            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    if (!await predicate(e.Current))
                    {
                        return false;
                    }
                }

                return true;
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }
        }

        internal static async Ticket<bool> AllAwaitWithCancellationAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Boolean>> predicate, CancellationToken cancellationToken)
        {
            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    if (!await predicate(e.Current, cancellationToken))
                    {
                        return false;
                    }
                }

                return true;
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