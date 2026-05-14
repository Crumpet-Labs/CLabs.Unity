using System.Threading;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static ITicketAsyncEnumerable<AsyncUnit> EveryUpdate(PlayerLoopTiming updateTiming = PlayerLoopTiming.Update, bool cancelImmediately = false)
        {
            return new EveryUpdate(updateTiming, cancelImmediately);
        }
    }

    internal sealed class EveryUpdate : ITicketAsyncEnumerable<AsyncUnit>
    {
        readonly PlayerLoopTiming updateTiming;
        readonly bool cancelImmediately;

        public EveryUpdate(PlayerLoopTiming updateTiming, bool cancelImmediately)
        {
            this.updateTiming = updateTiming;
            this.cancelImmediately = cancelImmediately;
        }

        public ITicketAsyncEnumerator<AsyncUnit> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _EveryUpdate(updateTiming, cancellationToken, cancelImmediately);
        }

        class _EveryUpdate : MoveNextSource, ITicketAsyncEnumerator<AsyncUnit>, IPlayerLoopItem
        {
            readonly PlayerLoopTiming updateTiming;
            readonly CancellationToken cancellationToken;
            readonly CancellationTokenRegistration cancellationTokenRegistration;

            bool disposed;

            public _EveryUpdate(PlayerLoopTiming updateTiming, CancellationToken cancellationToken, bool cancelImmediately)
            {
                this.updateTiming = updateTiming;
                this.cancellationToken = cancellationToken;

                if (cancelImmediately && cancellationToken.CanBeCanceled)
                {
                    cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(state =>
                    {
                        var source = (_EveryUpdate)state;
                        source.completionSource.TrySetCanceled(source.cancellationToken);
                    }, this);
                }

                TaskTracker.TrackActiveTask(this, 2);
                TicketRuntime.AddAction(updateTiming, this);
            }

            public AsyncUnit Current => default;

            public Ticket<bool> MoveNextAsync()
            {
                if (disposed) return CompletedTasks.False;
                
                completionSource.Reset();

                if (cancellationToken.IsCancellationRequested)
                {
                    completionSource.TrySetCanceled(cancellationToken);
                }
                return new Ticket<bool>(this, completionSource.Version);
            }

            public Ticket DisposeAsync()
            {
                if (!disposed)
                {
                    cancellationTokenRegistration.Dispose();
                    disposed = true;
                    TaskTracker.RemoveTracking(this);
                }
                return default;
            }

            public bool MoveNext()
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    completionSource.TrySetCanceled(cancellationToken);
                    return false;
                }
                
                if (disposed)
                {
                    completionSource.TrySetResult(false);
                    return false;
                }

                completionSource.TrySetResult(true);
                return true;
            }
        }
    }
}