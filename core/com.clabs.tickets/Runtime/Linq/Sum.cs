using System;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static Ticket<Int32> SumAsync(this ITicketAsyncEnumerable<Int32> source, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return Sum.SumAsync(source, cancellationToken);
        }

        public static Ticket<Int32> SumAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Int32> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Sum.SumAsync(source, selector, cancellationToken);
        }

        public static Ticket<Int32> SumAwaitAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Int32>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Sum.SumAwaitAsync(source, selector, cancellationToken);
        }

        public static Ticket<Int32> SumAwaitWithCancellationAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Int32>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Sum.SumAwaitWithCancellationAsync(source, selector, cancellationToken);
        }

        public static Ticket<Int64> SumAsync(this ITicketAsyncEnumerable<Int64> source, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return Sum.SumAsync(source, cancellationToken);
        }

        public static Ticket<Int64> SumAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Int64> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Sum.SumAsync(source, selector, cancellationToken);
        }

        public static Ticket<Int64> SumAwaitAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Int64>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Sum.SumAwaitAsync(source, selector, cancellationToken);
        }

        public static Ticket<Int64> SumAwaitWithCancellationAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Int64>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Sum.SumAwaitWithCancellationAsync(source, selector, cancellationToken);
        }

        public static Ticket<Single> SumAsync(this ITicketAsyncEnumerable<Single> source, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return Sum.SumAsync(source, cancellationToken);
        }

        public static Ticket<Single> SumAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Single> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Sum.SumAsync(source, selector, cancellationToken);
        }

        public static Ticket<Single> SumAwaitAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Single>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Sum.SumAwaitAsync(source, selector, cancellationToken);
        }

        public static Ticket<Single> SumAwaitWithCancellationAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Single>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Sum.SumAwaitWithCancellationAsync(source, selector, cancellationToken);
        }

        public static Ticket<Double> SumAsync(this ITicketAsyncEnumerable<Double> source, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return Sum.SumAsync(source, cancellationToken);
        }

        public static Ticket<Double> SumAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Double> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Sum.SumAsync(source, selector, cancellationToken);
        }

        public static Ticket<Double> SumAwaitAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Double>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Sum.SumAwaitAsync(source, selector, cancellationToken);
        }

        public static Ticket<Double> SumAwaitWithCancellationAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Double>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Sum.SumAwaitWithCancellationAsync(source, selector, cancellationToken);
        }

        public static Ticket<Decimal> SumAsync(this ITicketAsyncEnumerable<Decimal> source, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return Sum.SumAsync(source, cancellationToken);
        }

        public static Ticket<Decimal> SumAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Decimal> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Sum.SumAsync(source, selector, cancellationToken);
        }

        public static Ticket<Decimal> SumAwaitAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Decimal>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Sum.SumAwaitAsync(source, selector, cancellationToken);
        }

        public static Ticket<Decimal> SumAwaitWithCancellationAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Decimal>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Sum.SumAwaitWithCancellationAsync(source, selector, cancellationToken);
        }

        public static Ticket<Int32?> SumAsync(this ITicketAsyncEnumerable<Int32?> source, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return Sum.SumAsync(source, cancellationToken);
        }

        public static Ticket<Int32?> SumAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Int32?> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Sum.SumAsync(source, selector, cancellationToken);
        }

        public static Ticket<Int32?> SumAwaitAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Int32?>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Sum.SumAwaitAsync(source, selector, cancellationToken);
        }

        public static Ticket<Int32?> SumAwaitWithCancellationAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Int32?>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Sum.SumAwaitWithCancellationAsync(source, selector, cancellationToken);
        }

        public static Ticket<Int64?> SumAsync(this ITicketAsyncEnumerable<Int64?> source, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return Sum.SumAsync(source, cancellationToken);
        }

        public static Ticket<Int64?> SumAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Int64?> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Sum.SumAsync(source, selector, cancellationToken);
        }

        public static Ticket<Int64?> SumAwaitAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Int64?>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Sum.SumAwaitAsync(source, selector, cancellationToken);
        }

        public static Ticket<Int64?> SumAwaitWithCancellationAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Int64?>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Sum.SumAwaitWithCancellationAsync(source, selector, cancellationToken);
        }

        public static Ticket<Single?> SumAsync(this ITicketAsyncEnumerable<Single?> source, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return Sum.SumAsync(source, cancellationToken);
        }

        public static Ticket<Single?> SumAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Single?> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Sum.SumAsync(source, selector, cancellationToken);
        }

        public static Ticket<Single?> SumAwaitAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Single?>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Sum.SumAwaitAsync(source, selector, cancellationToken);
        }

        public static Ticket<Single?> SumAwaitWithCancellationAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Single?>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Sum.SumAwaitWithCancellationAsync(source, selector, cancellationToken);
        }

        public static Ticket<Double?> SumAsync(this ITicketAsyncEnumerable<Double?> source, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return Sum.SumAsync(source, cancellationToken);
        }

        public static Ticket<Double?> SumAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Double?> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Sum.SumAsync(source, selector, cancellationToken);
        }

        public static Ticket<Double?> SumAwaitAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Double?>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Sum.SumAwaitAsync(source, selector, cancellationToken);
        }

        public static Ticket<Double?> SumAwaitWithCancellationAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Double?>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Sum.SumAwaitWithCancellationAsync(source, selector, cancellationToken);
        }

        public static Ticket<Decimal?> SumAsync(this ITicketAsyncEnumerable<Decimal?> source, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return Sum.SumAsync(source, cancellationToken);
        }

        public static Ticket<Decimal?> SumAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Decimal?> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Sum.SumAsync(source, selector, cancellationToken);
        }

        public static Ticket<Decimal?> SumAwaitAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Decimal?>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Sum.SumAwaitAsync(source, selector, cancellationToken);
        }

        public static Ticket<Decimal?> SumAwaitWithCancellationAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Decimal?>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Sum.SumAwaitWithCancellationAsync(source, selector, cancellationToken);
        }

    }

    internal static class Sum
    {
        public static async Ticket<Int32> SumAsync(ITicketAsyncEnumerable<Int32> source, CancellationToken cancellationToken)
        {
            Int32 sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += e.Current;
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Int32> SumAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Int32> selector, CancellationToken cancellationToken)
        {
            Int32 sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += selector(e.Current);
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Int32> SumAwaitAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Int32>> selector, CancellationToken cancellationToken)
        {
            Int32 sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += (await selector(e.Current));
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Int32> SumAwaitWithCancellationAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Int32>> selector, CancellationToken cancellationToken)
        {
            Int32 sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += (await selector(e.Current, cancellationToken));
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Int64> SumAsync(ITicketAsyncEnumerable<Int64> source, CancellationToken cancellationToken)
        {
            Int64 sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += e.Current;
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Int64> SumAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Int64> selector, CancellationToken cancellationToken)
        {
            Int64 sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += selector(e.Current);
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Int64> SumAwaitAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Int64>> selector, CancellationToken cancellationToken)
        {
            Int64 sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += (await selector(e.Current));
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Int64> SumAwaitWithCancellationAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Int64>> selector, CancellationToken cancellationToken)
        {
            Int64 sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += (await selector(e.Current, cancellationToken));
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Single> SumAsync(ITicketAsyncEnumerable<Single> source, CancellationToken cancellationToken)
        {
            Single sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += e.Current;
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Single> SumAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Single> selector, CancellationToken cancellationToken)
        {
            Single sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += selector(e.Current);
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Single> SumAwaitAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Single>> selector, CancellationToken cancellationToken)
        {
            Single sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += (await selector(e.Current));
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Single> SumAwaitWithCancellationAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Single>> selector, CancellationToken cancellationToken)
        {
            Single sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += (await selector(e.Current, cancellationToken));
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Double> SumAsync(ITicketAsyncEnumerable<Double> source, CancellationToken cancellationToken)
        {
            Double sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += e.Current;
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Double> SumAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Double> selector, CancellationToken cancellationToken)
        {
            Double sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += selector(e.Current);
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Double> SumAwaitAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Double>> selector, CancellationToken cancellationToken)
        {
            Double sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += (await selector(e.Current));
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Double> SumAwaitWithCancellationAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Double>> selector, CancellationToken cancellationToken)
        {
            Double sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += (await selector(e.Current, cancellationToken));
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Decimal> SumAsync(ITicketAsyncEnumerable<Decimal> source, CancellationToken cancellationToken)
        {
            Decimal sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += e.Current;
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Decimal> SumAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Decimal> selector, CancellationToken cancellationToken)
        {
            Decimal sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += selector(e.Current);
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Decimal> SumAwaitAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Decimal>> selector, CancellationToken cancellationToken)
        {
            Decimal sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += (await selector(e.Current));
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Decimal> SumAwaitWithCancellationAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Decimal>> selector, CancellationToken cancellationToken)
        {
            Decimal sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += (await selector(e.Current, cancellationToken));
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Int32?> SumAsync(ITicketAsyncEnumerable<Int32?> source, CancellationToken cancellationToken)
        {
            Int32? sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += e.Current.GetValueOrDefault();
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Int32?> SumAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Int32?> selector, CancellationToken cancellationToken)
        {
            Int32? sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += selector(e.Current).GetValueOrDefault();
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Int32?> SumAwaitAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Int32?>> selector, CancellationToken cancellationToken)
        {
            Int32? sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += (await selector(e.Current)).GetValueOrDefault();
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Int32?> SumAwaitWithCancellationAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Int32?>> selector, CancellationToken cancellationToken)
        {
            Int32? sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += (await selector(e.Current, cancellationToken)).GetValueOrDefault();
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Int64?> SumAsync(ITicketAsyncEnumerable<Int64?> source, CancellationToken cancellationToken)
        {
            Int64? sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += e.Current.GetValueOrDefault();
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Int64?> SumAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Int64?> selector, CancellationToken cancellationToken)
        {
            Int64? sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += selector(e.Current).GetValueOrDefault();
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Int64?> SumAwaitAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Int64?>> selector, CancellationToken cancellationToken)
        {
            Int64? sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += (await selector(e.Current)).GetValueOrDefault();
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Int64?> SumAwaitWithCancellationAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Int64?>> selector, CancellationToken cancellationToken)
        {
            Int64? sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += (await selector(e.Current, cancellationToken)).GetValueOrDefault();
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Single?> SumAsync(ITicketAsyncEnumerable<Single?> source, CancellationToken cancellationToken)
        {
            Single? sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += e.Current.GetValueOrDefault();
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Single?> SumAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Single?> selector, CancellationToken cancellationToken)
        {
            Single? sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += selector(e.Current).GetValueOrDefault();
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Single?> SumAwaitAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Single?>> selector, CancellationToken cancellationToken)
        {
            Single? sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += (await selector(e.Current)).GetValueOrDefault();
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Single?> SumAwaitWithCancellationAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Single?>> selector, CancellationToken cancellationToken)
        {
            Single? sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += (await selector(e.Current, cancellationToken)).GetValueOrDefault();
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Double?> SumAsync(ITicketAsyncEnumerable<Double?> source, CancellationToken cancellationToken)
        {
            Double? sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += e.Current.GetValueOrDefault();
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Double?> SumAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Double?> selector, CancellationToken cancellationToken)
        {
            Double? sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += selector(e.Current).GetValueOrDefault();
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Double?> SumAwaitAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Double?>> selector, CancellationToken cancellationToken)
        {
            Double? sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += (await selector(e.Current)).GetValueOrDefault();
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Double?> SumAwaitWithCancellationAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Double?>> selector, CancellationToken cancellationToken)
        {
            Double? sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += (await selector(e.Current, cancellationToken)).GetValueOrDefault();
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Decimal?> SumAsync(ITicketAsyncEnumerable<Decimal?> source, CancellationToken cancellationToken)
        {
            Decimal? sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += e.Current.GetValueOrDefault();
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Decimal?> SumAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Decimal?> selector, CancellationToken cancellationToken)
        {
            Decimal? sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += selector(e.Current).GetValueOrDefault();
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Decimal?> SumAwaitAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Decimal?>> selector, CancellationToken cancellationToken)
        {
            Decimal? sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += (await selector(e.Current)).GetValueOrDefault();
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

        public static async Ticket<Decimal?> SumAwaitWithCancellationAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Decimal?>> selector, CancellationToken cancellationToken)
        {
            Decimal? sum = default;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    sum += (await selector(e.Current, cancellationToken)).GetValueOrDefault();
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return sum;
        }

    }
}
