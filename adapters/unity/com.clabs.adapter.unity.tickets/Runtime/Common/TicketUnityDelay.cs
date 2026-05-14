#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading;
using UnityEngine;

namespace CLabs.Tickets
{
    /// <summary>
    /// Unity-specific delay helpers that were previously exposed on
    /// <c>partial struct Ticket</c> in core. They live here because they
    /// depend on <see cref="MonoBehaviour"/> or <see cref="Awaitable"/>,
    /// and C# partial types cannot span assemblies — the engine-agnostic
    /// core asmdef cannot reference <c>UnityEngine</c>.
    ///
    /// Callers that used <c>Ticket.WaitForEndOfFrame(...)</c> should move
    /// to <c>TicketUnityDelay.WaitForEndOfFrame(...)</c>.
    /// </summary>
    public static class TicketUnityDelay
    {
        /// <summary>
        /// Awaits Unity's native end-of-frame signal via <see cref="Awaitable.EndOfFrameAsync"/>.
        /// Prefer this overload when a <see cref="MonoBehaviour"/> coroutine runner is not available.
        /// </summary>
        public static async Ticket WaitForEndOfFrame(CancellationToken cancellationToken = default)
        {
            await Awaitable.EndOfFrameAsync(cancellationToken);
        }

        /// <summary>
        /// Awaits end-of-frame by starting a coroutine on the supplied <paramref name="coroutineRunner"/>.
        /// Provides strict end-of-frame semantics (after all rendering, before present).
        /// </summary>
        public static Ticket WaitForEndOfFrame(MonoBehaviour coroutineRunner)
        {
            var source = WaitForEndOfFramePromise.Create(coroutineRunner, CancellationToken.None, false, out var token);
            return new Ticket(source, token);
        }

        /// <summary>
        /// Awaits end-of-frame by starting a coroutine on the supplied <paramref name="coroutineRunner"/>, with cancellation support.
        /// </summary>
        public static Ticket WaitForEndOfFrame(MonoBehaviour coroutineRunner, CancellationToken cancellationToken, bool cancelImmediately = false)
        {
            var source = WaitForEndOfFramePromise.Create(coroutineRunner, cancellationToken, cancelImmediately, out var token);
            return new Ticket(source, token);
        }
    }

    internal sealed class WaitForEndOfFramePromise : ITicketSource, ITaskPoolNode<WaitForEndOfFramePromise>, System.Collections.IEnumerator
    {
        static TaskPool<WaitForEndOfFramePromise> pool;
        WaitForEndOfFramePromise nextNode;
        public ref WaitForEndOfFramePromise NextNode => ref nextNode;

        static WaitForEndOfFramePromise()
        {
            TaskPool.RegisterSizeGetter(typeof(WaitForEndOfFramePromise), () => pool.Size);
        }

        TicketCompletionSourceCore<object> core;
        CancellationToken cancellationToken;
        CancellationTokenRegistration cancellationTokenRegistration;
        bool cancelImmediately;

        WaitForEndOfFramePromise()
        {
        }

        public static ITicketSource Create(MonoBehaviour coroutineRunner, CancellationToken cancellationToken, bool cancelImmediately, out short token)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return AutoResetTicketCompletionSource.CreateFromCanceled(cancellationToken, out token);
            }

            if (!pool.TryPop(out var result))
            {
                result = new WaitForEndOfFramePromise();
            }

            result.cancellationToken = cancellationToken;
            result.cancelImmediately = cancelImmediately;

            if (cancelImmediately && cancellationToken.CanBeCanceled)
            {
                result.cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(state =>
                {
                    var promise = (WaitForEndOfFramePromise)state;
                    promise.core.TrySetCanceled(promise.cancellationToken);
                }, result);
            }

            TaskTracker.TrackActiveTask(result, 3);

            coroutineRunner.StartCoroutine(result);

            token = result.core.Version;
            return result;
        }

        public void GetResult(short token)
        {
            try
            {
                core.GetResult(token);
            }
            finally
            {
                if (!(cancelImmediately && cancellationToken.IsCancellationRequested))
                {
                    TryReturn();
                }
                else
                {
                    TaskTracker.RemoveTracking(this);
                }
            }
        }

        public TicketStatus GetStatus(short token)
        {
            return core.GetStatus(token);
        }

        public TicketStatus UnsafeGetStatus()
        {
            return core.UnsafeGetStatus();
        }

        public void OnCompleted(Action<object> continuation, object state, short token)
        {
            core.OnCompleted(continuation, state, token);
        }

        bool TryReturn()
        {
            TaskTracker.RemoveTracking(this);
            core.Reset();
            Reset(); // Reset Enumerator
            cancellationToken = default;
            cancellationTokenRegistration.Dispose();
            return pool.TryPush(this);
        }

        // Coroutine Runner implementation

        static readonly WaitForEndOfFrame waitForEndOfFrameYieldInstruction = new WaitForEndOfFrame();
        bool isFirst = true;

        object System.Collections.IEnumerator.Current => waitForEndOfFrameYieldInstruction;

        bool System.Collections.IEnumerator.MoveNext()
        {
            if (isFirst)
            {
                isFirst = false;
                return true; // start WaitForEndOfFrame
            }

            if (cancellationToken.IsCancellationRequested)
            {
                core.TrySetCanceled(cancellationToken);
                return false;
            }

            core.TrySetResult(null);
            return false;
        }

        public void Reset()
        {
            isFirst = true;
        }
    }
}
