#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets
{
    // public for add user custom.

    public static class TaskTracker
    {
#if UNITY_EDITOR

        static int trackingId = 0;

        public const string EnableAutoReloadKey = "TicketTrackerWindow_EnableAutoReloadKey";
        public const string EnableTrackingKey = "TicketTrackerWindow_EnableTrackingKey";
        public const string EnableStackTraceKey = "TicketTrackerWindow_EnableStackTraceKey";

        // Plain in-memory flags. Persistence to EditorPrefs lives in the
        // adapter's TicketTrackerWindow (com.clabs.adapter.unity.ticket) so
        // core stays free of editor-only references.
        public static class EditorEnableState
        {
            public static bool EnableAutoReload;
            public static bool EnableTracking;
            public static bool EnableStackTrace;
        }

#endif


        static List<KeyValuePair<ITicketSource, (string formattedType, int trackingId, DateTime addTime, string stackTrace)>> listPool = new List<KeyValuePair<ITicketSource, (string formattedType, int trackingId, DateTime addTime, string stackTrace)>>();

        static readonly WeakDictionary<ITicketSource, (string formattedType, int trackingId, DateTime addTime, string stackTrace)> tracking = new WeakDictionary<ITicketSource, (string formattedType, int trackingId, DateTime addTime, string stackTrace)>();

        [Conditional("UNITY_EDITOR")]
        public static void TrackActiveTask(ITicketSource task, int skipFrame)
        {
#if UNITY_EDITOR
            dirty = true;
            if (!EditorEnableState.EnableTracking) return;
            var stackTrace = EditorEnableState.EnableStackTrace ? new StackTrace(skipFrame, true).CleanupAsyncStackTrace() : "";

            string typeName;
            if (EditorEnableState.EnableStackTrace)
            {
                var sb = new StringBuilder();
                TypeBeautify(task.GetType(), sb);
                typeName = sb.ToString();
            }
            else
            {
                typeName = task.GetType().Name;
            }
            tracking.TryAdd(task, (typeName, Interlocked.Increment(ref trackingId), DateTime.UtcNow, stackTrace));
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void RemoveTracking(ITicketSource task)
        {
#if UNITY_EDITOR
            dirty = true;
            if (!EditorEnableState.EnableTracking) return;
            var success = tracking.TryRemove(task);
#endif
        }

        static bool dirty;

        public static bool CheckAndResetDirty()
        {
            var current = dirty;
            dirty = false;
            return current;
        }

        /// <summary>(trackingId, awaiterType, awaiterStatus, createdTime, stackTrace)</summary>
        public static void ForEachActiveTask(Action<int, string, TicketStatus, DateTime, string> action)
        {
            lock (listPool)
            {
                var count = tracking.ToList(ref listPool, clear: false);
                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        action(listPool[i].Value.trackingId, listPool[i].Value.formattedType, listPool[i].Key.UnsafeGetStatus(), listPool[i].Value.addTime, listPool[i].Value.stackTrace);
                        listPool[i] = default;
                    }
                }
                catch
                {
                    listPool.Clear();
                    throw;
                }
            }
        }

        static void TypeBeautify(Type type, StringBuilder sb)
        {
            if (type.IsNested)
            {
                // TypeBeautify(type.DeclaringType, sb);
                sb.Append(type.DeclaringType.Name.ToString());
                sb.Append(".");
            }

            if (type.IsGenericType)
            {
                var genericsStart = type.Name.IndexOf("`");
                if (genericsStart != -1)
                {
                    sb.Append(type.Name.Substring(0, genericsStart));
                }
                else
                {
                    sb.Append(type.Name);
                }
                sb.Append("<");
                var first = true;
                foreach (var item in type.GetGenericArguments())
                {
                    if (!first)
                    {
                        sb.Append(", ");
                    }
                    first = false;
                    TypeBeautify(item, sb);
                }
                sb.Append(">");
            }
            else
            {
                sb.Append(type.Name);
            }
        }

        //static string RemoveTicketNamespace(string str)
        //{
        //    return str.Replace("CLabs.Tickets.CompilerServices", "")
        //        .Replace("CLabs.Tickets.Linq", "")
        //        .Replace("CLabs.Tickets", "");
        //}
    }
}

