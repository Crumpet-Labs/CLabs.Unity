#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets
{
    public partial struct Ticket
    {
        public static Ticket<(bool hasResultLeft, T result)> WhenAny<T>(Ticket<T> leftTask, Ticket rightTask)
        {
            return new Ticket<(bool, T)>(new WhenAnyLRPromise<T>(leftTask, rightTask), 0);
        }

        public static Ticket<(int winArgumentIndex, T result)> WhenAny<T>(params Ticket<T>[] tasks)
        {
            return new Ticket<(int, T)>(new WhenAnyPromise<T>(tasks, tasks.Length), 0);
        }

        public static Ticket<(int winArgumentIndex, T result)> WhenAny<T>(IEnumerable<Ticket<T>> tasks)
        {
            using (var span = ArrayPoolUtil.Materialize(tasks))
            {
                return new Ticket<(int, T)>(new WhenAnyPromise<T>(span.Array, span.Length), 0);
            }
        }

        /// <summary>Return value is winArgumentIndex</summary>
        public static Ticket<int> WhenAny(params Ticket[] tasks)
        {
            return new Ticket<int>(new WhenAnyPromise(tasks, tasks.Length), 0);
        }

        /// <summary>Return value is winArgumentIndex</summary>
        public static Ticket<int> WhenAny(IEnumerable<Ticket> tasks)
        {
            using (var span = ArrayPoolUtil.Materialize(tasks))
            {
                return new Ticket<int>(new WhenAnyPromise(span.Array, span.Length), 0);
            }
        }

        sealed class WhenAnyLRPromise<T> : ITicketSource<(bool, T)>
        {
            int completedCount;
            TicketCompletionSourceCore<(bool, T)> core;

            public WhenAnyLRPromise(Ticket<T> leftTask, Ticket rightTask)
            {
                TaskTracker.TrackActiveTask(this, 3);

                {
                    Ticket<T>.Awaiter awaiter;
                    try
                    {
                        awaiter = leftTask.GetAwaiter();
                    }
                    catch (Exception ex)
                    {
                        core.TrySetException(ex);
                        goto RIGHT;
                    }

                    if (awaiter.IsCompleted)
                    {
                        TryLeftInvokeContinuation(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAnyLRPromise<T>, Ticket<T>.Awaiter>)state)
                            {
                                TryLeftInvokeContinuation(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                RIGHT:
                {
                    Ticket.Awaiter awaiter;
                    try
                    {
                        awaiter = rightTask.GetAwaiter();
                    }
                    catch (Exception ex)
                    {
                        core.TrySetException(ex);
                        return;
                    }

                    if (awaiter.IsCompleted)
                    {
                        TryRightInvokeContinuation(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAnyLRPromise<T>, Ticket.Awaiter>)state)
                            {
                                TryRightInvokeContinuation(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
            }

            static void TryLeftInvokeContinuation(WhenAnyLRPromise<T> self, in Ticket<T>.Awaiter awaiter)
            {
                T result;
                try
                {
                    result = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }

                if (Interlocked.Increment(ref self.completedCount) == 1)
                {
                    self.core.TrySetResult((true, result));
                }
            }

            static void TryRightInvokeContinuation(WhenAnyLRPromise<T> self, in Ticket.Awaiter awaiter)
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

                if (Interlocked.Increment(ref self.completedCount) == 1)
                {
                    self.core.TrySetResult((false, default));
                }
            }

            public (bool, T) GetResult(short token)
            {
                TaskTracker.RemoveTracking(this);
                GC.SuppressFinalize(this);
                return core.GetResult(token);
            }

            public TicketStatus GetStatus(short token)
            {
                return core.GetStatus(token);
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                core.OnCompleted(continuation, state, token);
            }

            public TicketStatus UnsafeGetStatus()
            {
                return core.UnsafeGetStatus();
            }

            void ITicketSource.GetResult(short token)
            {
                GetResult(token);
            }
        }


        sealed class WhenAnyPromise<T> : ITicketSource<(int, T)>
        {
            int completedCount;
            TicketCompletionSourceCore<(int, T)> core;

            public WhenAnyPromise(Ticket<T>[] tasks, int tasksLength)
            {
                if (tasksLength == 0)
                {
                    throw new ArgumentException("The tasks argument contains no tasks.");
                }

                TaskTracker.TrackActiveTask(this, 3);

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
                        continue; // consume others.
                    }

                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuation(this, awaiter, i);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAnyPromise<T>, Ticket<T>.Awaiter, int>)state)
                            {
                                TryInvokeContinuation(t.Item1, t.Item2, t.Item3);
                            }
                        }, StateTuple.Create(this, awaiter, i));
                    }
                }
            }

            static void TryInvokeContinuation(WhenAnyPromise<T> self, in Ticket<T>.Awaiter awaiter, int i)
            {
                T result;
                try
                {
                    result = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }

                if (Interlocked.Increment(ref self.completedCount) == 1)
                {
                    self.core.TrySetResult((i, result));
                }
            }

            public (int, T) GetResult(short token)
            {
                TaskTracker.RemoveTracking(this);
                GC.SuppressFinalize(this);
                return core.GetResult(token);
            }

            public TicketStatus GetStatus(short token)
            {
                return core.GetStatus(token);
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                core.OnCompleted(continuation, state, token);
            }

            public TicketStatus UnsafeGetStatus()
            {
                return core.UnsafeGetStatus();
            }

            void ITicketSource.GetResult(short token)
            {
                GetResult(token);
            }
        }

        sealed class WhenAnyPromise : ITicketSource<int>
        {
            int completedCount;
            TicketCompletionSourceCore<int> core;

            public WhenAnyPromise(Ticket[] tasks, int tasksLength)
            {
                if (tasksLength == 0)
                {
                    throw new ArgumentException("The tasks argument contains no tasks.");
                }

                TaskTracker.TrackActiveTask(this, 3);

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
                        continue; // consume others.
                    }

                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuation(this, awaiter, i);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAnyPromise, Ticket.Awaiter, int>)state)
                            {
                                TryInvokeContinuation(t.Item1, t.Item2, t.Item3);
                            }
                        }, StateTuple.Create(this, awaiter, i));
                    }
                }
            }

            static void TryInvokeContinuation(WhenAnyPromise self, in Ticket.Awaiter awaiter, int i)
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

                if (Interlocked.Increment(ref self.completedCount) == 1)
                {
                    self.core.TrySetResult(i);
                }
            }

            public int GetResult(short token)
            {
                TaskTracker.RemoveTracking(this);
                GC.SuppressFinalize(this);
                return core.GetResult(token);
            }

            public TicketStatus GetStatus(short token)
            {
                return core.GetStatus(token);
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                core.OnCompleted(continuation, state, token);
            }

            public TicketStatus UnsafeGetStatus()
            {
                return core.UnsafeGetStatus();
            }

            void ITicketSource.GetResult(short token)
            {
                GetResult(token);
            }
        }
    }
}

