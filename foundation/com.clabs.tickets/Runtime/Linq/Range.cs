using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static ITicketAsyncEnumerable<int> Range(int start, int count)
        {
            if (count < 0) throw Error.ArgumentOutOfRange(nameof(count));

            var end = (long)start + count - 1L;
            if (end > int.MaxValue) throw Error.ArgumentOutOfRange(nameof(count));

            if (count == 0) TicketAsyncEnumerable.Empty<int>();

            return new Range(start, count);
        }
    }

    internal sealed class Range : ITicketAsyncEnumerable<int>
    {
        readonly int start;
        readonly int end;

        public Range(int start, int count)
        {
            this.start = start;
            this.end = start + count;
        }

        public ITicketAsyncEnumerator<int> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _Range(start, end, cancellationToken);
        }

        class _Range : ITicketAsyncEnumerator<int>
        {
            readonly int start;
            readonly int end;
            int current;
            CancellationToken cancellationToken;

            public _Range(int start, int end, CancellationToken cancellationToken)
            {
                this.start = start;
                this.end = end;
                this.cancellationToken = cancellationToken;

                this.current = start - 1;
            }

            public int Current => current;

            public Ticket<bool> MoveNextAsync()
            {
                cancellationToken.ThrowIfCancellationRequested();

                current++;

                if (current != end)
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