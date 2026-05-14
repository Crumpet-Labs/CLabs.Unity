using System.Threading;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static ITicketAsyncEnumerable<T> Empty<T>()
        {
            return Linq.Empty<T>.Instance;
        }
    }

    internal sealed class Empty<T> : ITicketAsyncEnumerable<T>
    {
        public static readonly ITicketAsyncEnumerable<T> Instance = new Empty<T>();

        Empty()
        {
        }

        public ITicketAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return _Empty.Instance;
        }

        class _Empty : ITicketAsyncEnumerator<T>
        {
            public static readonly ITicketAsyncEnumerator<T> Instance = new _Empty();

            _Empty()
            {
            }

            public T Current => default;

            public Ticket<bool> MoveNextAsync()
            {
                return CompletedTasks.False;
            }

            public Ticket DisposeAsync()
            {
                return default;
            }
        }
    }
}