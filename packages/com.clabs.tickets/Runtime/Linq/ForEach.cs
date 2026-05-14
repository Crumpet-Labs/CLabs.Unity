using System;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static Ticket ForEachAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Action<TSource> action, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(action, nameof(action));

            return ForEach.ForEachAsync(source, action, cancellationToken);
        }

        public static Ticket ForEachAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Action<TSource, Int32> action, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(action, nameof(action));

            return ForEach.ForEachAsync(source, action, cancellationToken);
        }

        /// <summary>Obsolete(Error), Use Use ForEachAwaitAsync instead.</summary>
        [Obsolete("Use ForEachAwaitAsync instead.", true)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static Ticket ForEachAsync<T>(this ITicketAsyncEnumerable<T> source, Func<T, Ticket> action, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException("Use ForEachAwaitAsync instead.");
        }

        /// <summary>Obsolete(Error), Use Use ForEachAwaitAsync instead.</summary>
        [Obsolete("Use ForEachAwaitAsync instead.", true)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static Ticket ForEachAsync<T>(this ITicketAsyncEnumerable<T> source, Func<T, int, Ticket> action, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException("Use ForEachAwaitAsync instead.");
        }

        public static Ticket ForEachAwaitAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket> action, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(action, nameof(action));

            return ForEach.ForEachAwaitAsync(source, action, cancellationToken);
        }

        public static Ticket ForEachAwaitAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Int32, Ticket> action, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(action, nameof(action));

            return ForEach.ForEachAwaitAsync(source, action, cancellationToken);
        }

        public static Ticket ForEachAwaitWithCancellationAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket> action, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(action, nameof(action));

            return ForEach.ForEachAwaitWithCancellationAsync(source, action, cancellationToken);
        }

        public static Ticket ForEachAwaitWithCancellationAsync<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Int32, CancellationToken, Ticket> action, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(action, nameof(action));

            return ForEach.ForEachAwaitWithCancellationAsync(source, action, cancellationToken);
        }
    }

    internal static class ForEach
    {
        public static async Ticket ForEachAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Action<TSource> action, CancellationToken cancellationToken)
        {
            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    action(e.Current);
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }
        }

        public static async Ticket ForEachAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Action<TSource, Int32> action, CancellationToken cancellationToken)
        {
            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                int index = 0;
                while (await e.MoveNextAsync())
                {
                    action(e.Current, checked(index++));
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }
        }

        public static async Ticket ForEachAwaitAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket> action, CancellationToken cancellationToken)
        {
            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    await action(e.Current);
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }
        }

        public static async Ticket ForEachAwaitAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Int32, Ticket> action, CancellationToken cancellationToken)
        {
            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                int index = 0;
                while (await e.MoveNextAsync())
                {
                    await action(e.Current, checked(index++));
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }
        }

        public static async Ticket ForEachAwaitWithCancellationAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket> action, CancellationToken cancellationToken)
        {
            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    await action(e.Current, cancellationToken);
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }
        }

        public static async Ticket ForEachAwaitWithCancellationAsync<TSource>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Int32, CancellationToken, Ticket> action, CancellationToken cancellationToken)
        {
            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                int index = 0;
                while (await e.MoveNextAsync())
                {
                    await action(e.Current, checked(index++), cancellationToken);
                }
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