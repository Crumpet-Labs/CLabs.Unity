using System;
using System.Collections.Generic;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static ITicketAsyncEnumerable<TSource> Except<TSource>(this ITicketAsyncEnumerable<TSource> first, ITicketAsyncEnumerable<TSource> second)
        {
            Error.ThrowArgumentNullException(first, nameof(first));
            Error.ThrowArgumentNullException(second, nameof(second));

            return new Except<TSource>(first, second, EqualityComparer<TSource>.Default);
        }

        public static ITicketAsyncEnumerable<TSource> Except<TSource>(this ITicketAsyncEnumerable<TSource> first, ITicketAsyncEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            Error.ThrowArgumentNullException(first, nameof(first));
            Error.ThrowArgumentNullException(second, nameof(second));
            Error.ThrowArgumentNullException(comparer, nameof(comparer));

            return new Except<TSource>(first, second, comparer);
        }
    }

    internal sealed class Except<TSource> : ITicketAsyncEnumerable<TSource>
    {
        readonly ITicketAsyncEnumerable<TSource> first;
        readonly ITicketAsyncEnumerable<TSource> second;
        readonly IEqualityComparer<TSource> comparer;

        public Except(ITicketAsyncEnumerable<TSource> first, ITicketAsyncEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            this.first = first;
            this.second = second;
            this.comparer = comparer;
        }

        public ITicketAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _Except(first, second, comparer, cancellationToken);
        }

        class _Except : AsyncEnumeratorBase<TSource, TSource>
        {
            static Action<object> HashSetAsyncCoreDelegate = HashSetAsyncCore;

            readonly IEqualityComparer<TSource> comparer;
            readonly ITicketAsyncEnumerable<TSource> second;

            HashSet<TSource> set;
            Ticket<HashSet<TSource>>.Awaiter awaiter;

            public _Except(ITicketAsyncEnumerable<TSource> first, ITicketAsyncEnumerable<TSource> second, IEqualityComparer<TSource> comparer, CancellationToken cancellationToken)

                : base(first, cancellationToken)
            {
                this.second = second;
                this.comparer = comparer;
            }

            protected override bool OnFirstIteration()
            {
                if (set != null) return false;

                awaiter = second.ToHashSetAsync(cancellationToken).GetAwaiter();
                if (awaiter.IsCompleted)
                {
                    set = awaiter.GetResult();
                    SourceMoveNext();
                }
                else
                {
                    awaiter.SourceOnCompleted(HashSetAsyncCoreDelegate, this);
                }

                return true;
            }

            static void HashSetAsyncCore(object state)
            {
                var self = (_Except)state;

                if (self.TryGetResult(self.awaiter, out var result))
                {
                    self.set = result;
                    self.SourceMoveNext();
                }
            }

            protected override bool TryMoveNextCore(bool sourceHasCurrent, out bool result)
            {
                if (sourceHasCurrent)
                {
                    var v = SourceCurrent;
                    if (set.Add(v))
                    {
                        Current = v;
                        result = true;
                        return true;
                    }
                    else
                    {
                        result = default;
                        return false;
                    }
                }

                result = false;
                return true;
            }
        }
    }
}