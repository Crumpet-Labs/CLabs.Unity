using System;
using System.Collections.Generic;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        // Dispatches to one of two enumerable implementations based on whether
        // an engine-object detector is registered (Unity/Godot adapters
        // register one) AND the target is recognized as an engine-tracked
        // object. Engine path holds the target by strong reference and
        // detects destruction through the engine detector, at zero allocation.
        // Fallback path uses a WeakReference wrapper for pure-C# targets.
        public static ITicketAsyncEnumerable<TProperty> EveryValueChanged<TTarget, TProperty>(TTarget target, Func<TTarget, TProperty> propertySelector, PlayerLoopTiming monitorTiming = PlayerLoopTiming.Update, IEqualityComparer<TProperty> equalityComparer = null, bool cancelImmediately = false)
            where TTarget : class
        {
            var cmp = equalityComparer ?? TicketEqualityComparer.GetDefault<TProperty>();
            if (TicketRuntime.HasEngineObjectDetector && TicketRuntime.IsEngineObject(target))
            {
                return new EveryValueChangedEngineObject<TTarget, TProperty>(target, propertySelector, cmp, monitorTiming, cancelImmediately);
            }
            return new EveryValueChangedStandardObject<TTarget, TProperty>(target, propertySelector, cmp, monitorTiming, cancelImmediately);
        }
    }

    // Zero-allocation engine-object specialization. Mirrors EveryValueChangedStandardObject
    // but holds the target by strong reference and checks liveness via
    // TicketRuntime.IsEngineObjectAlive each tick.
    internal sealed class EveryValueChangedEngineObject<TTarget, TProperty> : ITicketAsyncEnumerable<TProperty>
        where TTarget : class
    {
        readonly TTarget target;
        readonly Func<TTarget, TProperty> propertySelector;
        readonly IEqualityComparer<TProperty> equalityComparer;
        readonly PlayerLoopTiming monitorTiming;
        readonly bool cancelImmediately;

        public EveryValueChangedEngineObject(TTarget target, Func<TTarget, TProperty> propertySelector, IEqualityComparer<TProperty> equalityComparer, PlayerLoopTiming monitorTiming, bool cancelImmediately)
        {
            this.target = target;
            this.propertySelector = propertySelector;
            this.equalityComparer = equalityComparer;
            this.monitorTiming = monitorTiming;
            this.cancelImmediately = cancelImmediately;
        }

        public ITicketAsyncEnumerator<TProperty> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _EveryValueChanged(target, propertySelector, equalityComparer, monitorTiming, cancellationToken, cancelImmediately);
        }

        sealed class _EveryValueChanged : MoveNextSource, ITicketAsyncEnumerator<TProperty>, IPlayerLoopItem
        {
            readonly TTarget target;
            readonly IEqualityComparer<TProperty> equalityComparer;
            readonly Func<TTarget, TProperty> propertySelector;
            readonly CancellationToken cancellationToken;
            readonly CancellationTokenRegistration cancellationTokenRegistration;

            bool first;
            TProperty currentValue;
            bool disposed;

            public _EveryValueChanged(TTarget target, Func<TTarget, TProperty> propertySelector, IEqualityComparer<TProperty> equalityComparer, PlayerLoopTiming monitorTiming, CancellationToken cancellationToken, bool cancelImmediately)
            {
                this.target = target;
                this.propertySelector = propertySelector;
                this.equalityComparer = equalityComparer;
                this.cancellationToken = cancellationToken;
                this.first = true;

                if (cancelImmediately && cancellationToken.CanBeCanceled)
                {
                    cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(state =>
                    {
                        var source = (_EveryValueChanged)state;
                        source.completionSource.TrySetCanceled(source.cancellationToken);
                    }, this);
                }

                TaskTracker.TrackActiveTask(this, 2);
                TicketRuntime.AddAction(monitorTiming, this);
            }

            public TProperty Current => currentValue;

            public Ticket<bool> MoveNextAsync()
            {
                if (disposed) return CompletedTasks.False;

                completionSource.Reset();

                if (cancellationToken.IsCancellationRequested)
                {
                    completionSource.TrySetCanceled(cancellationToken);
                    return new Ticket<bool>(this, completionSource.Version);
                }

                if (first)
                {
                    first = false;
                    if (!TicketRuntime.IsEngineObjectAlive(target))
                    {
                        return CompletedTasks.False;
                    }
                    this.currentValue = propertySelector(target);
                    return CompletedTasks.True;
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
                if (disposed || !TicketRuntime.IsEngineObjectAlive(target))
                {
                    completionSource.TrySetResult(false);
                    DisposeAsync().Forget();
                    return false;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    completionSource.TrySetCanceled(cancellationToken);
                    return false;
                }

                TProperty nextValue = default(TProperty);
                try
                {
                    nextValue = propertySelector(target);
                    if (equalityComparer.Equals(currentValue, nextValue))
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    completionSource.TrySetException(ex);
                    DisposeAsync().Forget();
                    return false;
                }

                currentValue = nextValue;
                completionSource.TrySetResult(true);
                return true;
            }
        }
    }

    internal sealed class EveryValueChangedStandardObject<TTarget, TProperty> : ITicketAsyncEnumerable<TProperty>
        where TTarget : class
    {
        readonly WeakReference<TTarget> target;
        readonly Func<TTarget, TProperty> propertySelector;
        readonly IEqualityComparer<TProperty> equalityComparer;
        readonly PlayerLoopTiming monitorTiming;
        readonly bool cancelImmediately;

        public EveryValueChangedStandardObject(TTarget target, Func<TTarget, TProperty> propertySelector, IEqualityComparer<TProperty> equalityComparer, PlayerLoopTiming monitorTiming, bool cancelImmediately)
        {
            this.target = new WeakReference<TTarget>(target, false);
            this.propertySelector = propertySelector;
            this.equalityComparer = equalityComparer;
            this.monitorTiming = monitorTiming;
            this.cancelImmediately = cancelImmediately;
        }

        public ITicketAsyncEnumerator<TProperty> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _EveryValueChanged(target, propertySelector, equalityComparer, monitorTiming, cancellationToken, cancelImmediately);
        }

        sealed class _EveryValueChanged : MoveNextSource, ITicketAsyncEnumerator<TProperty>, IPlayerLoopItem
        {
            readonly WeakReference<TTarget> target;
            readonly IEqualityComparer<TProperty> equalityComparer;
            readonly Func<TTarget, TProperty> propertySelector;
            readonly CancellationToken cancellationToken;
            readonly CancellationTokenRegistration cancellationTokenRegistration;

            bool first;
            TProperty currentValue;
            bool disposed;

            public _EveryValueChanged(WeakReference<TTarget> target, Func<TTarget, TProperty> propertySelector, IEqualityComparer<TProperty> equalityComparer, PlayerLoopTiming monitorTiming, CancellationToken cancellationToken, bool cancelImmediately)
            {
                this.target = target;
                this.propertySelector = propertySelector;
                this.equalityComparer = equalityComparer;
                this.cancellationToken = cancellationToken;
                this.first = true;

                if (cancelImmediately && cancellationToken.CanBeCanceled)
                {
                    cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(state =>
                    {
                        var source = (_EveryValueChanged)state;
                        source.completionSource.TrySetCanceled(source.cancellationToken);
                    }, this);
                }

                TaskTracker.TrackActiveTask(this, 2);
                TicketRuntime.AddAction(monitorTiming, this);
            }

            public TProperty Current => currentValue;

            public Ticket<bool> MoveNextAsync()
            {
                if (disposed) return CompletedTasks.False;

                completionSource.Reset();

                if (cancellationToken.IsCancellationRequested)
                {
                    completionSource.TrySetCanceled(cancellationToken);
                    return new Ticket<bool>(this, completionSource.Version);
                }

                if (first)
                {
                    first = false;
                    if (!target.TryGetTarget(out var t))
                    {
                        return CompletedTasks.False;
                    }
                    this.currentValue = propertySelector(t);
                    return CompletedTasks.True;
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
                if (disposed || !target.TryGetTarget(out var t))
                {
                    completionSource.TrySetResult(false);
                    DisposeAsync().Forget();
                    return false;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    completionSource.TrySetCanceled(cancellationToken);
                    return false;
                }

                TProperty nextValue = default(TProperty);
                try
                {
                    nextValue = propertySelector(t);
                    if (equalityComparer.Equals(currentValue, nextValue))
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    completionSource.TrySetException(ex);
                    DisposeAsync().Forget();
                    return false;
                }

                currentValue = nextValue;
                completionSource.TrySetResult(true);
                return true;
            }
        }
    }
}
