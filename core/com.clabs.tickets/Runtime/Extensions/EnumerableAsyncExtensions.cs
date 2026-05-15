#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;

namespace CLabs.Tickets
{
    public static class EnumerableAsyncExtensions
    {
        // overload resolver - .Select(async x => { }) : IEnumerable<Ticket<T>>

        public static IEnumerable<Ticket> Select<T>(this IEnumerable<T> source, Func<T, Ticket> selector)
        {
            return System.Linq.Enumerable.Select(source, selector);
        }

        public static IEnumerable<Ticket<TR>> Select<T, TR>(this IEnumerable<T> source, Func<T, Ticket<TR>> selector)
        {
            return System.Linq.Enumerable.Select(source, selector);
        }

        public static IEnumerable<Ticket> Select<T>(this IEnumerable<T> source, Func<T, int, Ticket> selector)
        {
            return System.Linq.Enumerable.Select(source, selector);
        }

        public static IEnumerable<Ticket<TR>> Select<T, TR>(this IEnumerable<T> source, Func<T, int, Ticket<TR>> selector)
        {
            return System.Linq.Enumerable.Select(source, selector);
        }
    }
}


