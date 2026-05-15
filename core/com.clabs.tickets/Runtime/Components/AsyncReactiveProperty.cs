using System;
using System.Threading;

namespace CLabs.Tickets
{
    public interface IReadOnlyAsyncReactiveProperty<T> : ITicketAsyncEnumerable<T>
    {
        T Value { get; }
        ITicketAsyncEnumerable<T> WithoutCurrent();
        Ticket<T> WaitAsync(CancellationToken cancellationToken = default);
    }

    public interface IAsyncReactiveProperty<T> : IReadOnlyAsyncReactiveProperty<T>
    {
        new T Value { get; set; }
    }

    [Serializable]
    public sealed class AsyncReactiveProperty<T> : IAsyncReactiveProperty<T>, IDisposable
    {
        TriggerEvent<T> triggerEvent;

        // Upstream Ticket had a [SerializeField] attribute here for inspector
        // visibility. Removed in Phase B engine separation — the attribute had
        // no runtime effect outside the Unity inspector, and AsyncReactiveProperty
        // is a pure-C# type that nobody inspects in practice.
        T latestValue;

        public T Value
        {
            get
            {
                return latestValue;
            }
            set
            {
                this.latestValue = value;
                triggerEvent.SetResult(value);
            }
        }

        public AsyncReactiveProperty(T value)
        {
            this.latestValue = value;
            this.triggerEvent = default;
        }

        public ITicketAsyncEnumerable<T> WithoutCurrent()
        {
            return new WithoutCurrentEnumerable(this);
        }

        public ITicketAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken)
        {
            return new Enumerator(this, cancellationToken, true);
        }

        public void Dispose()
        {
            triggerEvent.SetCompleted();
        }

        public static implicit operator T(AsyncReactiveProperty<T> value)
        {
            return value.Value;
        }

        public override string ToString()
        {
            if (isValueType) return latestValue.ToString();
            return latestValue?.ToString();
        }

        public Ticket<T> WaitAsync(CancellationToken cancellationToken = default)
        {
            return new Ticket<T>(WaitAsyncSource.Create(this, cancellationToken, out var token), token);
        }

        static bool isValueType;

        static AsyncReactiveProperty()
        {
            isValueType = typeof(T).IsValueType;
        }

        sealed class WaitAsyncSource : ITicketSource<T>, ITriggerHandler<T>, ITaskPoolNode<WaitAsyncSource>
        {
            static Action<object> cancellationCallback = CancellationCallback;

            static TaskPool<WaitAsyncSource> pool;
            WaitAsyncSource nextNode;
            ref WaitAsyncSource ITaskPoolNode<WaitAsyncSource>.NextNode => ref nextNode;

            static WaitAsyncSource()
            {
                TaskPool.RegisterSizeGetter(typeof(WaitAsyncSource), () => pool.Size);
            }

            AsyncReactiveProperty<T> parent;
            CancellationToken cancellationToken;
            CancellationTokenRegistration cancellationTokenRegistration;
            TicketCompletionSourceCore<T> core;

            WaitAsyncSource()
            {
            }

            public static ITicketSource<T> Create(AsyncReactiveProperty<T> parent, CancellationToken cancellationToken, out short token)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return AutoResetTicketCompletionSource<T>.CreateFromCanceled(cancellationToken, out token);
                }

                if (!pool.TryPop(out var result))
                {
                    result = new WaitAsyncSource();
                }

                result.parent = parent;
                result.cancellationToken = cancellationToken;

                if (cancellationToken.CanBeCanceled)
                {
                    result.cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(cancellationCallback, result);
                }

                result.parent.triggerEvent.Add(result);

                TaskTracker.TrackActiveTask(result, 3);

                token = result.core.Version;
                return result;
            }

            bool TryReturn()
            {
                TaskTracker.RemoveTracking(this);
                core.Reset();
                cancellationTokenRegistration.Dispose();
                cancellationTokenRegistration = default;
                parent.triggerEvent.Remove(this);
                parent = null;
                cancellationToken = default;
                return pool.TryPush(this);
            }

            static void CancellationCallback(object state)
            {
                var self = (WaitAsyncSource)state;
                self.OnCanceled(self.cancellationToken);
            }

            // ITicketSource

            public T GetResult(short token)
            {
                try
                {
                    return core.GetResult(token);
                }
                finally
                {
                    TryReturn();
                }
            }

            void ITicketSource.GetResult(short token)
            {
                GetResult(token);
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                core.OnCompleted(continuation, state, token);
            }

            public TicketStatus GetStatus(short token)
            {
                return core.GetStatus(token);
            }

            public TicketStatus UnsafeGetStatus()
            {
                return core.UnsafeGetStatus();
            }

            // ITriggerHandler

            ITriggerHandler<T> ITriggerHandler<T>.Prev { get; set; }
            ITriggerHandler<T> ITriggerHandler<T>.Next { get; set; }

            public void OnCanceled(CancellationToken cancellationToken)
            {
                core.TrySetCanceled(cancellationToken);
            }

            public void OnCompleted()
            {
                // Complete as Cancel.
                core.TrySetCanceled(CancellationToken.None);
            }

            public void OnError(Exception ex)
            {
                core.TrySetException(ex);
            }

            public void OnNext(T value)
            {
                core.TrySetResult(value);
            }
        }

        sealed class WithoutCurrentEnumerable : ITicketAsyncEnumerable<T>
        {
            readonly AsyncReactiveProperty<T> parent;

            public WithoutCurrentEnumerable(AsyncReactiveProperty<T> parent)
            {
                this.parent = parent;
            }

            public ITicketAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                return new Enumerator(parent, cancellationToken, false);
            }
        }

        sealed class Enumerator : MoveNextSource, ITicketAsyncEnumerator<T>, ITriggerHandler<T>
        {
            static Action<object> cancellationCallback = CancellationCallback;

            readonly AsyncReactiveProperty<T> parent;
            readonly CancellationToken cancellationToken;
            readonly CancellationTokenRegistration cancellationTokenRegistration;
            T value;
            bool isDisposed;
            bool firstCall;

            public Enumerator(AsyncReactiveProperty<T> parent, CancellationToken cancellationToken, bool publishCurrentValue)
            {
                this.parent = parent;
                this.cancellationToken = cancellationToken;
                this.firstCall = publishCurrentValue;

                parent.triggerEvent.Add(this);
                TaskTracker.TrackActiveTask(this, 3);

                if (cancellationToken.CanBeCanceled)
                {
                    cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(cancellationCallback, this);
                }
            }

            public T Current => value;

            ITriggerHandler<T> ITriggerHandler<T>.Prev { get; set; }
            ITriggerHandler<T> ITriggerHandler<T>.Next { get; set; }

            public Ticket<bool> MoveNextAsync()
            {
                // raise latest value on first call.
                if (firstCall)
                {
                    firstCall = false;
                    value = parent.Value;
                    return CompletedTasks.True;
                }

                completionSource.Reset();
                return new Ticket<bool>(this, completionSource.Version);
            }

            public Ticket DisposeAsync()
            {
                if (!isDisposed)
                {
                    isDisposed = true;
                    TaskTracker.RemoveTracking(this);
                    completionSource.TrySetCanceled(cancellationToken);
                    parent.triggerEvent.Remove(this);
                }
                return default;
            }

            public void OnNext(T value)
            {
                this.value = value;
                completionSource.TrySetResult(true);
            }

            public void OnCanceled(CancellationToken cancellationToken)
            {
                DisposeAsync().Forget();
            }

            public void OnCompleted()
            {
                completionSource.TrySetResult(false);
            }

            public void OnError(Exception ex)
            {
                completionSource.TrySetException(ex);
            }

            static void CancellationCallback(object state)
            {
                var self = (Enumerator)state;
                self.DisposeAsync().Forget();
            }
        }
    }

    public sealed class ReadOnlyAsyncReactiveProperty<T> : IReadOnlyAsyncReactiveProperty<T>, IDisposable
    {
        TriggerEvent<T> triggerEvent;

        T latestValue;
        ITicketAsyncEnumerator<T> enumerator;

        public T Value
        {
            get
            {
                return latestValue;
            }
        }

        public ReadOnlyAsyncReactiveProperty(T initialValue, ITicketAsyncEnumerable<T> source, CancellationToken cancellationToken)
        {
            latestValue = initialValue;
            ConsumeEnumerator(source, cancellationToken).Forget();
        }

        public ReadOnlyAsyncReactiveProperty(ITicketAsyncEnumerable<T> source, CancellationToken cancellationToken)
        {
            ConsumeEnumerator(source, cancellationToken).Forget();
        }

        async TicketVoid ConsumeEnumerator(ITicketAsyncEnumerable<T> source, CancellationToken cancellationToken)
        {
            enumerator = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await enumerator.MoveNextAsync())
                {
                    var value = enumerator.Current;
                    this.latestValue = value;
                    triggerEvent.SetResult(value);
                }
            }
            finally
            {
                await enumerator.DisposeAsync();
                enumerator = null;
            }
        }

        public ITicketAsyncEnumerable<T> WithoutCurrent()
        {
            return new WithoutCurrentEnumerable(this);
        }

        public ITicketAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken)
        {
            return new Enumerator(this, cancellationToken, true);
        }

        public void Dispose()
        {
            if (enumerator != null)
            {
                enumerator.DisposeAsync().Forget();
            }

            triggerEvent.SetCompleted();
        }

        public static implicit operator T(ReadOnlyAsyncReactiveProperty<T> value)
        {
            return value.Value;
        }

        public override string ToString()
        {
            if (isValueType) return latestValue.ToString();
            return latestValue?.ToString();
        }

        public Ticket<T> WaitAsync(CancellationToken cancellationToken = default)
        {
            return new Ticket<T>(WaitAsyncSource.Create(this, cancellationToken, out var token), token);
        }

        static bool isValueType;

        static ReadOnlyAsyncReactiveProperty()
        {
            isValueType = typeof(T).IsValueType;
        }

        sealed class WaitAsyncSource : ITicketSource<T>, ITriggerHandler<T>, ITaskPoolNode<WaitAsyncSource>
        {
            static Action<object> cancellationCallback = CancellationCallback;

            static TaskPool<WaitAsyncSource> pool;
            WaitAsyncSource nextNode;
            ref WaitAsyncSource ITaskPoolNode<WaitAsyncSource>.NextNode => ref nextNode;

            static WaitAsyncSource()
            {
                TaskPool.RegisterSizeGetter(typeof(WaitAsyncSource), () => pool.Size);
            }

            ReadOnlyAsyncReactiveProperty<T> parent;
            CancellationToken cancellationToken;
            CancellationTokenRegistration cancellationTokenRegistration;
            TicketCompletionSourceCore<T> core;

            WaitAsyncSource()
            {
            }

            public static ITicketSource<T> Create(ReadOnlyAsyncReactiveProperty<T> parent, CancellationToken cancellationToken, out short token)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return AutoResetTicketCompletionSource<T>.CreateFromCanceled(cancellationToken, out token);
                }

                if (!pool.TryPop(out var result))
                {
                    result = new WaitAsyncSource();
                }

                result.parent = parent;
                result.cancellationToken = cancellationToken;

                if (cancellationToken.CanBeCanceled)
                {
                    result.cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(cancellationCallback, result);
                }

                result.parent.triggerEvent.Add(result);

                TaskTracker.TrackActiveTask(result, 3);

                token = result.core.Version;
                return result;
            }

            bool TryReturn()
            {
                TaskTracker.RemoveTracking(this);
                core.Reset();
                cancellationTokenRegistration.Dispose();
                cancellationTokenRegistration = default;
                parent.triggerEvent.Remove(this);
                parent = null;
                cancellationToken = default;
                return pool.TryPush(this);
            }

            static void CancellationCallback(object state)
            {
                var self = (WaitAsyncSource)state;
                self.OnCanceled(self.cancellationToken);
            }

            // ITicketSource

            public T GetResult(short token)
            {
                try
                {
                    return core.GetResult(token);
                }
                finally
                {
                    TryReturn();
                }
            }

            void ITicketSource.GetResult(short token)
            {
                GetResult(token);
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                core.OnCompleted(continuation, state, token);
            }

            public TicketStatus GetStatus(short token)
            {
                return core.GetStatus(token);
            }

            public TicketStatus UnsafeGetStatus()
            {
                return core.UnsafeGetStatus();
            }

            // ITriggerHandler

            ITriggerHandler<T> ITriggerHandler<T>.Prev { get; set; }
            ITriggerHandler<T> ITriggerHandler<T>.Next { get; set; }

            public void OnCanceled(CancellationToken cancellationToken)
            {
                core.TrySetCanceled(cancellationToken);
            }

            public void OnCompleted()
            {
                // Complete as Cancel.
                core.TrySetCanceled(CancellationToken.None);
            }

            public void OnError(Exception ex)
            {
                core.TrySetException(ex);
            }

            public void OnNext(T value)
            {
                core.TrySetResult(value);
            }
        }

        sealed class WithoutCurrentEnumerable : ITicketAsyncEnumerable<T>
        {
            readonly ReadOnlyAsyncReactiveProperty<T> parent;

            public WithoutCurrentEnumerable(ReadOnlyAsyncReactiveProperty<T> parent)
            {
                this.parent = parent;
            }

            public ITicketAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                return new Enumerator(parent, cancellationToken, false);
            }
        }

        sealed class Enumerator : MoveNextSource, ITicketAsyncEnumerator<T>, ITriggerHandler<T>
        {
            static Action<object> cancellationCallback = CancellationCallback;

            readonly ReadOnlyAsyncReactiveProperty<T> parent;
            readonly CancellationToken cancellationToken;
            readonly CancellationTokenRegistration cancellationTokenRegistration;
            T value;
            bool isDisposed;
            bool firstCall;

            public Enumerator(ReadOnlyAsyncReactiveProperty<T> parent, CancellationToken cancellationToken, bool publishCurrentValue)
            {
                this.parent = parent;
                this.cancellationToken = cancellationToken;
                this.firstCall = publishCurrentValue;

                parent.triggerEvent.Add(this);
                TaskTracker.TrackActiveTask(this, 3);

                if (cancellationToken.CanBeCanceled)
                {
                    cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(cancellationCallback, this);
                }
            }

            public T Current => value;
            ITriggerHandler<T> ITriggerHandler<T>.Prev { get; set; }
            ITriggerHandler<T> ITriggerHandler<T>.Next { get; set; }

            public Ticket<bool> MoveNextAsync()
            {
                // raise latest value on first call.
                if (firstCall)
                {
                    firstCall = false;
                    value = parent.Value;
                    return CompletedTasks.True;
                }

                completionSource.Reset();
                return new Ticket<bool>(this, completionSource.Version);
            }

            public Ticket DisposeAsync()
            {
                if (!isDisposed)
                {
                    isDisposed = true;
                    TaskTracker.RemoveTracking(this);
                    completionSource.TrySetCanceled(cancellationToken);
                    parent.triggerEvent.Remove(this);
                }
                return default;
            }

            public void OnNext(T value)
            {
                this.value = value;
                completionSource.TrySetResult(true);
            }

            public void OnCanceled(CancellationToken cancellationToken)
            {
                DisposeAsync().Forget();
            }

            public void OnCompleted()
            {
                completionSource.TrySetResult(false);
            }

            public void OnError(Exception ex)
            {
                completionSource.TrySetException(ex);
            }

            static void CancellationCallback(object state)
            {
                var self = (Enumerator)state;
                self.DisposeAsync().Forget();
            }
        }
    }

    public static class StateExtensions
    {
        public static ReadOnlyAsyncReactiveProperty<T> ToReadOnlyAsyncReactiveProperty<T>(this ITicketAsyncEnumerable<T> source, CancellationToken cancellationToken)
        {
            return new ReadOnlyAsyncReactiveProperty<T>(source, cancellationToken);
        }

        public static ReadOnlyAsyncReactiveProperty<T> ToReadOnlyAsyncReactiveProperty<T>(this ITicketAsyncEnumerable<T> source, T initialValue, CancellationToken cancellationToken)
        {
            return new ReadOnlyAsyncReactiveProperty<T>(initialValue, source, cancellationToken);
        }
    }
}