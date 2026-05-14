#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#if !UNITY_2019_1_OR_NEWER || Ticket_UGUI_SUPPORT
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CLabs.Tickets
{
    public static partial class UnityAsyncExtensions
    {
        public static AsyncUnityEventHandler GetAsyncEventHandler(this UnityEvent unityEvent, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler(unityEvent, cancellationToken, false);
        }

        public static Ticket OnInvokeAsync(this UnityEvent unityEvent, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler(unityEvent, cancellationToken, true).OnInvokeAsync();
        }

        public static ITicketAsyncEnumerable<AsyncUnit> OnInvokeAsAsyncEnumerable(this UnityEvent unityEvent, CancellationToken cancellationToken)
        {
            return new UnityEventHandlerAsyncEnumerable(unityEvent, cancellationToken);
        }

        public static AsyncUnityEventHandler<T> GetAsyncEventHandler<T>(this UnityEvent<T> unityEvent, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<T>(unityEvent, cancellationToken, false);
        }

        public static Ticket<T> OnInvokeAsync<T>(this UnityEvent<T> unityEvent, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<T>(unityEvent, cancellationToken, true).OnInvokeAsync();
        }

        public static ITicketAsyncEnumerable<T> OnInvokeAsAsyncEnumerable<T>(this UnityEvent<T> unityEvent, CancellationToken cancellationToken)
        {
            return new UnityEventHandlerAsyncEnumerable<T>(unityEvent, cancellationToken);
        }

        public static IAsyncClickEventHandler GetAsyncClickEventHandler(this Button button)
        {
            return new AsyncUnityEventHandler(button.onClick, button.GetCancellationTokenOnDestroy(), false);
        }

        public static IAsyncClickEventHandler GetAsyncClickEventHandler(this Button button, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler(button.onClick, cancellationToken, false);
        }

        public static Ticket OnClickAsync(this Button button)
        {
            return new AsyncUnityEventHandler(button.onClick, button.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
        }

        public static Ticket OnClickAsync(this Button button, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler(button.onClick, cancellationToken, true).OnInvokeAsync();
        }

        public static ITicketAsyncEnumerable<AsyncUnit> OnClickAsAsyncEnumerable(this Button button)
        {
            return new UnityEventHandlerAsyncEnumerable(button.onClick, button.GetCancellationTokenOnDestroy());
        }

        public static ITicketAsyncEnumerable<AsyncUnit> OnClickAsAsyncEnumerable(this Button button, CancellationToken cancellationToken)
        {
            return new UnityEventHandlerAsyncEnumerable(button.onClick, cancellationToken);
        }

        public static IAsyncValueChangedEventHandler<bool> GetAsyncValueChangedEventHandler(this Toggle toggle)
        {
            return new AsyncUnityEventHandler<bool>(toggle.onValueChanged, toggle.GetCancellationTokenOnDestroy(), false);
        }

        public static IAsyncValueChangedEventHandler<bool> GetAsyncValueChangedEventHandler(this Toggle toggle, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<bool>(toggle.onValueChanged, cancellationToken, false);
        }

        public static Ticket<bool> OnValueChangedAsync(this Toggle toggle)
        {
            return new AsyncUnityEventHandler<bool>(toggle.onValueChanged, toggle.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
        }

        public static Ticket<bool> OnValueChangedAsync(this Toggle toggle, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<bool>(toggle.onValueChanged, cancellationToken, true).OnInvokeAsync();
        }

        public static ITicketAsyncEnumerable<bool> OnValueChangedAsAsyncEnumerable(this Toggle toggle)
        {
            return new UnityEventHandlerAsyncEnumerable<bool>(toggle.onValueChanged, toggle.GetCancellationTokenOnDestroy());
        }

        public static ITicketAsyncEnumerable<bool> OnValueChangedAsAsyncEnumerable(this Toggle toggle, CancellationToken cancellationToken)
        {
            return new UnityEventHandlerAsyncEnumerable<bool>(toggle.onValueChanged, cancellationToken);
        }

        public static IAsyncValueChangedEventHandler<float> GetAsyncValueChangedEventHandler(this Scrollbar scrollbar)
        {
            return new AsyncUnityEventHandler<float>(scrollbar.onValueChanged, scrollbar.GetCancellationTokenOnDestroy(), false);
        }

        public static IAsyncValueChangedEventHandler<float> GetAsyncValueChangedEventHandler(this Scrollbar scrollbar, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<float>(scrollbar.onValueChanged, cancellationToken, false);
        }

        public static Ticket<float> OnValueChangedAsync(this Scrollbar scrollbar)
        {
            return new AsyncUnityEventHandler<float>(scrollbar.onValueChanged, scrollbar.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
        }

        public static Ticket<float> OnValueChangedAsync(this Scrollbar scrollbar, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<float>(scrollbar.onValueChanged, cancellationToken, true).OnInvokeAsync();
        }

        public static ITicketAsyncEnumerable<float> OnValueChangedAsAsyncEnumerable(this Scrollbar scrollbar)
        {
            return new UnityEventHandlerAsyncEnumerable<float>(scrollbar.onValueChanged, scrollbar.GetCancellationTokenOnDestroy());
        }

        public static ITicketAsyncEnumerable<float> OnValueChangedAsAsyncEnumerable(this Scrollbar scrollbar, CancellationToken cancellationToken)
        {
            return new UnityEventHandlerAsyncEnumerable<float>(scrollbar.onValueChanged, cancellationToken);
        }

        public static IAsyncValueChangedEventHandler<Vector2> GetAsyncValueChangedEventHandler(this ScrollRect scrollRect)
        {
            return new AsyncUnityEventHandler<Vector2>(scrollRect.onValueChanged, scrollRect.GetCancellationTokenOnDestroy(), false);
        }

        public static IAsyncValueChangedEventHandler<Vector2> GetAsyncValueChangedEventHandler(this ScrollRect scrollRect, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<Vector2>(scrollRect.onValueChanged, cancellationToken, false);
        }

        public static Ticket<Vector2> OnValueChangedAsync(this ScrollRect scrollRect)
        {
            return new AsyncUnityEventHandler<Vector2>(scrollRect.onValueChanged, scrollRect.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
        }

        public static Ticket<Vector2> OnValueChangedAsync(this ScrollRect scrollRect, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<Vector2>(scrollRect.onValueChanged, cancellationToken, true).OnInvokeAsync();
        }

        public static ITicketAsyncEnumerable<Vector2> OnValueChangedAsAsyncEnumerable(this ScrollRect scrollRect)
        {
            return new UnityEventHandlerAsyncEnumerable<Vector2>(scrollRect.onValueChanged, scrollRect.GetCancellationTokenOnDestroy());
        }

        public static ITicketAsyncEnumerable<Vector2> OnValueChangedAsAsyncEnumerable(this ScrollRect scrollRect, CancellationToken cancellationToken)
        {
            return new UnityEventHandlerAsyncEnumerable<Vector2>(scrollRect.onValueChanged, cancellationToken);
        }

        public static IAsyncValueChangedEventHandler<float> GetAsyncValueChangedEventHandler(this Slider slider)
        {
            return new AsyncUnityEventHandler<float>(slider.onValueChanged, slider.GetCancellationTokenOnDestroy(), false);
        }

        public static IAsyncValueChangedEventHandler<float> GetAsyncValueChangedEventHandler(this Slider slider, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<float>(slider.onValueChanged, cancellationToken, false);
        }

        public static Ticket<float> OnValueChangedAsync(this Slider slider)
        {
            return new AsyncUnityEventHandler<float>(slider.onValueChanged, slider.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
        }

        public static Ticket<float> OnValueChangedAsync(this Slider slider, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<float>(slider.onValueChanged, cancellationToken, true).OnInvokeAsync();
        }

        public static ITicketAsyncEnumerable<float> OnValueChangedAsAsyncEnumerable(this Slider slider)
        {
            return new UnityEventHandlerAsyncEnumerable<float>(slider.onValueChanged, slider.GetCancellationTokenOnDestroy());
        }

        public static ITicketAsyncEnumerable<float> OnValueChangedAsAsyncEnumerable(this Slider slider, CancellationToken cancellationToken)
        {
            return new UnityEventHandlerAsyncEnumerable<float>(slider.onValueChanged, cancellationToken);
        }

        public static IAsyncEndEditEventHandler<string> GetAsyncEndEditEventHandler(this InputField inputField)
        {
            return new AsyncUnityEventHandler<string>(inputField.onEndEdit, inputField.GetCancellationTokenOnDestroy(), false);
        }

        public static IAsyncEndEditEventHandler<string> GetAsyncEndEditEventHandler(this InputField inputField, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<string>(inputField.onEndEdit, cancellationToken, false);
        }

        public static Ticket<string> OnEndEditAsync(this InputField inputField)
        {
            return new AsyncUnityEventHandler<string>(inputField.onEndEdit, inputField.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
        }

        public static Ticket<string> OnEndEditAsync(this InputField inputField, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<string>(inputField.onEndEdit, cancellationToken, true).OnInvokeAsync();
        }

        public static ITicketAsyncEnumerable<string> OnEndEditAsAsyncEnumerable(this InputField inputField)
        {
            return new UnityEventHandlerAsyncEnumerable<string>(inputField.onEndEdit, inputField.GetCancellationTokenOnDestroy());
        }

        public static ITicketAsyncEnumerable<string> OnEndEditAsAsyncEnumerable(this InputField inputField, CancellationToken cancellationToken)
        {
            return new UnityEventHandlerAsyncEnumerable<string>(inputField.onEndEdit, cancellationToken);
        }

        public static IAsyncValueChangedEventHandler<string> GetAsyncValueChangedEventHandler(this InputField inputField)
        {
            return new AsyncUnityEventHandler<string>(inputField.onValueChanged, inputField.GetCancellationTokenOnDestroy(), false);
        }

        public static IAsyncValueChangedEventHandler<string> GetAsyncValueChangedEventHandler(this InputField inputField, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<string>(inputField.onValueChanged, cancellationToken, false);
        }

        public static Ticket<string> OnValueChangedAsync(this InputField inputField)
        {
            return new AsyncUnityEventHandler<string>(inputField.onValueChanged, inputField.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
        }

        public static Ticket<string> OnValueChangedAsync(this InputField inputField, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<string>(inputField.onValueChanged, cancellationToken, true).OnInvokeAsync();
        }

        public static ITicketAsyncEnumerable<string> OnValueChangedAsAsyncEnumerable(this InputField inputField)
        {
            return new UnityEventHandlerAsyncEnumerable<string>(inputField.onValueChanged, inputField.GetCancellationTokenOnDestroy());
        }

        public static ITicketAsyncEnumerable<string> OnValueChangedAsAsyncEnumerable(this InputField inputField, CancellationToken cancellationToken)
        {
            return new UnityEventHandlerAsyncEnumerable<string>(inputField.onValueChanged, cancellationToken);
        }

        public static IAsyncValueChangedEventHandler<int> GetAsyncValueChangedEventHandler(this Dropdown dropdown)
        {
            return new AsyncUnityEventHandler<int>(dropdown.onValueChanged, dropdown.GetCancellationTokenOnDestroy(), false);
        }

        public static IAsyncValueChangedEventHandler<int> GetAsyncValueChangedEventHandler(this Dropdown dropdown, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<int>(dropdown.onValueChanged, cancellationToken, false);
        }

        public static Ticket<int> OnValueChangedAsync(this Dropdown dropdown)
        {
            return new AsyncUnityEventHandler<int>(dropdown.onValueChanged, dropdown.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
        }

        public static Ticket<int> OnValueChangedAsync(this Dropdown dropdown, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler<int>(dropdown.onValueChanged, cancellationToken, true).OnInvokeAsync();
        }

        public static ITicketAsyncEnumerable<int> OnValueChangedAsAsyncEnumerable(this Dropdown dropdown)
        {
            return new UnityEventHandlerAsyncEnumerable<int>(dropdown.onValueChanged, dropdown.GetCancellationTokenOnDestroy());
        }

        public static ITicketAsyncEnumerable<int> OnValueChangedAsAsyncEnumerable(this Dropdown dropdown, CancellationToken cancellationToken)
        {
            return new UnityEventHandlerAsyncEnumerable<int>(dropdown.onValueChanged, cancellationToken);
        }
    }

    public interface IAsyncClickEventHandler : IDisposable
    {
        Ticket OnClickAsync();
    }

    public interface IAsyncValueChangedEventHandler<T> : IDisposable
    {
        Ticket<T> OnValueChangedAsync();
    }

    public interface IAsyncEndEditEventHandler<T> : IDisposable
    {
        Ticket<T> OnEndEditAsync();
    }

    // for TMP_PRO

    public interface IAsyncEndTextSelectionEventHandler<T> : IDisposable
    {
        Ticket<T> OnEndTextSelectionAsync();
    }

    public interface IAsyncTextSelectionEventHandler<T> : IDisposable
    {
        Ticket<T> OnTextSelectionAsync();
    }

    public interface IAsyncDeselectEventHandler<T> : IDisposable
    {
        Ticket<T> OnDeselectAsync();
    }

    public interface IAsyncSelectEventHandler<T> : IDisposable
    {
        Ticket<T> OnSelectAsync();
    }

    public interface IAsyncSubmitEventHandler<T> : IDisposable
    {
        Ticket<T> OnSubmitAsync();
    }

    internal sealed class TextSelectionEventConverter : UnityEvent<(string, int, int)>, IDisposable
    {
        readonly UnityEvent<string, int, int> innerEvent;
        readonly UnityAction<string, int, int> invokeDelegate;


        public TextSelectionEventConverter(UnityEvent<string, int, int> unityEvent)
        {
            this.innerEvent = unityEvent;
            this.invokeDelegate = InvokeCore;

            innerEvent.AddListener(invokeDelegate);
        }

        void InvokeCore(string item1, int item2, int item3)
        {
            Invoke((item1, item2, item3));
        }

        public void Dispose()
        {
            innerEvent.RemoveListener(invokeDelegate);
        }
    }

    public sealed class AsyncUnityEventHandler : ITicketSource, IDisposable, IAsyncClickEventHandler
    {
        static Action<object> cancellationCallback = CancellationCallback;

        readonly UnityAction action;
        readonly UnityEvent unityEvent;

        CancellationToken cancellationToken;
        CancellationTokenRegistration registration;
        bool isDisposed;
        bool callOnce;

        TicketCompletionSourceCore<AsyncUnit> core;

        public AsyncUnityEventHandler(UnityEvent unityEvent, CancellationToken cancellationToken, bool callOnce)
        {
            this.cancellationToken = cancellationToken;
            if (cancellationToken.IsCancellationRequested)
            {
                isDisposed = true;
                return;
            }

            this.action = Invoke;
            this.unityEvent = unityEvent;
            this.callOnce = callOnce;

            unityEvent.AddListener(action);

            if (cancellationToken.CanBeCanceled)
            {
                registration = cancellationToken.RegisterWithoutCaptureExecutionContext(cancellationCallback, this);
            }

            TaskTracker.TrackActiveTask(this, 3);
        }

        public Ticket OnInvokeAsync()
        {
            core.Reset();
            if (isDisposed)
            {
                core.TrySetCanceled(this.cancellationToken);
            }
            return new Ticket(this, core.Version);
        }

        void Invoke()
        {
            core.TrySetResult(AsyncUnit.Default);
        }

        static void CancellationCallback(object state)
        {
            var self = (AsyncUnityEventHandler)state;
            self.Dispose();
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                TaskTracker.RemoveTracking(this);
                registration.Dispose();
                if (unityEvent != null)
                {
                    unityEvent.RemoveListener(action);
                }
                core.TrySetCanceled(cancellationToken);
            }
        }

        Ticket IAsyncClickEventHandler.OnClickAsync()
        {
            return OnInvokeAsync();
        }

        void ITicketSource.GetResult(short token)
        {
            try
            {
                core.GetResult(token);
            }
            finally
            {
                if (callOnce)
                {
                    Dispose();
                }
            }
        }

        TicketStatus ITicketSource.GetStatus(short token)
        {
            return core.GetStatus(token);
        }

        TicketStatus ITicketSource.UnsafeGetStatus()
        {
            return core.UnsafeGetStatus();
        }

        void ITicketSource.OnCompleted(Action<object> continuation, object state, short token)
        {
            core.OnCompleted(continuation, state, token);
        }
    }

    public sealed class AsyncUnityEventHandler<T> : ITicketSource<T>, IDisposable, IAsyncValueChangedEventHandler<T>, IAsyncEndEditEventHandler<T>
        , IAsyncEndTextSelectionEventHandler<T>, IAsyncTextSelectionEventHandler<T>, IAsyncDeselectEventHandler<T>, IAsyncSelectEventHandler<T>, IAsyncSubmitEventHandler<T>
    {
        static Action<object> cancellationCallback = CancellationCallback;

        readonly UnityAction<T> action;
        readonly UnityEvent<T> unityEvent;

        CancellationToken cancellationToken;
        CancellationTokenRegistration registration;
        bool isDisposed;
        bool callOnce;

        TicketCompletionSourceCore<T> core;

        public AsyncUnityEventHandler(UnityEvent<T> unityEvent, CancellationToken cancellationToken, bool callOnce)
        {
            this.cancellationToken = cancellationToken;
            if (cancellationToken.IsCancellationRequested)
            {
                isDisposed = true;
                return;
            }

            this.action = Invoke;
            this.unityEvent = unityEvent;
            this.callOnce = callOnce;

            unityEvent.AddListener(action);

            if (cancellationToken.CanBeCanceled)
            {
                registration = cancellationToken.RegisterWithoutCaptureExecutionContext(cancellationCallback, this);
            }

            TaskTracker.TrackActiveTask(this, 3);
        }

        public Ticket<T> OnInvokeAsync()
        {
            core.Reset();
            if (isDisposed)
            {
                core.TrySetCanceled(this.cancellationToken);
            }
            return new Ticket<T>(this, core.Version);
        }

        void Invoke(T result)
        {
            core.TrySetResult(result);
        }

        static void CancellationCallback(object state)
        {
            var self = (AsyncUnityEventHandler<T>)state;
            self.Dispose();
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                TaskTracker.RemoveTracking(this);
                registration.Dispose();
                if (unityEvent != null)
                {
                    // Dispose inner delegate for TextSelectionEventConverter
                    if (unityEvent is IDisposable disp)
                    {
                        disp.Dispose();
                    }

                    unityEvent.RemoveListener(action);
                }

                core.TrySetCanceled();
            }
        }

        Ticket<T> IAsyncValueChangedEventHandler<T>.OnValueChangedAsync()
        {
            return OnInvokeAsync();
        }

        Ticket<T> IAsyncEndEditEventHandler<T>.OnEndEditAsync()
        {
            return OnInvokeAsync();
        }

        Ticket<T> IAsyncEndTextSelectionEventHandler<T>.OnEndTextSelectionAsync()
        {
            return OnInvokeAsync();
        }

        Ticket<T> IAsyncTextSelectionEventHandler<T>.OnTextSelectionAsync()
        {
            return OnInvokeAsync();
        }

        Ticket<T> IAsyncDeselectEventHandler<T>.OnDeselectAsync()
        {
            return OnInvokeAsync();
        }

        Ticket<T> IAsyncSelectEventHandler<T>.OnSelectAsync()
        {
            return OnInvokeAsync();
        }

        Ticket<T> IAsyncSubmitEventHandler<T>.OnSubmitAsync()
        {
            return OnInvokeAsync();
        }

        T ITicketSource<T>.GetResult(short token)
        {
            try
            {
                return core.GetResult(token);
            }
            finally
            {
                if (callOnce)
                {
                    Dispose();
                }
            }
        }

        void ITicketSource.GetResult(short token)
        {
            ((ITicketSource<T>)this).GetResult(token);
        }

        TicketStatus ITicketSource.GetStatus(short token)
        {
            return core.GetStatus(token);
        }

        TicketStatus ITicketSource.UnsafeGetStatus()
        {
            return core.UnsafeGetStatus();
        }

        void ITicketSource.OnCompleted(Action<object> continuation, object state, short token)
        {
            core.OnCompleted(continuation, state, token);
        }
    }

    public sealed class UnityEventHandlerAsyncEnumerable : ITicketAsyncEnumerable<AsyncUnit>
    {
        readonly UnityEvent unityEvent;
        readonly CancellationToken cancellationToken1;

        public UnityEventHandlerAsyncEnumerable(UnityEvent unityEvent, CancellationToken cancellationToken)
        {
            this.unityEvent = unityEvent;
            this.cancellationToken1 = cancellationToken;
        }

        public ITicketAsyncEnumerator<AsyncUnit> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            if (this.cancellationToken1 == cancellationToken)
            {
                return new UnityEventHandlerAsyncEnumerator(unityEvent, this.cancellationToken1, CancellationToken.None);
            }
            else
            {
                return new UnityEventHandlerAsyncEnumerator(unityEvent, this.cancellationToken1, cancellationToken);
            }
        }

        class UnityEventHandlerAsyncEnumerator : MoveNextSource, ITicketAsyncEnumerator<AsyncUnit>
        {
            static readonly Action<object> cancel1 = OnCanceled1;
            static readonly Action<object> cancel2 = OnCanceled2;

            readonly UnityEvent unityEvent;
            CancellationToken cancellationToken1;
            CancellationToken cancellationToken2;

            UnityAction unityAction;
            CancellationTokenRegistration registration1;
            CancellationTokenRegistration registration2;
            bool isDisposed;

            public UnityEventHandlerAsyncEnumerator(UnityEvent unityEvent, CancellationToken cancellationToken1, CancellationToken cancellationToken2)
            {
                this.unityEvent = unityEvent;
                this.cancellationToken1 = cancellationToken1;
                this.cancellationToken2 = cancellationToken2;
            }

            public AsyncUnit Current => default;

            public Ticket<bool> MoveNextAsync()
            {
                cancellationToken1.ThrowIfCancellationRequested();
                cancellationToken2.ThrowIfCancellationRequested();
                completionSource.Reset();

                if (unityAction == null)
                {
                    unityAction = Invoke;

                    TaskTracker.TrackActiveTask(this, 3);
                    unityEvent.AddListener(unityAction);
                    if (cancellationToken1.CanBeCanceled)
                    {
                        registration1 = cancellationToken1.RegisterWithoutCaptureExecutionContext(cancel1, this);
                    }
                    if (cancellationToken2.CanBeCanceled)
                    {
                        registration2 = cancellationToken2.RegisterWithoutCaptureExecutionContext(cancel2, this);
                    }
                }

                return new Ticket<bool>(this, completionSource.Version);
            }

            void Invoke()
            {
                completionSource.TrySetResult(true);
            }

            static void OnCanceled1(object state)
            {
                var self = (UnityEventHandlerAsyncEnumerator)state;
                try
                {
                    self.completionSource.TrySetCanceled(self.cancellationToken1);
                }
                finally
                {
                    self.DisposeAsync().Forget();
                }
            }

            static void OnCanceled2(object state)
            {
                var self = (UnityEventHandlerAsyncEnumerator)state;
                try
                {
                    self.completionSource.TrySetCanceled(self.cancellationToken2);
                }
                finally
                {
                    self.DisposeAsync().Forget();
                }
            }

            public Ticket DisposeAsync()
            {
                if (!isDisposed)
                {
                    isDisposed = true;
                    TaskTracker.RemoveTracking(this);
                    registration1.Dispose();
                    registration2.Dispose();
                    unityEvent.RemoveListener(unityAction);

                    completionSource.TrySetCanceled();
                }

                return default;
            }
        }
    }

    public sealed class UnityEventHandlerAsyncEnumerable<T> : ITicketAsyncEnumerable<T>
    {
        readonly UnityEvent<T> unityEvent;
        readonly CancellationToken cancellationToken1;

        public UnityEventHandlerAsyncEnumerable(UnityEvent<T> unityEvent, CancellationToken cancellationToken)
        {
            this.unityEvent = unityEvent;
            this.cancellationToken1 = cancellationToken;
        }

        public ITicketAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            if (this.cancellationToken1 == cancellationToken)
            {
                return new UnityEventHandlerAsyncEnumerator(unityEvent, this.cancellationToken1, CancellationToken.None);
            }
            else
            {
                return new UnityEventHandlerAsyncEnumerator(unityEvent, this.cancellationToken1, cancellationToken);
            }
        }

        class UnityEventHandlerAsyncEnumerator : MoveNextSource, ITicketAsyncEnumerator<T>
        {
            static readonly Action<object> cancel1 = OnCanceled1;
            static readonly Action<object> cancel2 = OnCanceled2;

            readonly UnityEvent<T> unityEvent;
            CancellationToken cancellationToken1;
            CancellationToken cancellationToken2;

            UnityAction<T> unityAction;
            CancellationTokenRegistration registration1;
            CancellationTokenRegistration registration2;
            bool isDisposed;

            public UnityEventHandlerAsyncEnumerator(UnityEvent<T> unityEvent, CancellationToken cancellationToken1, CancellationToken cancellationToken2)
            {
                this.unityEvent = unityEvent;
                this.cancellationToken1 = cancellationToken1;
                this.cancellationToken2 = cancellationToken2;
            }

            public T Current { get; private set; }

            public Ticket<bool> MoveNextAsync()
            {
                cancellationToken1.ThrowIfCancellationRequested();
                cancellationToken2.ThrowIfCancellationRequested();
                completionSource.Reset();

                if (unityAction == null)
                {
                    unityAction = Invoke;

                    TaskTracker.TrackActiveTask(this, 3);
                    unityEvent.AddListener(unityAction);
                    if (cancellationToken1.CanBeCanceled)
                    {
                        registration1 = cancellationToken1.RegisterWithoutCaptureExecutionContext(cancel1, this);
                    }
                    if (cancellationToken2.CanBeCanceled)
                    {
                        registration2 = cancellationToken2.RegisterWithoutCaptureExecutionContext(cancel2, this);
                    }
                }

                return new Ticket<bool>(this, completionSource.Version);
            }

            void Invoke(T value)
            {
                Current = value;
                completionSource.TrySetResult(true);
            }

            static void OnCanceled1(object state)
            {
                var self = (UnityEventHandlerAsyncEnumerator)state;
                try
                {
                    self.completionSource.TrySetCanceled(self.cancellationToken1);
                }
                finally
                {
                    self.DisposeAsync().Forget();
                }
            }

            static void OnCanceled2(object state)
            {
                var self = (UnityEventHandlerAsyncEnumerator)state;
                try
                {
                    self.completionSource.TrySetCanceled(self.cancellationToken2);
                }
                finally
                {
                    self.DisposeAsync().Forget();
                }
            }

            public Ticket DisposeAsync()
            {
                if (!isDisposed)
                {
                    isDisposed = true;
                    TaskTracker.RemoveTracking(this);
                    registration1.Dispose();
                    registration2.Dispose();
                    if (unityEvent is IDisposable disp)
                    {
                        disp.Dispose();
                    }
                    unityEvent.RemoveListener(unityAction);

                    completionSource.TrySetCanceled();
                }

                return default;
            }
        }
    }
}

#endif
