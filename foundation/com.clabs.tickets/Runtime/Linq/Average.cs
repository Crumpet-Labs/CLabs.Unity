using System;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static Ticket<double> AverageAsync(this ITicketAsyncEnumerable<Int32> source, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return Average.AverageAsync(source, cancellationToken);
        }

        public static Ticket<double> AverageAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Int32> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Average.AverageAsync(source, selector, cancellationToken);
        }

        public static Ticket<double> AverageAwaitAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Int32>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Average.AverageAwaitAsync(source, selector, cancellationToken);
        }

        public static Ticket<double> AverageAwaitWithCancellationAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Int32>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Average.AverageAwaitWithCancellationAsync(source, selector, cancellationToken);
        }

        public static Ticket<double> AverageAsync(this ITicketAsyncEnumerable<Int64> source, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return Average.AverageAsync(source, cancellationToken);
        }

        public static Ticket<double> AverageAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Int64> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Average.AverageAsync(source, selector, cancellationToken);
        }

        public static Ticket<double> AverageAwaitAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Int64>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Average.AverageAwaitAsync(source, selector, cancellationToken);
        }

        public static Ticket<double> AverageAwaitWithCancellationAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Int64>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Average.AverageAwaitWithCancellationAsync(source, selector, cancellationToken);
        }

        public static Ticket<float> AverageAsync(this ITicketAsyncEnumerable<Single> source, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return Average.AverageAsync(source, cancellationToken);
        }

        public static Ticket<float> AverageAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Single> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Average.AverageAsync(source, selector, cancellationToken);
        }

        public static Ticket<float> AverageAwaitAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Single>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Average.AverageAwaitAsync(source, selector, cancellationToken);
        }

        public static Ticket<float> AverageAwaitWithCancellationAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Single>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Average.AverageAwaitWithCancellationAsync(source, selector, cancellationToken);
        }

        public static Ticket<double> AverageAsync(this ITicketAsyncEnumerable<Double> source, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return Average.AverageAsync(source, cancellationToken);
        }

        public static Ticket<double> AverageAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Double> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Average.AverageAsync(source, selector, cancellationToken);
        }

        public static Ticket<double> AverageAwaitAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Double>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Average.AverageAwaitAsync(source, selector, cancellationToken);
        }

        public static Ticket<double> AverageAwaitWithCancellationAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Double>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Average.AverageAwaitWithCancellationAsync(source, selector, cancellationToken);
        }

        public static Ticket<decimal> AverageAsync(this ITicketAsyncEnumerable<Decimal> source, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return Average.AverageAsync(source, cancellationToken);
        }

        public static Ticket<decimal> AverageAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Decimal> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Average.AverageAsync(source, selector, cancellationToken);
        }

        public static Ticket<decimal> AverageAwaitAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Decimal>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Average.AverageAwaitAsync(source, selector, cancellationToken);
        }

        public static Ticket<decimal> AverageAwaitWithCancellationAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Decimal>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Average.AverageAwaitWithCancellationAsync(source, selector, cancellationToken);
        }

        public static Ticket<double?> AverageAsync(this ITicketAsyncEnumerable<Int32?> source, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return Average.AverageAsync(source, cancellationToken);
        }

        public static Ticket<double?> AverageAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Int32?> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Average.AverageAsync(source, selector, cancellationToken);
        }

        public static Ticket<double?> AverageAwaitAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Int32?>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Average.AverageAwaitAsync(source, selector, cancellationToken);
        }

        public static Ticket<double?> AverageAwaitWithCancellationAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Int32?>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Average.AverageAwaitWithCancellationAsync(source, selector, cancellationToken);
        }

        public static Ticket<double?> AverageAsync(this ITicketAsyncEnumerable<Int64?> source, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return Average.AverageAsync(source, cancellationToken);
        }

        public static Ticket<double?> AverageAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Int64?> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Average.AverageAsync(source, selector, cancellationToken);
        }

        public static Ticket<double?> AverageAwaitAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Int64?>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Average.AverageAwaitAsync(source, selector, cancellationToken);
        }

        public static Ticket<double?> AverageAwaitWithCancellationAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Int64?>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Average.AverageAwaitWithCancellationAsync(source, selector, cancellationToken);
        }

        public static Ticket<float?> AverageAsync(this ITicketAsyncEnumerable<Single?> source, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return Average.AverageAsync(source, cancellationToken);
        }

        public static Ticket<float?> AverageAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Single?> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Average.AverageAsync(source, selector, cancellationToken);
        }

        public static Ticket<float?> AverageAwaitAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Single?>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Average.AverageAwaitAsync(source, selector, cancellationToken);
        }

        public static Ticket<float?> AverageAwaitWithCancellationAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Single?>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Average.AverageAwaitWithCancellationAsync(source, selector, cancellationToken);
        }

        public static Ticket<double?> AverageAsync(this ITicketAsyncEnumerable<Double?> source, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return Average.AverageAsync(source, cancellationToken);
        }

        public static Ticket<double?> AverageAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Double?> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Average.AverageAsync(source, selector, cancellationToken);
        }

        public static Ticket<double?> AverageAwaitAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Double?>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Average.AverageAwaitAsync(source, selector, cancellationToken);
        }

        public static Ticket<double?> AverageAwaitWithCancellationAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Double?>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Average.AverageAwaitWithCancellationAsync(source, selector, cancellationToken);
        }

        public static Ticket<decimal?> AverageAsync(this ITicketAsyncEnumerable<Decimal?> source, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return Average.AverageAsync(source, cancellationToken);
        }

        public static Ticket<decimal?> AverageAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Decimal?> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Average.AverageAsync(source, selector, cancellationToken);
        }

        public static Ticket<decimal?> AverageAwaitAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Decimal?>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Average.AverageAwaitAsync(source, selector, cancellationToken);
        }

        public static Ticket<decimal?> AverageAwaitWithCancellationAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Decimal?>> selector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(selector));

            return Average.AverageAwaitWithCancellationAsync(source, selector, cancellationToken);
        }

    }

    internal static class Average
    {
        public static async Ticket<double> AverageAsync(ITicketAsyncEnumerable<Int32> source, CancellationToken cancellationToken)
        {
            long count = 0;
            Int32 sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    checked
                    {
                        sum += e.Current;
                        count++;
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

            return (double)sum / count;
        }

        public static async Ticket<double> AverageAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Int32> selector, CancellationToken cancellationToken)
        {
            long count = 0;
            Int32 sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    checked
                    {
                        sum += selector(e.Current);
                        count++;
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

            return (double)sum / count;
        }

        public static async Ticket<double> AverageAwaitAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Int32>> selector, CancellationToken cancellationToken)
        {
            long count = 0;
            Int32 sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    checked
                    {
                        sum += await selector(e.Current);
                        count++;
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

            return (double)sum / count;
        }

        public static async Ticket<double> AverageAwaitWithCancellationAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Int32>> selector, CancellationToken cancellationToken)
        {
            long count = 0;
            Int32 sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    checked
                    {
                        sum += await selector(e.Current, cancellationToken);
                        count++;
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

            return (double)sum / count;
        }

        public static async Ticket<double> AverageAsync(ITicketAsyncEnumerable<Int64> source, CancellationToken cancellationToken)
        {
            long count = 0;
            Int64 sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    checked
                    {
                        sum += e.Current;
                        count++;
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

            return (double)sum / count;
        }

        public static async Ticket<double> AverageAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Int64> selector, CancellationToken cancellationToken)
        {
            long count = 0;
            Int64 sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    checked
                    {
                        sum += selector(e.Current);
                        count++;
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

            return (double)sum / count;
        }

        public static async Ticket<double> AverageAwaitAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Int64>> selector, CancellationToken cancellationToken)
        {
            long count = 0;
            Int64 sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    checked
                    {
                        sum += await selector(e.Current);
                        count++;
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

            return (double)sum / count;
        }

        public static async Ticket<double> AverageAwaitWithCancellationAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Int64>> selector, CancellationToken cancellationToken)
        {
            long count = 0;
            Int64 sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    checked
                    {
                        sum += await selector(e.Current, cancellationToken);
                        count++;
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

            return (double)sum / count;
        }

        public static async Ticket<float> AverageAsync(ITicketAsyncEnumerable<Single> source, CancellationToken cancellationToken)
        {
            long count = 0;
            Single sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    checked
                    {
                        sum += e.Current;
                        count++;
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

            return (float)(sum / count);
        }

        public static async Ticket<float> AverageAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Single> selector, CancellationToken cancellationToken)
        {
            long count = 0;
            Single sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    checked
                    {
                        sum += selector(e.Current);
                        count++;
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

            return (float)(sum / count);
        }

        public static async Ticket<float> AverageAwaitAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Single>> selector, CancellationToken cancellationToken)
        {
            long count = 0;
            Single sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    checked
                    {
                        sum += await selector(e.Current);
                        count++;
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

            return (float)(sum / count);
        }

        public static async Ticket<float> AverageAwaitWithCancellationAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Single>> selector, CancellationToken cancellationToken)
        {
            long count = 0;
            Single sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    checked
                    {
                        sum += await selector(e.Current, cancellationToken);
                        count++;
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

            return (float)(sum / count);
        }

        public static async Ticket<double> AverageAsync(ITicketAsyncEnumerable<Double> source, CancellationToken cancellationToken)
        {
            long count = 0;
            Double sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    checked
                    {
                        sum += e.Current;
                        count++;
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

            return sum / count;
        }

        public static async Ticket<double> AverageAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Double> selector, CancellationToken cancellationToken)
        {
            long count = 0;
            Double sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    checked
                    {
                        sum += selector(e.Current);
                        count++;
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

            return sum / count;
        }

        public static async Ticket<double> AverageAwaitAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Double>> selector, CancellationToken cancellationToken)
        {
            long count = 0;
            Double sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    checked
                    {
                        sum += await selector(e.Current);
                        count++;
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

            return sum / count;
        }

        public static async Ticket<double> AverageAwaitWithCancellationAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Double>> selector, CancellationToken cancellationToken)
        {
            long count = 0;
            Double sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    checked
                    {
                        sum += await selector(e.Current, cancellationToken);
                        count++;
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

            return sum / count;
        }

        public static async Ticket<decimal> AverageAsync(ITicketAsyncEnumerable<Decimal> source, CancellationToken cancellationToken)
        {
            long count = 0;
            Decimal sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    checked
                    {
                        sum += e.Current;
                        count++;
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

            return sum / count;
        }

        public static async Ticket<decimal> AverageAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Decimal> selector, CancellationToken cancellationToken)
        {
            long count = 0;
            Decimal sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    checked
                    {
                        sum += selector(e.Current);
                        count++;
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

            return sum / count;
        }

        public static async Ticket<decimal> AverageAwaitAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Decimal>> selector, CancellationToken cancellationToken)
        {
            long count = 0;
            Decimal sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    checked
                    {
                        sum += await selector(e.Current);
                        count++;
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

            return sum / count;
        }

        public static async Ticket<decimal> AverageAwaitWithCancellationAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Decimal>> selector, CancellationToken cancellationToken)
        {
            long count = 0;
            Decimal sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    checked
                    {
                        sum += await selector(e.Current, cancellationToken);
                        count++;
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

            return sum / count;
        }

        public static async Ticket<double?> AverageAsync(ITicketAsyncEnumerable<Int32?> source, CancellationToken cancellationToken)
        {
            long count = 0;
            Int32? sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    var v = e.Current;
                    if (v.HasValue)
                    {
                        checked    
                        {
                            sum += v.Value;
                            count++;
                        }
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

            return (double)sum / count;
        }

        public static async Ticket<double?> AverageAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Int32?> selector, CancellationToken cancellationToken)
        {
            long count = 0;
            Int32? sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    var v = selector(e.Current);
                    if (v.HasValue)
                    {
                        checked    
                        {
                            sum += v.Value;
                            count++;
                        }
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

            return (double)sum / count;
        }

        public static async Ticket<double?> AverageAwaitAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Int32?>> selector, CancellationToken cancellationToken)
        {
            long count = 0;
            Int32? sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    var v = await selector(e.Current);
                    if (v.HasValue)
                    {
                        checked    
                        {
                            sum += v.Value;
                            count++;
                        }
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

            return (double)sum / count;
        }

        public static async Ticket<double?> AverageAwaitWithCancellationAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Int32?>> selector, CancellationToken cancellationToken)
        {
            long count = 0;
            Int32? sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    var v = await selector(e.Current, cancellationToken);
                    if (v.HasValue)
                    {
                        checked    
                        {
                            sum += v.Value;
                            count++;
                        }
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

            return (double)sum / count;
        }

        public static async Ticket<double?> AverageAsync(ITicketAsyncEnumerable<Int64?> source, CancellationToken cancellationToken)
        {
            long count = 0;
            Int64? sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    var v = e.Current;
                    if (v.HasValue)
                    {
                        checked    
                        {
                            sum += v.Value;
                            count++;
                        }
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

            return (double)sum / count;
        }

        public static async Ticket<double?> AverageAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Int64?> selector, CancellationToken cancellationToken)
        {
            long count = 0;
            Int64? sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    var v = selector(e.Current);
                    if (v.HasValue)
                    {
                        checked    
                        {
                            sum += v.Value;
                            count++;
                        }
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

            return (double)sum / count;
        }

        public static async Ticket<double?> AverageAwaitAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Int64?>> selector, CancellationToken cancellationToken)
        {
            long count = 0;
            Int64? sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    var v = await selector(e.Current);
                    if (v.HasValue)
                    {
                        checked    
                        {
                            sum += v.Value;
                            count++;
                        }
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

            return (double)sum / count;
        }

        public static async Ticket<double?> AverageAwaitWithCancellationAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Int64?>> selector, CancellationToken cancellationToken)
        {
            long count = 0;
            Int64? sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    var v = await selector(e.Current, cancellationToken);
                    if (v.HasValue)
                    {
                        checked    
                        {
                            sum += v.Value;
                            count++;
                        }
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

            return (double)sum / count;
        }

        public static async Ticket<float?> AverageAsync(ITicketAsyncEnumerable<Single?> source, CancellationToken cancellationToken)
        {
            long count = 0;
            Single? sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    var v = e.Current;
                    if (v.HasValue)
                    {
                        checked    
                        {
                            sum += v.Value;
                            count++;
                        }
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

            return (float)(sum / count);
        }

        public static async Ticket<float?> AverageAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Single?> selector, CancellationToken cancellationToken)
        {
            long count = 0;
            Single? sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    var v = selector(e.Current);
                    if (v.HasValue)
                    {
                        checked    
                        {
                            sum += v.Value;
                            count++;
                        }
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

            return (float)(sum / count);
        }

        public static async Ticket<float?> AverageAwaitAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Single?>> selector, CancellationToken cancellationToken)
        {
            long count = 0;
            Single? sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    var v = await selector(e.Current);
                    if (v.HasValue)
                    {
                        checked    
                        {
                            sum += v.Value;
                            count++;
                        }
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

            return (float)(sum / count);
        }

        public static async Ticket<float?> AverageAwaitWithCancellationAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Single?>> selector, CancellationToken cancellationToken)
        {
            long count = 0;
            Single? sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    var v = await selector(e.Current, cancellationToken);
                    if (v.HasValue)
                    {
                        checked    
                        {
                            sum += v.Value;
                            count++;
                        }
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

            return (float)(sum / count);
        }

        public static async Ticket<double?> AverageAsync(ITicketAsyncEnumerable<Double?> source, CancellationToken cancellationToken)
        {
            long count = 0;
            Double? sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    var v = e.Current;
                    if (v.HasValue)
                    {
                        checked    
                        {
                            sum += v.Value;
                            count++;
                        }
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

            return sum / count;
        }

        public static async Ticket<double?> AverageAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Double?> selector, CancellationToken cancellationToken)
        {
            long count = 0;
            Double? sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    var v = selector(e.Current);
                    if (v.HasValue)
                    {
                        checked    
                        {
                            sum += v.Value;
                            count++;
                        }
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

            return sum / count;
        }

        public static async Ticket<double?> AverageAwaitAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Double?>> selector, CancellationToken cancellationToken)
        {
            long count = 0;
            Double? sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    var v = await selector(e.Current);
                    if (v.HasValue)
                    {
                        checked    
                        {
                            sum += v.Value;
                            count++;
                        }
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

            return sum / count;
        }

        public static async Ticket<double?> AverageAwaitWithCancellationAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Double?>> selector, CancellationToken cancellationToken)
        {
            long count = 0;
            Double? sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    var v = await selector(e.Current, cancellationToken);
                    if (v.HasValue)
                    {
                        checked    
                        {
                            sum += v.Value;
                            count++;
                        }
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

            return sum / count;
        }

        public static async Ticket<decimal?> AverageAsync(ITicketAsyncEnumerable<Decimal?> source, CancellationToken cancellationToken)
        {
            long count = 0;
            Decimal? sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    var v = e.Current;
                    if (v.HasValue)
                    {
                        checked    
                        {
                            sum += v.Value;
                            count++;
                        }
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

            return sum / count;
        }

        public static async Ticket<decimal?> AverageAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Decimal?> selector, CancellationToken cancellationToken)
        {
            long count = 0;
            Decimal? sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    var v = selector(e.Current);
                    if (v.HasValue)
                    {
                        checked    
                        {
                            sum += v.Value;
                            count++;
                        }
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

            return sum / count;
        }

        public static async Ticket<decimal?> AverageAwaitAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<Decimal?>> selector, CancellationToken cancellationToken)
        {
            long count = 0;
            Decimal? sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    var v = await selector(e.Current);
                    if (v.HasValue)
                    {
                        checked    
                        {
                            sum += v.Value;
                            count++;
                        }
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

            return sum / count;
        }

        public static async Ticket<decimal?> AverageAwaitWithCancellationAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<Decimal?>> selector, CancellationToken cancellationToken)
        {
            long count = 0;
            Decimal? sum = 0;

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    var v = await selector(e.Current, cancellationToken);
                    if (v.HasValue)
                    {
                        checked    
                        {
                            sum += v.Value;
                            count++;
                        }
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

            return sum / count;
        }

    }
}
