using System;
using System.Collections.Generic;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static Ticket<TSource> AggregateAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, TSource, TSource> accumulator, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(accumulator, nameof(accumulator));

            return Aggregate.AggregateAsync(source, accumulator, cancellationToken);
        }

        public static Ticket<TAccumulate> AggregateAsync<TSource, TAccumulate>(this ITicketAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(accumulator, nameof(accumulator));

            return Aggregate.AggregateAsync(source, seed, accumulator, cancellationToken);
        }

        public static Ticket<TResult> AggregateAsync<TSource, TAccumulate, TResult>(this ITicketAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator, Func<TAccumulate, TResult> resultSelector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(accumulator, nameof(accumulator));
            Error.ThrowArgumentNullException(accumulator, nameof(resultSelector));

            return Aggregate.AggregateAsync(source, seed, accumulator, resultSelector, cancellationToken);
        }

        public static Ticket<TSource> AggregateAwaitAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, TSource, Ticket<TSource>> accumulator, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(accumulator, nameof(accumulator));

            return Aggregate.AggregateAwaitAsync(source, accumulator, cancellationToken);
        }

        public static Ticket<TAccumulate> AggregateAwaitAsync<TSource, TAccumulate>(this ITicketAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, Ticket<TAccumulate>> accumulator, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(accumulator, nameof(accumulator));

            return Aggregate.AggregateAwaitAsync(source, seed, accumulator, cancellationToken);
        }

        public static Ticket<TResult> AggregateAwaitAsync<TSource, TAccumulate, TResult>(this ITicketAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, Ticket<TAccumulate>> accumulator, Func<TAccumulate, Ticket<TResult>> resultSelector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(accumulator, nameof(accumulator));
            Error.ThrowArgumentNullException(accumulator, nameof(resultSelector));

            return Aggregate.AggregateAwaitAsync(source, seed, accumulator, resultSelector, cancellationToken);
        }

        public static Ticket<TSource> AggregateAwaitWithCancellationAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, TSource, CancellationToken, Ticket<TSource>> accumulator, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(accumulator, nameof(accumulator));

            return Aggregate.AggregateAwaitWithCancellationAsync(source, accumulator, cancellationToken);
        }

        public static Ticket<TAccumulate> AggregateAwaitWithCancellationAsync<TSource, TAccumulate>(this ITicketAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, CancellationToken, Ticket<TAccumulate>> accumulator, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(accumulator, nameof(accumulator));

            return Aggregate.AggregateAwaitWithCancellationAsync(source, seed, accumulator, cancellationToken);
        }

        public static Ticket<TResult> AggregateAwaitWithCancellationAsync<TSource, TAccumulate, TResult>(this ITicketAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, CancellationToken, Ticket<TAccumulate>> accumulator, Func<TAccumulate, CancellationToken, Ticket<TResult>> resultSelector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(accumulator, nameof(accumulator));
            Error.ThrowArgumentNullException(accumulator, nameof(resultSelector));

            return Aggregate.AggregateAwaitWithCancellationAsync(source, seed, accumulator, resultSelector, cancellationToken);
        }
    }

    internal static class Aggregate
    {
        internal static async Ticket<TSource> AggregateAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, TSource, TSource> accumulator, CancellationToken cancellationToken)
        {
            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                TSource value;
                if (await e.MoveNextAsync())
                {
                    value = e.Current;
                }
                else
                {
                    throw Error.NoElements();
                }

                while (await e.MoveNextAsync())
                {
                    value = accumulator(value, e.Current);
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

        internal static async Ticket<TAccumulate> AggregateAsync<TSource, TAccumulate>(ITicketAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator, CancellationToken cancellationToken)
        {
            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                TAccumulate value = seed;
                while (await e.MoveNextAsync())
                {
                    value = accumulator(value, e.Current);
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

        internal static async Ticket<TResult> AggregateAsync<TSource, TAccumulate, TResult>(ITicketAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator, Func<TAccumulate, TResult> resultSelector, CancellationToken cancellationToken)
        {
            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                TAccumulate value = seed;
                while (await e.MoveNextAsync())
                {
                    value = accumulator(value, e.Current);
                }
                return resultSelector(value);

            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }
        }

        // with async

        internal static async Ticket<TSource> AggregateAwaitAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, TSource, Ticket<TSource>> accumulator, CancellationToken cancellationToken)
        {
            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                TSource value;
                if (await e.MoveNextAsync())
                {
                    value = e.Current;
                }
                else
                {
                    throw Error.NoElements();
                }

                while (await e.MoveNextAsync())
                {
                    value = await accumulator(value, e.Current);
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

        internal static async Ticket<TAccumulate> AggregateAwaitAsync<TSource, TAccumulate>(ITicketAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, Ticket<TAccumulate>> accumulator, CancellationToken cancellationToken)
        {
            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                TAccumulate value = seed;
                while (await e.MoveNextAsync())
                {
                    value = await accumulator(value, e.Current);
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

        internal static async Ticket<TResult> AggregateAwaitAsync<TSource, TAccumulate, TResult>(ITicketAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, Ticket<TAccumulate>> accumulator, Func<TAccumulate, Ticket<TResult>> resultSelector, CancellationToken cancellationToken)
        {
            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                TAccumulate value = seed;
                while (await e.MoveNextAsync())
                {
                    value = await accumulator(value, e.Current);
                }
                return await resultSelector(value);

            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }
        }


        // with cancellation

        internal static async Ticket<TSource> AggregateAwaitWithCancellationAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, TSource, CancellationToken, Ticket<TSource>> accumulator, CancellationToken cancellationToken)
        {
            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                TSource value;
                if (await e.MoveNextAsync())
                {
                    value = e.Current;
                }
                else
                {
                    throw Error.NoElements();
                }

                while (await e.MoveNextAsync())
                {
                    value = await accumulator(value, e.Current, cancellationToken);
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

        internal static async Ticket<TAccumulate> AggregateAwaitWithCancellationAsync<TSource, TAccumulate>(ITicketAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, CancellationToken, Ticket<TAccumulate>> accumulator, CancellationToken cancellationToken)
        {
            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                TAccumulate value = seed;
                while (await e.MoveNextAsync())
                {
                    value = await accumulator(value, e.Current, cancellationToken);
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

        internal static async Ticket<TResult> AggregateAwaitWithCancellationAsync<TSource, TAccumulate, TResult>(ITicketAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, CancellationToken, Ticket<TAccumulate>> accumulator, Func<TAccumulate, CancellationToken, Ticket<TResult>> resultSelector, CancellationToken cancellationToken)
        {
            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                TAccumulate value = seed;
                while (await e.MoveNextAsync())
                {
                    value = await accumulator(value, e.Current, cancellationToken);
                }
                return await resultSelector(value, cancellationToken);

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