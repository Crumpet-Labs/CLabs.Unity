using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static ITicketAsyncEnumerable<TSource> Reverse<TSource>(this ITicketAsyncEnumerable<TSource> source)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            return new Reverse<TSource>(source);
        }
    }

    internal sealed class Reverse<TSource> : ITicketAsyncEnumerable<TSource>
    {
        readonly ITicketAsyncEnumerable<TSource> source;

        public Reverse(ITicketAsyncEnumerable<TSource> source)
        {
            this.source = source;
        }

        public ITicketAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _Reverse(source, cancellationToken);
        }

        sealed class _Reverse : MoveNextSource, ITicketAsyncEnumerator<TSource>
        {
            readonly ITicketAsyncEnumerable<TSource> source;
            CancellationToken cancellationToken;

            TSource[] array;
            int index;

            public _Reverse(ITicketAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
            {
                this.source = source;
                this.cancellationToken = cancellationToken;
                TaskTracker.TrackActiveTask(this, 3);
            }

            public TSource Current { get; private set; }

            // after consumed array, don't use await so allow async(not require TicketCompletionSourceCore).
            public async Ticket<bool> MoveNextAsync()
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (array == null)
                {
                    array = await source.ToArrayAsync(cancellationToken);
                    index = array.Length - 1;
                }

                if (index != -1)
                {
                    Current = array[index];
                    --index;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public Ticket DisposeAsync()
            {
                TaskTracker.RemoveTracking(this);
                return default;
            }
        }
    }
}