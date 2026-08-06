#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading;

namespace CLabs.Tickets
{
    // Ticket has no scheduler like TaskScheduler.
    // Only handle unobserved exception.

    public static class TicketScheduler
    {
        public static event Action<Exception> UnobservedTaskException;

        /// <summary>
        /// Propagate OperationCanceledException to UnobservedTaskException when true. Default is false.
        /// </summary>
        public static bool PropagateOperationCanceledException = false;

        /// <summary>
        /// Dispatch exception event to the engine main thread. Default is true.
        /// (Was named DispatchUnityMainThread upstream; kept for compatibility,
        /// will be renamed in Phase D's rebrand pass.)
        /// </summary>
        public static bool DispatchUnityMainThread = true;

        // cache delegate.
        static readonly SendOrPostCallback handleExceptionInvoke = InvokeUnobservedTaskException;

        static void InvokeUnobservedTaskException(object state)
        {
            UnobservedTaskException((Exception)state);
        }

        internal static void PublishUnobservedTaskException(Exception ex)
        {
            if (ex != null)
            {
                if (!PropagateOperationCanceledException && ex is OperationCanceledException)
                {
                    return;
                }

                if (UnobservedTaskException != null)
                {
                    if (!DispatchUnityMainThread || Thread.CurrentThread.ManagedThreadId == TicketRuntime.MainThreadId)
                    {
                        // allows inlining call.
                        UnobservedTaskException.Invoke(ex);
                    }
                    else
                    {
                        // Post to main thread via the engine-registered SynchronizationContext.
                        TicketRuntime.MainThreadSynchronizationContext.Post(handleExceptionInvoke, ex);
                    }
                }
                else
                {
                    // No handler registered. Upstream Ticket routed this through
                    // Unity's Debug.LogException with a configurable LogType
                    // dispatcher (UnobservedExceptionWriteLogType field). The
                    // engine-agnostic fallback uses System.Diagnostics.Trace;
                    // engine adapters can subscribe UnobservedTaskException to
                    // bridge into their native logger if desired.
                    System.Diagnostics.Trace.TraceError("UnobservedTaskException: " + ex.ToString());
                }
            }
        }
    }
}

