using System;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static ITicketAsyncEnumerable<TSource> TakeUntil<TSource>(this ITicketAsyncEnumerable<TSource> source, Ticket other)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return new TakeUntil<TSource>(source, other, null);
        }

        public static ITicketAsyncEnumerable<TSource> TakeUntil<TSource>(this ITicketAsyncEnumerable<TSource> source, Func<CancellationToken, Ticket> other)
        {
            Error.ThrowArgumentNullException(source, nameof(source));
            Error.ThrowArgumentNullException(source, nameof(other));

            return new TakeUntil<TSource>(source, default, other);
        }
    }

    internal sealed class TakeUntil<TSource> : ITicketAsyncEnumerable<TSource>
    {
        readonly ITicketAsyncEnumerable<TSource> source;
        readonly Ticket other;
        readonly Func<CancellationToken, Ticket> other2;

        public TakeUntil(ITicketAsyncEnumerable<TSource> source, Ticket other, Func<CancellationToken, Ticket> other2)
        {
            this.source = source;
            this.other = other;
            this.other2 = other2;
        }

        public ITicketAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            if (other2 != null)
            {
                return new _TakeUntil(source, this.other2(cancellationToken), cancellationToken);
            }
            else
            {
                return new _TakeUntil(source, this.other, cancellationToken);
            }
        }

        sealed class _TakeUntil : MoveNextSource, ITicketAsyncEnumerator<TSource>
        {
            static readonly Action<object> CancelDelegate1 = OnCanceled1;
            static readonly Action<object> MoveNextCoreDelegate = MoveNextCore;

            readonly ITicketAsyncEnumerable<TSource> source;
            CancellationToken cancellationToken1;
            CancellationTokenRegistration cancellationTokenRegistration1;

            bool completed;
            Exception exception;
            ITicketAsyncEnumerator<TSource> enumerator;
            Ticket<bool>.Awaiter awaiter;

            public _TakeUntil(ITicketAsyncEnumerable<TSource> source, Ticket other, CancellationToken cancellationToken1)
            {
                this.source = source;
                this.cancellationToken1 = cancellationToken1;

                if (cancellationToken1.CanBeCanceled)
                {
                    this.cancellationTokenRegistration1 = cancellationToken1.RegisterWithoutCaptureExecutionContext(CancelDelegate1, this);
                }

                TaskTracker.TrackActiveTask(this, 3);

                RunOther(other).Forget();
            }

            public TSource Current { get; private set; }

            public Ticket<bool> MoveNextAsync()
            {
                if (completed)
                {
                    return CompletedTasks.False;
                }

                if (exception != null)
                {
                    return Ticket.FromException<bool>(exception);
                }

                if (cancellationToken1.IsCancellationRequested)
                {
                    return Ticket.FromCanceled<bool>(cancellationToken1);
                }

                if (enumerator == null)
                {
                    enumerator = source.GetAsyncEnumerator(cancellationToken1);
                }

                completionSource.Reset();
                SourceMoveNext();
                return new Ticket<bool>(this, completionSource.Version);
            }

            void SourceMoveNext()
            {
                try
                {
                    awaiter = enumerator.MoveNextAsync().GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        MoveNextCore(this);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(MoveNextCoreDelegate, this);
                    }
                }
                catch (Exception ex)
                {
                    completionSource.TrySetException(ex);
                }
            }

            static void MoveNextCore(object state)
            {
                var self = (_TakeUntil)state;

                if (self.TryGetResult(self.awaiter, out var result))
                {
                    if (result)
                    {
                        if (self.exception != null)
                        {
                            self.completionSource.TrySetException(self.exception);
                        }
                        else if (self.cancellationToken1.IsCancellationRequested)
                        {
                            self.completionSource.TrySetCanceled(self.cancellationToken1);
                        }
                        else
                        {
                            self.Current = self.enumerator.Current;
                            self.completionSource.TrySetResult(true);
                        }
                    }
                    else
                    {
                        self.completionSource.TrySetResult(false);
                    }
                }
            }

            async TicketVoid RunOther(Ticket other)
            {
                try
                {
                    await other;
                    completed = true;
                    completionSource.TrySetResult(false);
                }
                catch (Exception ex)
                {
                    exception = ex;
                    completionSource.TrySetException(ex);
                }
            }

            static void OnCanceled1(object state)
            {
                var self = (_TakeUntil)state;
                self.completionSource.TrySetCanceled(self.cancellationToken1);
            }

            public Ticket DisposeAsync()
            {
                TaskTracker.RemoveTracking(this);
                cancellationTokenRegistration1.Dispose();
                if (enumerator != null)
                {
                    return enumerator.DisposeAsync();
                }
                return default;
            }
        }
    }
}