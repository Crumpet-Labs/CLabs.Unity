using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static ITicketAsyncEnumerable<TElement> Repeat<TElement>(TElement element, int count)
        {
            if (count < 0) throw Error.ArgumentOutOfRange(nameof(count));

            return new Repeat<TElement>(element, count);
        }
    }

    internal sealed class Repeat<TElement> : ITicketAsyncEnumerable<TElement>
    {
        readonly TElement element;
        readonly int count;

        public Repeat(TElement element, int count)
        {
            this.element = element;
            this.count = count;
        }

        public ITicketAsyncEnumerator<TElement> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _Repeat(element, count, cancellationToken);
        }

        class _Repeat : ITicketAsyncEnumerator<TElement>
        {
            readonly TElement element;
            readonly int count;
            int remaining;
            CancellationToken cancellationToken;

            public _Repeat(TElement element, int count, CancellationToken cancellationToken)
            {
                this.element = element;
                this.count = count;
                this.cancellationToken = cancellationToken;

                this.remaining = count;
            }

            public TElement Current => element;

            public Ticket<bool> MoveNextAsync()
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (remaining-- != 0)
                {
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