#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading;
using UnityEngine.Events;

namespace CLabs.Tickets
{
    /// <summary>
    /// Unity-specific factory overloads for creating <see cref="UnityAction"/>
    /// adapters around async TicketVoid methods. Split out of Ticket.Factory.cs
    /// in Phase B engine separation: partial classes cannot span assemblies, so
    /// these overloads moved from <c>Ticket.UnityAction(...)</c> in core to
    /// <c>TicketUnityActions.UnityAction(...)</c> in the Unity adapter.
    /// Will be folded back under a unified Ticket API in Phase D's rebrand pass.
    /// </summary>
    public static class TicketUnityActions
    {
        /// <summary>
        /// Create async void(TicketVoid) UnityAction.
        /// For example: onClick.AddListener(TicketUnityActions.UnityAction(async () => { /* */ } ))
        /// </summary>
        public static UnityAction UnityAction(Func<TicketVoid> asyncAction)
        {
            return () => asyncAction().Forget();
        }

        /// <summary>
        /// Create async void(TicketVoid) UnityAction.
        /// For example: onClick.AddListener(TicketUnityActions.UnityAction(FooAsync, this.GetCancellationTokenOnDestroy()))
        /// </summary>
        public static UnityAction UnityAction(Func<CancellationToken, TicketVoid> asyncAction, CancellationToken cancellationToken)
        {
            return () => asyncAction(cancellationToken).Forget();
        }

        /// <summary>
        /// Create async void(TicketVoid) UnityAction.
        /// For example: onClick.AddListener(TicketUnityActions.UnityAction(FooAsync, Argument))
        /// </summary>
        public static UnityAction UnityAction<T>(T state, Func<T, TicketVoid> asyncAction)
        {
            return () => asyncAction(state).Forget();
        }

        public static UnityAction<T> UnityAction<T>(Func<T, TicketVoid> asyncAction)
        {
            return (arg) => asyncAction(arg).Forget();
        }

        public static UnityAction<T0, T1> UnityAction<T0, T1>(Func<T0, T1, TicketVoid> asyncAction)
        {
            return (arg0, arg1) => asyncAction(arg0, arg1).Forget();
        }

        public static UnityAction<T0, T1, T2> UnityAction<T0, T1, T2>(Func<T0, T1, T2, TicketVoid> asyncAction)
        {
            return (arg0, arg1, arg2) => asyncAction(arg0, arg1, arg2).Forget();
        }

        public static UnityAction<T0, T1, T2, T3> UnityAction<T0, T1, T2, T3>(Func<T0, T1, T2, T3, TicketVoid> asyncAction)
        {
            return (arg0, arg1, arg2, arg3) => asyncAction(arg0, arg1, arg2, arg3).Forget();
        }

        public static UnityAction<T> UnityAction<T>(Func<T, CancellationToken, TicketVoid> asyncAction, CancellationToken cancellationToken)
        {
            return (arg) => asyncAction(arg, cancellationToken).Forget();
        }

        public static UnityAction<T0, T1> UnityAction<T0, T1>(Func<T0, T1, CancellationToken, TicketVoid> asyncAction, CancellationToken cancellationToken)
        {
            return (arg0, arg1) => asyncAction(arg0, arg1, cancellationToken).Forget();
        }

        public static UnityAction<T0, T1, T2> UnityAction<T0, T1, T2>(Func<T0, T1, T2, CancellationToken, TicketVoid> asyncAction, CancellationToken cancellationToken)
        {
            return (arg0, arg1, arg2) => asyncAction(arg0, arg1, arg2, cancellationToken).Forget();
        }

        public static UnityAction<T0, T1, T2, T3> UnityAction<T0, T1, T2, T3>(Func<T0, T1, T2, T3, CancellationToken, TicketVoid> asyncAction, CancellationToken cancellationToken)
        {
            return (arg0, arg1, arg2, arg3) => asyncAction(arg0, arg1, arg2, arg3, cancellationToken).Forget();
        }
    }
}
