using System;
using System.Collections.Generic;

namespace CLabs.Tickets.Internal
{
    /// <summary>
    /// Factory-driven default equality comparer used by tickets APIs that
    /// need to compare values: <see cref="Progress.CreateOnlyValueChanged{T}"/>,
    /// <c>Ticket.WaitUntilValueChanged</c>, and <c>EveryValueChanged</c>.
    ///
    /// This package is engine-agnostic, so it can't reference the Unity-only
    /// <c>UnityEqualityComparer</c> (which provides float-aware equality for
    /// <c>Vector3</c>, <c>Color</c>, <c>Quaternion</c>, etc.). Instead, the
    /// Unity adapter installs an override factory on startup via
    /// <see cref="SetOverride"/>. When no override is installed (e.g. another
    /// engine, or the Unity adapter hasn't initialised yet), we fall back to
    /// <see cref="EqualityComparer{T}.Default"/>, which still does the right
    /// thing for most types; only Unity math structs get bitwise instead of
    /// fuzzy comparison.
    /// </summary>
    public static class TicketEqualityComparer
    {
        static Func<Type, object> s_override;

        /// <summary>
        /// Install an override factory. Typically called once at process
        /// start by the Unity adapter (via <c>RuntimeInitializeOnLoadMethod</c>).
        /// Pass <c>null</c> to clear.
        ///
        /// The factory receives the requested type and returns either an
        /// <see cref="IEqualityComparer{T}"/> cast to <see cref="object"/>,
        /// or <c>null</c> to defer to <see cref="EqualityComparer{T}.Default"/>.
        /// </summary>
        public static void SetOverride(Func<Type, object> factory)
        {
            s_override = factory;
        }

        /// <summary>
        /// Resolve the default comparer for <typeparamref name="T"/>. If an
        /// override factory is installed and returns a comparer for this
        /// type, it wins; otherwise returns <see cref="EqualityComparer{T}.Default"/>.
        /// </summary>
        public static IEqualityComparer<T> GetDefault<T>()
        {
            var @override = s_override;
            if (@override != null)
            {
                if (@override(typeof(T)) is IEqualityComparer<T> typed)
                {
                    return typed;
                }
            }
            return EqualityComparer<T>.Default;
        }
    }
}
