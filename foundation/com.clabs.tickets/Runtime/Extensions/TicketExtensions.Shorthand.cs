#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System.Collections.Generic;

namespace CLabs.Tickets
{
    public static partial class TicketExtensions
    {
        // shorthand of WhenAll
    
        public static Ticket.Awaiter GetAwaiter(this Ticket[] tasks)
        {
            return Ticket.WhenAll(tasks).GetAwaiter();
        }

        public static Ticket.Awaiter GetAwaiter(this IEnumerable<Ticket> tasks)
        {
            return Ticket.WhenAll(tasks).GetAwaiter();
        }

        public static Ticket<T[]>.Awaiter GetAwaiter<T>(this Ticket<T>[] tasks)
        {
            return Ticket.WhenAll(tasks).GetAwaiter();
        }

        public static Ticket<T[]>.Awaiter GetAwaiter<T>(this IEnumerable<Ticket<T>> tasks)
        {
            return Ticket.WhenAll(tasks).GetAwaiter();
        }

        public static Ticket<(T1, T2)>.Awaiter GetAwaiter<T1, T2>(this (Ticket<T1> task1, Ticket<T2> task2) tasks)
        {
            return Ticket.WhenAll(tasks.Item1, tasks.Item2).GetAwaiter();
        }

        public static Ticket<(T1, T2, T3)>.Awaiter GetAwaiter<T1, T2, T3>(this (Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3) tasks)
        {
            return Ticket.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3).GetAwaiter();
        }

        public static Ticket<(T1, T2, T3, T4)>.Awaiter GetAwaiter<T1, T2, T3, T4>(this (Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4) tasks)
        {
            return Ticket.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4).GetAwaiter();
        }

        public static Ticket<(T1, T2, T3, T4, T5)>.Awaiter GetAwaiter<T1, T2, T3, T4, T5>(this (Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4, Ticket<T5> task5) tasks)
        {
            return Ticket.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5).GetAwaiter();
        }

        public static Ticket<(T1, T2, T3, T4, T5, T6)>.Awaiter GetAwaiter<T1, T2, T3, T4, T5, T6>(this (Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4, Ticket<T5> task5, Ticket<T6> task6) tasks)
        {
            return Ticket.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6).GetAwaiter();
        }

        public static Ticket<(T1, T2, T3, T4, T5, T6, T7)>.Awaiter GetAwaiter<T1, T2, T3, T4, T5, T6, T7>(this (Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4, Ticket<T5> task5, Ticket<T6> task6, Ticket<T7> task7) tasks)
        {
            return Ticket.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7).GetAwaiter();
        }

        public static Ticket<(T1, T2, T3, T4, T5, T6, T7, T8)>.Awaiter GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8>(this (Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4, Ticket<T5> task5, Ticket<T6> task6, Ticket<T7> task7, Ticket<T8> task8) tasks)
        {
            return Ticket.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8).GetAwaiter();
        }

        public static Ticket<(T1, T2, T3, T4, T5, T6, T7, T8, T9)>.Awaiter GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this (Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4, Ticket<T5> task5, Ticket<T6> task6, Ticket<T7> task7, Ticket<T8> task8, Ticket<T9> task9) tasks)
        {
            return Ticket.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9).GetAwaiter();
        }

        public static Ticket<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10)>.Awaiter GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this (Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4, Ticket<T5> task5, Ticket<T6> task6, Ticket<T7> task7, Ticket<T8> task8, Ticket<T9> task9, Ticket<T10> task10) tasks)
        {
            return Ticket.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10).GetAwaiter();
        }

        public static Ticket<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)>.Awaiter GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this (Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4, Ticket<T5> task5, Ticket<T6> task6, Ticket<T7> task7, Ticket<T8> task8, Ticket<T9> task9, Ticket<T10> task10, Ticket<T11> task11) tasks)
        {
            return Ticket.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11).GetAwaiter();
        }

        public static Ticket<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12)>.Awaiter GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this (Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4, Ticket<T5> task5, Ticket<T6> task6, Ticket<T7> task7, Ticket<T8> task8, Ticket<T9> task9, Ticket<T10> task10, Ticket<T11> task11, Ticket<T12> task12) tasks)
        {
            return Ticket.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12).GetAwaiter();
        }

        public static Ticket<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13)>.Awaiter GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this (Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4, Ticket<T5> task5, Ticket<T6> task6, Ticket<T7> task7, Ticket<T8> task8, Ticket<T9> task9, Ticket<T10> task10, Ticket<T11> task11, Ticket<T12> task12, Ticket<T13> task13) tasks)
        {
            return Ticket.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13).GetAwaiter();
        }

        public static Ticket<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14)>.Awaiter GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this (Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4, Ticket<T5> task5, Ticket<T6> task6, Ticket<T7> task7, Ticket<T8> task8, Ticket<T9> task9, Ticket<T10> task10, Ticket<T11> task11, Ticket<T12> task12, Ticket<T13> task13, Ticket<T14> task14) tasks)
        {
            return Ticket.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13, tasks.Item14).GetAwaiter();
        }

        public static Ticket<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15)>.Awaiter GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this (Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4, Ticket<T5> task5, Ticket<T6> task6, Ticket<T7> task7, Ticket<T8> task8, Ticket<T9> task9, Ticket<T10> task10, Ticket<T11> task11, Ticket<T12> task12, Ticket<T13> task13, Ticket<T14> task14, Ticket<T15> task15) tasks)
        {
            return Ticket.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13, tasks.Item14, tasks.Item15).GetAwaiter();
        }



        public static Ticket.Awaiter GetAwaiter(this (Ticket task1, Ticket task2) tasks)
        {
            return Ticket.WhenAll(tasks.Item1, tasks.Item2).GetAwaiter();
        }


        public static Ticket.Awaiter GetAwaiter(this (Ticket task1, Ticket task2, Ticket task3) tasks)
        {
            return Ticket.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3).GetAwaiter();
        }


        public static Ticket.Awaiter GetAwaiter(this (Ticket task1, Ticket task2, Ticket task3, Ticket task4) tasks)
        {
            return Ticket.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4).GetAwaiter();
        }


        public static Ticket.Awaiter GetAwaiter(this (Ticket task1, Ticket task2, Ticket task3, Ticket task4, Ticket task5) tasks)
        {
            return Ticket.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5).GetAwaiter();
        }


        public static Ticket.Awaiter GetAwaiter(this (Ticket task1, Ticket task2, Ticket task3, Ticket task4, Ticket task5, Ticket task6) tasks)
        {
            return Ticket.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6).GetAwaiter();
        }


        public static Ticket.Awaiter GetAwaiter(this (Ticket task1, Ticket task2, Ticket task3, Ticket task4, Ticket task5, Ticket task6, Ticket task7) tasks)
        {
            return Ticket.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7).GetAwaiter();
        }


        public static Ticket.Awaiter GetAwaiter(this (Ticket task1, Ticket task2, Ticket task3, Ticket task4, Ticket task5, Ticket task6, Ticket task7, Ticket task8) tasks)
        {
            return Ticket.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8).GetAwaiter();
        }


        public static Ticket.Awaiter GetAwaiter(this (Ticket task1, Ticket task2, Ticket task3, Ticket task4, Ticket task5, Ticket task6, Ticket task7, Ticket task8, Ticket task9) tasks)
        {
            return Ticket.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9).GetAwaiter();
        }


        public static Ticket.Awaiter GetAwaiter(this (Ticket task1, Ticket task2, Ticket task3, Ticket task4, Ticket task5, Ticket task6, Ticket task7, Ticket task8, Ticket task9, Ticket task10) tasks)
        {
            return Ticket.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10).GetAwaiter();
        }


        public static Ticket.Awaiter GetAwaiter(this (Ticket task1, Ticket task2, Ticket task3, Ticket task4, Ticket task5, Ticket task6, Ticket task7, Ticket task8, Ticket task9, Ticket task10, Ticket task11) tasks)
        {
            return Ticket.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11).GetAwaiter();
        }


        public static Ticket.Awaiter GetAwaiter(this (Ticket task1, Ticket task2, Ticket task3, Ticket task4, Ticket task5, Ticket task6, Ticket task7, Ticket task8, Ticket task9, Ticket task10, Ticket task11, Ticket task12) tasks)
        {
            return Ticket.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12).GetAwaiter();
        }


        public static Ticket.Awaiter GetAwaiter(this (Ticket task1, Ticket task2, Ticket task3, Ticket task4, Ticket task5, Ticket task6, Ticket task7, Ticket task8, Ticket task9, Ticket task10, Ticket task11, Ticket task12, Ticket task13) tasks)
        {
            return Ticket.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13).GetAwaiter();
        }


        public static Ticket.Awaiter GetAwaiter(this (Ticket task1, Ticket task2, Ticket task3, Ticket task4, Ticket task5, Ticket task6, Ticket task7, Ticket task8, Ticket task9, Ticket task10, Ticket task11, Ticket task12, Ticket task13, Ticket task14) tasks)
        {
            return Ticket.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13, tasks.Item14).GetAwaiter();
        }


        public static Ticket.Awaiter GetAwaiter(this (Ticket task1, Ticket task2, Ticket task3, Ticket task4, Ticket task5, Ticket task6, Ticket task7, Ticket task8, Ticket task9, Ticket task10, Ticket task11, Ticket task12, Ticket task13, Ticket task14, Ticket task15) tasks)
        {
            return Ticket.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13, tasks.Item14, tasks.Item15).GetAwaiter();
        }


    }
}