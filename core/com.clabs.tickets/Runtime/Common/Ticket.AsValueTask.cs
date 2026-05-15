#pragma warning disable 0649

#if Ticket_NETCORE || UNITY_2022_3_OR_NEWER
#define SUPPORT_VALUETASK
#endif

#if SUPPORT_VALUETASK

using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace CLabs.Tickets
{
    public static class TicketValueTaskExtensions
    {
        public static ValueTask AsValueTask(this in Ticket task)
        {
#if (Ticket_NETCORE && NETSTANDARD2_0)
            return new ValueTask(new TicketValueTaskSource(task), 0);
#else
            return task;
#endif
        }

        public static ValueTask<T> AsValueTask<T>(this in Ticket<T> task)
        {
#if (Ticket_NETCORE && NETSTANDARD2_0)
            return new ValueTask<T>(new TicketValueTaskSource<T>(task), 0);
#else
            return task;
#endif
        }

        public static async Ticket<T> AsTicket<T>(this ValueTask<T> task)
        {
            return await task;
        }

        public static async Ticket AsTicket(this ValueTask task)
        {
            await task;
        }

#if (Ticket_NETCORE && NETSTANDARD2_0)

        class TicketValueTaskSource : IValueTaskSource
        {
            readonly Ticket task;
            readonly Ticket.Awaiter awaiter;

            public TicketValueTaskSource(Ticket task)
            {
                this.task = task;
                this.awaiter = task.GetAwaiter();
            }

            public void GetResult(short token)
            {
                awaiter.GetResult();
            }

            public ValueTaskSourceStatus GetStatus(short token)
            {
                return (ValueTaskSourceStatus)task.Status;
            }

            public void OnCompleted(Action<object> continuation, object state, short token, ValueTaskSourceOnCompletedFlags flags)
            {
                awaiter.SourceOnCompleted(continuation, state);
            }
        }

        class TicketValueTaskSource<T> : IValueTaskSource<T>
        {
            readonly Ticket<T> task;
            readonly Ticket<T>.Awaiter awaiter;

            public TicketValueTaskSource(Ticket<T> task)
            {
                this.task = task;
                this.awaiter = task.GetAwaiter();
            }

            public T GetResult(short token)
            {
                return awaiter.GetResult();
            }

            public ValueTaskSourceStatus GetStatus(short token)
            {
                return (ValueTaskSourceStatus)task.Status;
            }

            public void OnCompleted(Action<object> continuation, object state, short token, ValueTaskSourceOnCompletedFlags flags)
            {
                awaiter.SourceOnCompleted(continuation, state);
            }
        }

#endif
    }
}
#endif
