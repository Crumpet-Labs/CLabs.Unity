using System;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static ITicketAsyncEnumerable<TResult> Cast<TResult>(this ITicketAsyncEnumerable<Object> source)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return new Cast<TResult>(source);
        }
    }

    internal sealed class Cast<TResult> : ITicketAsyncEnumerable<TResult>
    {
        readonly ITicketAsyncEnumerable<object> source;

        public Cast(ITicketAsyncEnumerable<object> source)
        {
            this.source = source;
        }

        public ITicketAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _Cast(source, cancellationToken);
        }

        class _Cast : AsyncEnumeratorBase<object, TResult>
        {
            public _Cast(ITicketAsyncEnumerable<object> source, CancellationToken cancellationToken)

                : base(source, cancellationToken)
            {
            }

            protected override bool TryMoveNextCore(bool sourceHasCurrent, out bool result)
            {
                if (sourceHasCurrent)
                {
                    Current = (TResult)SourceCurrent;
                    result = true;
                    return true;
                }

                result = false;
                return true;
            }
        }
    }
}