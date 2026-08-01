using CLabs.Tickets.Internal;
using System;
using System.Threading;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static ITicketAsyncEnumerable<TValue> Throw<TValue>(Exception exception)
        {
            return new Throw<TValue>(exception);
        }
    }

    internal sealed class Throw<TValue> : ITicketAsyncEnumerable<TValue>
    {
        readonly Exception exception;

        public Throw(Exception exception)
        {
            this.exception = exception;
        }

        public ITicketAsyncEnumerator<TValue> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _Throw(exception, cancellationToken);
        }

        class _Throw : ITicketAsyncEnumerator<TValue>
        {
            readonly Exception exception;
            CancellationToken cancellationToken;

            public _Throw(Exception exception, CancellationToken cancellationToken)
            {
                this.exception = exception;
                this.cancellationToken = cancellationToken;
            }

            public TValue Current => default;

            public Ticket<bool> MoveNextAsync()
            {
                cancellationToken.ThrowIfCancellationRequested();
                return Ticket.FromException<bool>(exception);
            }

            public Ticket DisposeAsync()
            {
                return default;
            }
        }
    }
}