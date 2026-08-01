using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace CLabs.Tickets
{
    public interface ITicketAsyncEnumerable<out T>
    {
        ITicketAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default);
    }

    public interface ITicketAsyncEnumerator<out T> : ITicketAsyncDisposable
    {
        T Current { get; }
        Ticket<bool> MoveNextAsync();
    }

    public interface ITicketAsyncDisposable
    {
        Ticket DisposeAsync();
    }

    public interface ITicketOrderedAsyncEnumerable<TElement> : ITicketAsyncEnumerable<TElement>
    {
        ITicketOrderedAsyncEnumerable<TElement> CreateOrderedEnumerable<TKey>(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending);
        ITicketOrderedAsyncEnumerable<TElement> CreateOrderedEnumerable<TKey>(Func<TElement, Ticket<TKey>> keySelector, IComparer<TKey> comparer, bool descending);
        ITicketOrderedAsyncEnumerable<TElement> CreateOrderedEnumerable<TKey>(Func<TElement, CancellationToken, Ticket<TKey>> keySelector, IComparer<TKey> comparer, bool descending);
    }

    public interface IConnectableTicketAsyncEnumerable<out T> : ITicketAsyncEnumerable<T>
    {
        IDisposable Connect();
    }

    // don't use AsyncGrouping.
    //public interface ITicketAsyncGrouping<out TKey, out TElement> : ITicketAsyncEnumerable<TElement>
    //{
    //    TKey Key { get; }
    //}

    public static class TicketAsyncEnumerableExtensions
    {
        public static TicketCancelableAsyncEnumerable<T> WithCancellation<T>(this ITicketAsyncEnumerable<T> source, CancellationToken cancellationToken)
        {
            return new TicketCancelableAsyncEnumerable<T>(source, cancellationToken);
        }
    }

    [StructLayout(LayoutKind.Auto)]
    public readonly struct TicketCancelableAsyncEnumerable<T>
    {
        private readonly ITicketAsyncEnumerable<T> enumerable;
        private readonly CancellationToken cancellationToken;

        internal TicketCancelableAsyncEnumerable(ITicketAsyncEnumerable<T> enumerable, CancellationToken cancellationToken)
        {
            this.enumerable = enumerable;
            this.cancellationToken = cancellationToken;
        }

        public Enumerator GetAsyncEnumerator()
        {
            return new Enumerator(enumerable.GetAsyncEnumerator(cancellationToken));
        }

        [StructLayout(LayoutKind.Auto)]
        public readonly struct Enumerator
        {
            private readonly ITicketAsyncEnumerator<T> enumerator;

            internal Enumerator(ITicketAsyncEnumerator<T> enumerator)
            {
                this.enumerator = enumerator;
            }

            public T Current => enumerator.Current;

            public Ticket<bool> MoveNextAsync()
            {
                return enumerator.MoveNextAsync();
            }


            public Ticket DisposeAsync()
            {
                return enumerator.DisposeAsync();
            }
        }
    }
}