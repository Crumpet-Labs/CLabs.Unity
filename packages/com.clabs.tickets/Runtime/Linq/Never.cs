using System.Threading;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static ITicketAsyncEnumerable<T> Never<T>()
        {
            return Linq.Never<T>.Instance;
        }
    }

    internal sealed class Never<T> : ITicketAsyncEnumerable<T>
    {
        public static readonly ITicketAsyncEnumerable<T> Instance = new Never<T>();

        Never()
        {
        }

        public ITicketAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _Never(cancellationToken);
        }

        class _Never : ITicketAsyncEnumerator<T>
        {
            CancellationToken cancellationToken;

            public _Never(CancellationToken cancellationToken)
            {
                this.cancellationToken = cancellationToken;
            }

            public T Current => default;

            public Ticket<bool> MoveNextAsync()
            {
                var tcs = new TicketCompletionSource<bool>();

                cancellationToken.Register(state =>
                {
                    var task = (TicketCompletionSource<bool>)state;
                    task.TrySetCanceled(cancellationToken);
                }, tcs);

                return tcs.Task;
            }

            public Ticket DisposeAsync()
            {
                return default;
            }
        }
    }
}