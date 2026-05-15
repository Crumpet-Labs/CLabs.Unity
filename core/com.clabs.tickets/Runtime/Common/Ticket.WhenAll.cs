#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets
{
    public partial struct Ticket
    {
        public static Ticket<T[]> WhenAll<T>(params Ticket<T>[] tasks)
        {
            if (tasks.Length == 0)
            {
                return Ticket.FromResult(Array.Empty<T>());
            }

            return new Ticket<T[]>(new WhenAllPromise<T>(tasks, tasks.Length), 0);
        }

        public static Ticket<T[]> WhenAll<T>(IEnumerable<Ticket<T>> tasks)
        {
            using (var span = ArrayPoolUtil.Materialize(tasks))
            {
                var promise = new WhenAllPromise<T>(span.Array, span.Length); // consumed array in constructor.
                return new Ticket<T[]>(promise, 0);
            }
        }

        public static Ticket WhenAll(params Ticket[] tasks)
        {
            if (tasks.Length == 0)
            {
                return Ticket.CompletedTask;
            }

            return new Ticket(new WhenAllPromise(tasks, tasks.Length), 0);
        }

        public static Ticket WhenAll(IEnumerable<Ticket> tasks)
        {
            using (var span = ArrayPoolUtil.Materialize(tasks))
            {
                var promise = new WhenAllPromise(span.Array, span.Length); // consumed array in constructor.
                return new Ticket(promise, 0);
            }
        }

        sealed class WhenAllPromise<T> : ITicketSource<T[]>
        {
            T[] result;
            int completeCount;
            TicketCompletionSourceCore<T[]> core; // don't reset(called after GetResult, will invoke TrySetException.)

            public WhenAllPromise(Ticket<T>[] tasks, int tasksLength)
            {
                TaskTracker.TrackActiveTask(this, 3);

                this.completeCount = 0;

                if (tasksLength == 0)
                {
                    this.result = Array.Empty<T>();
                    core.TrySetResult(result);
                    return;
                }

                this.result = new T[tasksLength];

                for (int i = 0; i < tasksLength; i++)
                {
                    Ticket<T>.Awaiter awaiter;
                    try
                    {
                        awaiter = tasks[i].GetAwaiter();
                    }
                    catch (Exception ex)
                    {
                        core.TrySetException(ex);
                        continue;
                    }

                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuation(this, awaiter, i);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T>, Ticket<T>.Awaiter, int>)state)
                            {
                                TryInvokeContinuation(t.Item1, t.Item2, t.Item3);
                            }
                        }, StateTuple.Create(this, awaiter, i));
                    }
                }
            }

            static void TryInvokeContinuation(WhenAllPromise<T> self, in Ticket<T>.Awaiter awaiter, int i)
            {
                try
                {
                    self.result[i] = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }

                if (Interlocked.Increment(ref self.completeCount) == self.result.Length)
                {
                    self.core.TrySetResult(self.result);
                }
            }

            public T[] GetResult(short token)
            {
                TaskTracker.RemoveTracking(this);
                GC.SuppressFinalize(this);
                return core.GetResult(token);
            }

            void ITicketSource.GetResult(short token)
            {
                GetResult(token);
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
        }

        sealed class WhenAllPromise : ITicketSource
        {
            int completeCount;
            int tasksLength;
            TicketCompletionSourceCore<AsyncUnit> core; // don't reset(called after GetResult, will invoke TrySetException.)

            public WhenAllPromise(Ticket[] tasks, int tasksLength)
            {
                TaskTracker.TrackActiveTask(this, 3);

                this.tasksLength = tasksLength;
                this.completeCount = 0;

                if (tasksLength == 0)
                {
                    core.TrySetResult(AsyncUnit.Default);
                    return;
                }

                for (int i = 0; i < tasksLength; i++)
                {
                    Ticket.Awaiter awaiter;
                    try
                    {
                        awaiter = tasks[i].GetAwaiter();
                    }
                    catch (Exception ex)
                    {
                        core.TrySetException(ex);
                        continue;
                    }

                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuation(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise, Ticket.Awaiter>)state)
                            {
                                TryInvokeContinuation(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
            }

            static void TryInvokeContinuation(WhenAllPromise self, in Ticket.Awaiter awaiter)
            {
                try
                {
                    awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }

                if (Interlocked.Increment(ref self.completeCount) == self.tasksLength)
                {
                    self.core.TrySetResult(AsyncUnit.Default);
                }
            }

            public void GetResult(short token)
            {
                TaskTracker.RemoveTracking(this);
                GC.SuppressFinalize(this);
                core.GetResult(token);
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
        }
    }
}
