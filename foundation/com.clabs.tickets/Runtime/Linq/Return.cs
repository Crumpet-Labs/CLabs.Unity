using CLabs.Tickets.Internal;
using System.Threading;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static ITicketAsyncEnumerable<TValue> Return<TValue>(TValue value)
        {
            return new Return<TValue>(value);
        }
    }

    internal sealed class Return<TValue> : ITicketAsyncEnumerable<TValue>
    {
        readonly TValue value;

        public Return(TValue value)
        {
            this.value = value;
        }

        public ITicketAsyncEnumerator<TValue> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _Return(value, cancellationToken);
        }

        class _Return : ITicketAsyncEnumerator<TValue>
        {
            readonly TValue value;
            CancellationToken cancellationToken;

            bool called;

            public _Return(TValue value, CancellationToken cancellationToken)
            {
                this.value = value;
                this.cancellationToken = cancellationToken;
                this.called = false;
            }

            public TValue Current => value;

            public Ticket<bool> MoveNextAsync()
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!called)
                {
                    called = true;
                    return CompletedTasks.True;
                }

                return CompletedTasks.False;
            }

            public Ticket DisposeAsync()
            {
                return default;
            }
        }
    }
}