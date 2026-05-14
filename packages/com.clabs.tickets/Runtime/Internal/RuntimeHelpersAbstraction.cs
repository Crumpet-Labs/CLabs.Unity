#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;

namespace CLabs.Tickets.Internal
{
    internal static class RuntimeHelpersAbstraction
    {
        // Adapter-registered value types that are known to contain no managed
        // references (Unity's Vector2/3/4, Color, Rect, Bounds, Quaternion,
        // Godot's equivalents, etc.). Populated via TicketRuntime.
        // RegisterWellKnownNoReferenceType during adapter static init, BEFORE
        // any async code runs. The generic caching class below reads this set
        // inside its static constructor, which runs lazily on first access to
        // WellKnownNoReferenceContainsType<T>, so adapter-registered types
        // get picked up provided the adapter init ran first (which it does on
        // every supported engine: Unity BeforeSceneLoad, Godot autoload _Ready).
        internal static readonly HashSet<Type> AdditionalWellKnownTypes = new HashSet<Type>();

        // If we can use RuntimeHelpers.IsReferenceOrContainsReferences(.NET Core 2.0), use it.
        public static bool IsWellKnownNoReferenceContainsType<T>()
        {
            return WellKnownNoReferenceContainsType<T>.IsWellKnownType;
        }

        static bool WellKnownNoReferenceContainsTypeInitialize(Type t)
        {
            // The primitive types are Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, IntPtr, UIntPtr, Char, Double, and Single.
            if (t.IsPrimitive) return true;

            if (t.IsEnum) return true;
            if (t == typeof(DateTime)) return true;
            if (t == typeof(DateTimeOffset)) return true;
            if (t == typeof(Guid)) return true;
            if (t == typeof(decimal)) return true;

            // unwrap nullable
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return WellKnownNoReferenceContainsTypeInitialize(t.GetGenericArguments()[0]);
            }

            // Engine-registered value types (Vector2/3/4, Color, Rect, Bounds,
            // Quaternion, etc. on Unity) — populated by adapter init.
            if (AdditionalWellKnownTypes.Contains(t)) return true;

            return false;
        }

        static class WellKnownNoReferenceContainsType<T>
        {
            public static readonly bool IsWellKnownType;

            static WellKnownNoReferenceContainsType()
            {
                IsWellKnownType = WellKnownNoReferenceContainsTypeInitialize(typeof(T));
            }
        }
    }
}
