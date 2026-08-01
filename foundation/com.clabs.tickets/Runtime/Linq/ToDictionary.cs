using System;
using System.Collections.Generic;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static Ticket<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(keySelector, nameof(keySelector));

            return ToDictionary.ToDictionaryAsync(source, keySelector, EqualityComparer<TKey>.Default, cancellationToken);
        }

        public static Ticket<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(keySelector, nameof(keySelector));
            Error.ThrowArgumentNullException(comparer, nameof(comparer));

            return ToDictionary.ToDictionaryAsync(source, keySelector, comparer, cancellationToken);
        }

        public static Ticket<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(keySelector, nameof(keySelector));
            Error.ThrowArgumentNullException(elementSelector, nameof(elementSelector));

            return ToDictionary.ToDictionaryAsync(source, keySelector, elementSelector, EqualityComparer<TKey>.Default, cancellationToken);
        }

        public static Ticket<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(keySelector, nameof(keySelector));
            Error.ThrowArgumentNullException(elementSelector, nameof(elementSelector));
            Error.ThrowArgumentNullException(comparer, nameof(comparer));

            return ToDictionary.ToDictionaryAsync(source, keySelector, elementSelector, comparer, cancellationToken);
        }

        public static Ticket<Dictionary<TKey, TSource>> ToDictionaryAwaitAsync<TSource, TKey>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<TKey>> keySelector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(keySelector, nameof(keySelector));

            return ToDictionary.ToDictionaryAwaitAsync(source, keySelector, EqualityComparer<TKey>.Default, cancellationToken);
        }

        public static Ticket<Dictionary<TKey, TSource>> ToDictionaryAwaitAsync<TSource, TKey>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<TKey>> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(keySelector, nameof(keySelector));
            Error.ThrowArgumentNullException(comparer, nameof(comparer));

            return ToDictionary.ToDictionaryAwaitAsync(source, keySelector, comparer, cancellationToken);
        }

        public static Ticket<Dictionary<TKey, TElement>> ToDictionaryAwaitAsync<TSource, TKey, TElement>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<TKey>> keySelector, Func<TSource, Ticket<TElement>> elementSelector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(keySelector, nameof(keySelector));
            Error.ThrowArgumentNullException(elementSelector, nameof(elementSelector));

            return ToDictionary.ToDictionaryAwaitAsync(source, keySelector, elementSelector, EqualityComparer<TKey>.Default, cancellationToken);
        }

        public static Ticket<Dictionary<TKey, TElement>> ToDictionaryAwaitAsync<TSource, TKey, TElement>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<TKey>> keySelector, Func<TSource, Ticket<TElement>> elementSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(keySelector, nameof(keySelector));
            Error.ThrowArgumentNullException(elementSelector, nameof(elementSelector));
            Error.ThrowArgumentNullException(comparer, nameof(comparer));

            return ToDictionary.ToDictionaryAwaitAsync(source, keySelector, elementSelector, comparer, cancellationToken);
        }

        public static Ticket<Dictionary<TKey, TSource>> ToDictionaryAwaitWithCancellationAsync<TSource, TKey>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<TKey>> keySelector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(keySelector, nameof(keySelector));

            return ToDictionary.ToDictionaryAwaitWithCancellationAsync(source, keySelector, EqualityComparer<TKey>.Default, cancellationToken);
        }

        public static Ticket<Dictionary<TKey, TSource>> ToDictionaryAwaitWithCancellationAsync<TSource, TKey>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<TKey>> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(keySelector, nameof(keySelector));
            Error.ThrowArgumentNullException(comparer, nameof(comparer));

            return ToDictionary.ToDictionaryAwaitWithCancellationAsync(source, keySelector, comparer, cancellationToken);
        }

        public static Ticket<Dictionary<TKey, TElement>> ToDictionaryAwaitWithCancellationAsync<TSource, TKey, TElement>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<TKey>> keySelector, Func<TSource, CancellationToken, Ticket<TElement>> elementSelector, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(keySelector, nameof(keySelector));
            Error.ThrowArgumentNullException(elementSelector, nameof(elementSelector));

            return ToDictionary.ToDictionaryAwaitWithCancellationAsync(source, keySelector, elementSelector, EqualityComparer<TKey>.Default, cancellationToken);
        }

        public static Ticket<Dictionary<TKey, TElement>> ToDictionaryAwaitWithCancellationAsync<TSource, TKey, TElement>(this ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<TKey>> keySelector, Func<TSource, CancellationToken, Ticket<TElement>> elementSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(keySelector, nameof(keySelector));
            Error.ThrowArgumentNullException(elementSelector, nameof(elementSelector));
            Error.ThrowArgumentNullException(comparer, nameof(comparer));

            return ToDictionary.ToDictionaryAwaitWithCancellationAsync(source, keySelector, elementSelector, comparer, cancellationToken);
        }
    }

    internal static class ToDictionary
    {
        internal static async Ticket<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(ITicketAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
        {
            var dict = new Dictionary<TKey, TSource>(comparer);

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    var v = e.Current;
                    var key = keySelector(v);
                    dict.Add(key, v);
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return dict;
        }

        internal static async Ticket<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(ITicketAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
        {
            var dict = new Dictionary<TKey, TElement>(comparer);

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    var v = e.Current;
                    var key = keySelector(v);
                    var value = elementSelector(v);
                    dict.Add(key, value);
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return dict;
        }

        // with await

        internal static async Ticket<Dictionary<TKey, TSource>> ToDictionaryAwaitAsync<TSource, TKey>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<TKey>> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
        {
            var dict = new Dictionary<TKey, TSource>(comparer);

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    var v = e.Current;
                    var key = await keySelector(v);
                    dict.Add(key, v);
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return dict;
        }

        internal static async Ticket<Dictionary<TKey, TElement>> ToDictionaryAwaitAsync<TSource, TKey, TElement>(ITicketAsyncEnumerable<TSource> source, Func<TSource, Ticket<TKey>> keySelector, Func<TSource, Ticket<TElement>> elementSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
        {
            var dict = new Dictionary<TKey, TElement>(comparer);

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    var v = e.Current;
                    var key = await keySelector(v);
                    var value = await elementSelector(v);
                    dict.Add(key, value);
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return dict;
        }

        // with cancellation

        internal static async Ticket<Dictionary<TKey, TSource>> ToDictionaryAwaitWithCancellationAsync<TSource, TKey>(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<TKey>> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
        {
            var dict = new Dictionary<TKey, TSource>(comparer);

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    var v = e.Current;
                    var key = await keySelector(v, cancellationToken);
                    dict.Add(key, v);
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return dict;
        }

        internal static async Ticket<Dictionary<TKey, TElement>> ToDictionaryAwaitWithCancellationAsync<TSource, TKey, TElement>(ITicketAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, Ticket<TKey>> keySelector, Func<TSource, CancellationToken, Ticket<TElement>> elementSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
        {
            var dict = new Dictionary<TKey, TElement>(comparer);

            var e = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await e.MoveNextAsync())
                {
                    var v = e.Current;
                    var key = await keySelector(v, cancellationToken);
                    var value = await elementSelector(v, cancellationToken);
                    dict.Add(key, value);
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }

            return dict;
        }
    }
}