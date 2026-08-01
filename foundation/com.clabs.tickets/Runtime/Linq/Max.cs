using System;
using System.Collections.Generic;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static Ticket<TSource> MaxAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return Max.MaxAsync(source, cancellationToken);
        }

        public static Ticket<TResult> MaxAsync<TSource, TResult>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, TResult> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Max.MaxAsync(source, selector, cancellationToken);
        }

        public static Ticket<TResult> MaxAwaitAsync<TSource, TResult>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<TResult>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Max.MaxAwaitAsync(source, selector, cancellationToken);
        }

        public static Ticket<TResult> MaxAwaitWithCancellationAsync<TSource, TResult>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<TResult>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Max.MaxAwaitWithCancellationAsync(source, selector, cancellationToken);
        }
    }

    internal static partial class Max
    {
        public static async Ticket<TSource> MaxAsync<TSource>(ITicketAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
        {
            TSource value = default;
            var comparer = Comparer<TSource>.Default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    value = e.Current;

                    goto NEXT_LOOP;
                }

                return value;

                NEXT_LOOP:

                while (await e.MoveNextAsync())
                {
                    var x = e.Current;
                    if (comparer.Compare(value, x) < 0)
                    {
                        value = x;
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

            return value;
        }

        public static async Ticket<TResult> MaxAsync<TSource, TResult>(ITicketAsyncEnumerable<TSource> source, Func<TSource, TResult> selector, CancellationToken cancellationToken)
        {
            TResult value = default;
            var comparer = Comparer<TResult>.Default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    value = selector(e.Current);

                    goto NEXT_LOOP;
                }

                return value;

                NEXT_LOOP:

                while (await e.MoveNextAsync())
                {
                    var x = selector(e.Current);
                    if (comparer.Compare(value, x) < 0)
                    {
                        value = x;
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

            return value;
        }

        public static async Ticket<TResult> MaxAwaitAsync<TSource, TResult>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<TResult>> selector, CancellationToken cancellationToken)
        {
            TResult value = default;
            var comparer = Comparer<TResult>.Default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    value = await selector(e.Current);

                    goto NEXT_LOOP;
                }

                return value;

                NEXT_LOOP:

                while (await e.MoveNextAsync())
                {
                    var x = await selector(e.Current);
                    if (comparer.Compare(value, x) < 0)
                    {
                        value = x;
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

            return value;
        }

        public static async Ticket<TResult> MaxAwaitWithCancellationAsync<TSource, TResult>(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<TResult>> selector, CancellationToken cancellationToken)
        {
            TResult value = default;
            var comparer = Comparer<TResult>.Default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    value = await selector(e.Current, cancellationToken);

                    goto NEXT_LOOP;
                }

                return value;

                NEXT_LOOP:

                while (await e.MoveNextAsync())
                {
                    var x = await selector(e.Current, cancellationToken);
                    if (comparer.Compare(value, x) < 0)
                    {
                        value = x;
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

            return value;
        }
    }
}