using System;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static Ticket<Int32> CountAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return Count.CountAsync(source, cancellationToken);
        }

        public static Ticket<Int32> CountAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Boolean> predicate, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return Count.CountAsync(source, predicate, cancellationToken);
        }

        public static Ticket<Int32> CountAwaitAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Boolean>> predicate, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return Count.CountAwaitAsync(source, predicate, cancellationToken);
        }

        public static Ticket<Int32> CountAwaitWithCancellationAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Boolean>> predicate, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(predicate, nameof(predicate));

            return Count.CountAwaitWithCancellationAsync(source, predicate, cancellationToken);
        }
    }

    internal static class Count
    {
        internal static async Ticket<int> CountAsync<TSource>(ITicketAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
        {
            var count = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    checked { count++; }
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return count;
        }

        internal static async Ticket<int> CountAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Boolean> predicate, CancellationToken cancellationToken)
        {
            var count = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    if (predicate(e.Current))
                    {
                        checked { count++; }
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

            return count;
        }

        internal static async Ticket<int> CountAwaitAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Boolean>> predicate, CancellationToken cancellationToken)
        {
            var count = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    if (await predicate(e.Current))
                    {
                        checked { count++; }
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

            return count;
        }

        internal static async Ticket<int> CountAwaitWithCancellationAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Boolean>> predicate, CancellationToken cancellationToken)
        {
            var count = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    if (await predicate(e.Current, cancellationToken))
                    {
                        checked { count++; }
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

            return count;
        }
    }
}