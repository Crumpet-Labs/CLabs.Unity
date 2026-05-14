#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets
{
    public partial struct Ticket
    {
        
        public static Ticket<(T1, T2)> WhenAll<T1, T2>(Ticket<T1> task1, Ticket<T2> task2)
        {
            if (task1.Status.IsCompletedSuccessfully() && task2.Status.IsCompletedSuccessfully())
            {
                return new Ticket<(T1, T2)>((task1.GetAwaiter().GetResult(), task2.GetAwaiter().GetResult()));
            }

            return new Ticket<(T1, T2)>(new WhenAllPromise<T1, T2>(task1, task2), 0);
        }

        sealed class WhenAllPromise<T1, T2> : ITicketSource<(T1, T2)>
        {
            T1 t1 = default;
            T2 t2 = default;
            int completedCount;
            TicketCompletionSourceCore<(T1, T2)> core;

            public WhenAllPromise(Ticket<T1> task1, Ticket<T2> task2)
            {
                TaskTracker.TrackActiveTask(this, 3);

                this.completedCount = 0;
                {
                    var awaiter = task1.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT1(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2>, Ticket<T1>.Awaiter>)state)
                            {
                                TryInvokeContinuationT1(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task2.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT2(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2>, Ticket<T2>.Awaiter>)state)
                            {
                                TryInvokeContinuationT2(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
            }

            static void TryInvokeContinuationT1(WhenAllPromise<T1, T2> self, in Ticket<T1>.Awaiter awaiter)
            {
                try
                {
                    self.t1 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 2)
                {
                    self.core.TrySetResult((self.t1, self.t2));
                }
            }

            static void TryInvokeContinuationT2(WhenAllPromise<T1, T2> self, in Ticket<T2>.Awaiter awaiter)
            {
                try
                {
                    self.t2 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 2)
                {
                    self.core.TrySetResult((self.t1, self.t2));
                }
            }


            public (T1, T2) GetResult(short token)
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
        
        public static Ticket<(T1, T2, T3)> WhenAll<T1, T2, T3>(Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3)
        {
            if (task1.Status.IsCompletedSuccessfully() && task2.Status.IsCompletedSuccessfully() && task3.Status.IsCompletedSuccessfully())
            {
                return new Ticket<(T1, T2, T3)>((task1.GetAwaiter().GetResult(), task2.GetAwaiter().GetResult(), task3.GetAwaiter().GetResult()));
            }

            return new Ticket<(T1, T2, T3)>(new WhenAllPromise<T1, T2, T3>(task1, task2, task3), 0);
        }

        sealed class WhenAllPromise<T1, T2, T3> : ITicketSource<(T1, T2, T3)>
        {
            T1 t1 = default;
            T2 t2 = default;
            T3 t3 = default;
            int completedCount;
            TicketCompletionSourceCore<(T1, T2, T3)> core;

            public WhenAllPromise(Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3)
            {
                TaskTracker.TrackActiveTask(this, 3);

                this.completedCount = 0;
                {
                    var awaiter = task1.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT1(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3>, Ticket<T1>.Awaiter>)state)
                            {
                                TryInvokeContinuationT1(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task2.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT2(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3>, Ticket<T2>.Awaiter>)state)
                            {
                                TryInvokeContinuationT2(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task3.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT3(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3>, Ticket<T3>.Awaiter>)state)
                            {
                                TryInvokeContinuationT3(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
            }

            static void TryInvokeContinuationT1(WhenAllPromise<T1, T2, T3> self, in Ticket<T1>.Awaiter awaiter)
            {
                try
                {
                    self.t1 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 3)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3));
                }
            }

            static void TryInvokeContinuationT2(WhenAllPromise<T1, T2, T3> self, in Ticket<T2>.Awaiter awaiter)
            {
                try
                {
                    self.t2 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 3)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3));
                }
            }

            static void TryInvokeContinuationT3(WhenAllPromise<T1, T2, T3> self, in Ticket<T3>.Awaiter awaiter)
            {
                try
                {
                    self.t3 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 3)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3));
                }
            }


            public (T1, T2, T3) GetResult(short token)
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
        
        public static Ticket<(T1, T2, T3, T4)> WhenAll<T1, T2, T3, T4>(Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4)
        {
            if (task1.Status.IsCompletedSuccessfully() && task2.Status.IsCompletedSuccessfully() && task3.Status.IsCompletedSuccessfully() && task4.Status.IsCompletedSuccessfully())
            {
                return new Ticket<(T1, T2, T3, T4)>((task1.GetAwaiter().GetResult(), task2.GetAwaiter().GetResult(), task3.GetAwaiter().GetResult(), task4.GetAwaiter().GetResult()));
            }

            return new Ticket<(T1, T2, T3, T4)>(new WhenAllPromise<T1, T2, T3, T4>(task1, task2, task3, task4), 0);
        }

        sealed class WhenAllPromise<T1, T2, T3, T4> : ITicketSource<(T1, T2, T3, T4)>
        {
            T1 t1 = default;
            T2 t2 = default;
            T3 t3 = default;
            T4 t4 = default;
            int completedCount;
            TicketCompletionSourceCore<(T1, T2, T3, T4)> core;

            public WhenAllPromise(Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4)
            {
                TaskTracker.TrackActiveTask(this, 3);

                this.completedCount = 0;
                {
                    var awaiter = task1.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT1(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4>, Ticket<T1>.Awaiter>)state)
                            {
                                TryInvokeContinuationT1(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task2.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT2(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4>, Ticket<T2>.Awaiter>)state)
                            {
                                TryInvokeContinuationT2(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task3.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT3(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4>, Ticket<T3>.Awaiter>)state)
                            {
                                TryInvokeContinuationT3(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task4.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT4(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4>, Ticket<T4>.Awaiter>)state)
                            {
                                TryInvokeContinuationT4(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
            }

            static void TryInvokeContinuationT1(WhenAllPromise<T1, T2, T3, T4> self, in Ticket<T1>.Awaiter awaiter)
            {
                try
                {
                    self.t1 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 4)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4));
                }
            }

            static void TryInvokeContinuationT2(WhenAllPromise<T1, T2, T3, T4> self, in Ticket<T2>.Awaiter awaiter)
            {
                try
                {
                    self.t2 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 4)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4));
                }
            }

            static void TryInvokeContinuationT3(WhenAllPromise<T1, T2, T3, T4> self, in Ticket<T3>.Awaiter awaiter)
            {
                try
                {
                    self.t3 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 4)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4));
                }
            }

            static void TryInvokeContinuationT4(WhenAllPromise<T1, T2, T3, T4> self, in Ticket<T4>.Awaiter awaiter)
            {
                try
                {
                    self.t4 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 4)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4));
                }
            }


            public (T1, T2, T3, T4) GetResult(short token)
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
        
        public static Ticket<(T1, T2, T3, T4, T5)> WhenAll<T1, T2, T3, T4, T5>(Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4, Ticket<T5> task5)
        {
            if (task1.Status.IsCompletedSuccessfully() && task2.Status.IsCompletedSuccessfully() && task3.Status.IsCompletedSuccessfully() && task4.Status.IsCompletedSuccessfully() && task5.Status.IsCompletedSuccessfully())
            {
                return new Ticket<(T1, T2, T3, T4, T5)>((task1.GetAwaiter().GetResult(), task2.GetAwaiter().GetResult(), task3.GetAwaiter().GetResult(), task4.GetAwaiter().GetResult(), task5.GetAwaiter().GetResult()));
            }

            return new Ticket<(T1, T2, T3, T4, T5)>(new WhenAllPromise<T1, T2, T3, T4, T5>(task1, task2, task3, task4, task5), 0);
        }

        sealed class WhenAllPromise<T1, T2, T3, T4, T5> : ITicketSource<(T1, T2, T3, T4, T5)>
        {
            T1 t1 = default;
            T2 t2 = default;
            T3 t3 = default;
            T4 t4 = default;
            T5 t5 = default;
            int completedCount;
            TicketCompletionSourceCore<(T1, T2, T3, T4, T5)> core;

            public WhenAllPromise(Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4, Ticket<T5> task5)
            {
                TaskTracker.TrackActiveTask(this, 3);

                this.completedCount = 0;
                {
                    var awaiter = task1.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT1(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5>, Ticket<T1>.Awaiter>)state)
                            {
                                TryInvokeContinuationT1(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task2.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT2(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5>, Ticket<T2>.Awaiter>)state)
                            {
                                TryInvokeContinuationT2(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task3.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT3(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5>, Ticket<T3>.Awaiter>)state)
                            {
                                TryInvokeContinuationT3(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task4.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT4(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5>, Ticket<T4>.Awaiter>)state)
                            {
                                TryInvokeContinuationT4(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task5.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT5(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5>, Ticket<T5>.Awaiter>)state)
                            {
                                TryInvokeContinuationT5(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
            }

            static void TryInvokeContinuationT1(WhenAllPromise<T1, T2, T3, T4, T5> self, in Ticket<T1>.Awaiter awaiter)
            {
                try
                {
                    self.t1 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 5)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5));
                }
            }

            static void TryInvokeContinuationT2(WhenAllPromise<T1, T2, T3, T4, T5> self, in Ticket<T2>.Awaiter awaiter)
            {
                try
                {
                    self.t2 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 5)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5));
                }
            }

            static void TryInvokeContinuationT3(WhenAllPromise<T1, T2, T3, T4, T5> self, in Ticket<T3>.Awaiter awaiter)
            {
                try
                {
                    self.t3 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 5)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5));
                }
            }

            static void TryInvokeContinuationT4(WhenAllPromise<T1, T2, T3, T4, T5> self, in Ticket<T4>.Awaiter awaiter)
            {
                try
                {
                    self.t4 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 5)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5));
                }
            }

            static void TryInvokeContinuationT5(WhenAllPromise<T1, T2, T3, T4, T5> self, in Ticket<T5>.Awaiter awaiter)
            {
                try
                {
                    self.t5 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 5)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5));
                }
            }


            public (T1, T2, T3, T4, T5) GetResult(short token)
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
        
        public static Ticket<(T1, T2, T3, T4, T5, T6)> WhenAll<T1, T2, T3, T4, T5, T6>(Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4, Ticket<T5> task5, Ticket<T6> task6)
        {
            if (task1.Status.IsCompletedSuccessfully() && task2.Status.IsCompletedSuccessfully() && task3.Status.IsCompletedSuccessfully() && task4.Status.IsCompletedSuccessfully() && task5.Status.IsCompletedSuccessfully() && task6.Status.IsCompletedSuccessfully())
            {
                return new Ticket<(T1, T2, T3, T4, T5, T6)>((task1.GetAwaiter().GetResult(), task2.GetAwaiter().GetResult(), task3.GetAwaiter().GetResult(), task4.GetAwaiter().GetResult(), task5.GetAwaiter().GetResult(), task6.GetAwaiter().GetResult()));
            }

            return new Ticket<(T1, T2, T3, T4, T5, T6)>(new WhenAllPromise<T1, T2, T3, T4, T5, T6>(task1, task2, task3, task4, task5, task6), 0);
        }

        sealed class WhenAllPromise<T1, T2, T3, T4, T5, T6> : ITicketSource<(T1, T2, T3, T4, T5, T6)>
        {
            T1 t1 = default;
            T2 t2 = default;
            T3 t3 = default;
            T4 t4 = default;
            T5 t5 = default;
            T6 t6 = default;
            int completedCount;
            TicketCompletionSourceCore<(T1, T2, T3, T4, T5, T6)> core;

            public WhenAllPromise(Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4, Ticket<T5> task5, Ticket<T6> task6)
            {
                TaskTracker.TrackActiveTask(this, 3);

                this.completedCount = 0;
                {
                    var awaiter = task1.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT1(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6>, Ticket<T1>.Awaiter>)state)
                            {
                                TryInvokeContinuationT1(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task2.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT2(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6>, Ticket<T2>.Awaiter>)state)
                            {
                                TryInvokeContinuationT2(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task3.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT3(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6>, Ticket<T3>.Awaiter>)state)
                            {
                                TryInvokeContinuationT3(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task4.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT4(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6>, Ticket<T4>.Awaiter>)state)
                            {
                                TryInvokeContinuationT4(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task5.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT5(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6>, Ticket<T5>.Awaiter>)state)
                            {
                                TryInvokeContinuationT5(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task6.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT6(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6>, Ticket<T6>.Awaiter>)state)
                            {
                                TryInvokeContinuationT6(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
            }

            static void TryInvokeContinuationT1(WhenAllPromise<T1, T2, T3, T4, T5, T6> self, in Ticket<T1>.Awaiter awaiter)
            {
                try
                {
                    self.t1 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 6)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6));
                }
            }

            static void TryInvokeContinuationT2(WhenAllPromise<T1, T2, T3, T4, T5, T6> self, in Ticket<T2>.Awaiter awaiter)
            {
                try
                {
                    self.t2 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 6)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6));
                }
            }

            static void TryInvokeContinuationT3(WhenAllPromise<T1, T2, T3, T4, T5, T6> self, in Ticket<T3>.Awaiter awaiter)
            {
                try
                {
                    self.t3 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 6)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6));
                }
            }

            static void TryInvokeContinuationT4(WhenAllPromise<T1, T2, T3, T4, T5, T6> self, in Ticket<T4>.Awaiter awaiter)
            {
                try
                {
                    self.t4 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 6)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6));
                }
            }

            static void TryInvokeContinuationT5(WhenAllPromise<T1, T2, T3, T4, T5, T6> self, in Ticket<T5>.Awaiter awaiter)
            {
                try
                {
                    self.t5 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 6)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6));
                }
            }

            static void TryInvokeContinuationT6(WhenAllPromise<T1, T2, T3, T4, T5, T6> self, in Ticket<T6>.Awaiter awaiter)
            {
                try
                {
                    self.t6 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 6)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6));
                }
            }


            public (T1, T2, T3, T4, T5, T6) GetResult(short token)
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
        
        public static Ticket<(T1, T2, T3, T4, T5, T6, T7)> WhenAll<T1, T2, T3, T4, T5, T6, T7>(Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4, Ticket<T5> task5, Ticket<T6> task6, Ticket<T7> task7)
        {
            if (task1.Status.IsCompletedSuccessfully() && task2.Status.IsCompletedSuccessfully() && task3.Status.IsCompletedSuccessfully() && task4.Status.IsCompletedSuccessfully() && task5.Status.IsCompletedSuccessfully() && task6.Status.IsCompletedSuccessfully() && task7.Status.IsCompletedSuccessfully())
            {
                return new Ticket<(T1, T2, T3, T4, T5, T6, T7)>((task1.GetAwaiter().GetResult(), task2.GetAwaiter().GetResult(), task3.GetAwaiter().GetResult(), task4.GetAwaiter().GetResult(), task5.GetAwaiter().GetResult(), task6.GetAwaiter().GetResult(), task7.GetAwaiter().GetResult()));
            }

            return new Ticket<(T1, T2, T3, T4, T5, T6, T7)>(new WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>(task1, task2, task3, task4, task5, task6, task7), 0);
        }

        sealed class WhenAllPromise<T1, T2, T3, T4, T5, T6, T7> : ITicketSource<(T1, T2, T3, T4, T5, T6, T7)>
        {
            T1 t1 = default;
            T2 t2 = default;
            T3 t3 = default;
            T4 t4 = default;
            T5 t5 = default;
            T6 t6 = default;
            T7 t7 = default;
            int completedCount;
            TicketCompletionSourceCore<(T1, T2, T3, T4, T5, T6, T7)> core;

            public WhenAllPromise(Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4, Ticket<T5> task5, Ticket<T6> task6, Ticket<T7> task7)
            {
                TaskTracker.TrackActiveTask(this, 3);

                this.completedCount = 0;
                {
                    var awaiter = task1.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT1(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>, Ticket<T1>.Awaiter>)state)
                            {
                                TryInvokeContinuationT1(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task2.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT2(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>, Ticket<T2>.Awaiter>)state)
                            {
                                TryInvokeContinuationT2(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task3.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT3(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>, Ticket<T3>.Awaiter>)state)
                            {
                                TryInvokeContinuationT3(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task4.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT4(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>, Ticket<T4>.Awaiter>)state)
                            {
                                TryInvokeContinuationT4(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task5.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT5(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>, Ticket<T5>.Awaiter>)state)
                            {
                                TryInvokeContinuationT5(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task6.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT6(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>, Ticket<T6>.Awaiter>)state)
                            {
                                TryInvokeContinuationT6(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task7.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT7(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>, Ticket<T7>.Awaiter>)state)
                            {
                                TryInvokeContinuationT7(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
            }

            static void TryInvokeContinuationT1(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7> self, in Ticket<T1>.Awaiter awaiter)
            {
                try
                {
                    self.t1 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 7)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7));
                }
            }

            static void TryInvokeContinuationT2(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7> self, in Ticket<T2>.Awaiter awaiter)
            {
                try
                {
                    self.t2 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 7)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7));
                }
            }

            static void TryInvokeContinuationT3(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7> self, in Ticket<T3>.Awaiter awaiter)
            {
                try
                {
                    self.t3 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 7)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7));
                }
            }

            static void TryInvokeContinuationT4(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7> self, in Ticket<T4>.Awaiter awaiter)
            {
                try
                {
                    self.t4 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 7)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7));
                }
            }

            static void TryInvokeContinuationT5(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7> self, in Ticket<T5>.Awaiter awaiter)
            {
                try
                {
                    self.t5 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 7)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7));
                }
            }

            static void TryInvokeContinuationT6(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7> self, in Ticket<T6>.Awaiter awaiter)
            {
                try
                {
                    self.t6 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 7)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7));
                }
            }

            static void TryInvokeContinuationT7(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7> self, in Ticket<T7>.Awaiter awaiter)
            {
                try
                {
                    self.t7 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 7)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7));
                }
            }


            public (T1, T2, T3, T4, T5, T6, T7) GetResult(short token)
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
        
        public static Ticket<(T1, T2, T3, T4, T5, T6, T7, T8)> WhenAll<T1, T2, T3, T4, T5, T6, T7, T8>(Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4, Ticket<T5> task5, Ticket<T6> task6, Ticket<T7> task7, Ticket<T8> task8)
        {
            if (task1.Status.IsCompletedSuccessfully() && task2.Status.IsCompletedSuccessfully() && task3.Status.IsCompletedSuccessfully() && task4.Status.IsCompletedSuccessfully() && task5.Status.IsCompletedSuccessfully() && task6.Status.IsCompletedSuccessfully() && task7.Status.IsCompletedSuccessfully() && task8.Status.IsCompletedSuccessfully())
            {
                return new Ticket<(T1, T2, T3, T4, T5, T6, T7, T8)>((task1.GetAwaiter().GetResult(), task2.GetAwaiter().GetResult(), task3.GetAwaiter().GetResult(), task4.GetAwaiter().GetResult(), task5.GetAwaiter().GetResult(), task6.GetAwaiter().GetResult(), task7.GetAwaiter().GetResult(), task8.GetAwaiter().GetResult()));
            }

            return new Ticket<(T1, T2, T3, T4, T5, T6, T7, T8)>(new WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>(task1, task2, task3, task4, task5, task6, task7, task8), 0);
        }

        sealed class WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8> : ITicketSource<(T1, T2, T3, T4, T5, T6, T7, T8)>
        {
            T1 t1 = default;
            T2 t2 = default;
            T3 t3 = default;
            T4 t4 = default;
            T5 t5 = default;
            T6 t6 = default;
            T7 t7 = default;
            T8 t8 = default;
            int completedCount;
            TicketCompletionSourceCore<(T1, T2, T3, T4, T5, T6, T7, T8)> core;

            public WhenAllPromise(Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4, Ticket<T5> task5, Ticket<T6> task6, Ticket<T7> task7, Ticket<T8> task8)
            {
                TaskTracker.TrackActiveTask(this, 3);

                this.completedCount = 0;
                {
                    var awaiter = task1.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT1(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>, Ticket<T1>.Awaiter>)state)
                            {
                                TryInvokeContinuationT1(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task2.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT2(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>, Ticket<T2>.Awaiter>)state)
                            {
                                TryInvokeContinuationT2(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task3.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT3(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>, Ticket<T3>.Awaiter>)state)
                            {
                                TryInvokeContinuationT3(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task4.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT4(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>, Ticket<T4>.Awaiter>)state)
                            {
                                TryInvokeContinuationT4(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task5.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT5(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>, Ticket<T5>.Awaiter>)state)
                            {
                                TryInvokeContinuationT5(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task6.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT6(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>, Ticket<T6>.Awaiter>)state)
                            {
                                TryInvokeContinuationT6(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task7.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT7(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>, Ticket<T7>.Awaiter>)state)
                            {
                                TryInvokeContinuationT7(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task8.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT8(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8>, Ticket<T8>.Awaiter>)state)
                            {
                                TryInvokeContinuationT8(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
            }

            static void TryInvokeContinuationT1(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8> self, in Ticket<T1>.Awaiter awaiter)
            {
                try
                {
                    self.t1 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 8)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8));
                }
            }

            static void TryInvokeContinuationT2(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8> self, in Ticket<T2>.Awaiter awaiter)
            {
                try
                {
                    self.t2 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 8)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8));
                }
            }

            static void TryInvokeContinuationT3(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8> self, in Ticket<T3>.Awaiter awaiter)
            {
                try
                {
                    self.t3 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 8)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8));
                }
            }

            static void TryInvokeContinuationT4(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8> self, in Ticket<T4>.Awaiter awaiter)
            {
                try
                {
                    self.t4 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 8)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8));
                }
            }

            static void TryInvokeContinuationT5(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8> self, in Ticket<T5>.Awaiter awaiter)
            {
                try
                {
                    self.t5 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 8)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8));
                }
            }

            static void TryInvokeContinuationT6(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8> self, in Ticket<T6>.Awaiter awaiter)
            {
                try
                {
                    self.t6 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 8)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8));
                }
            }

            static void TryInvokeContinuationT7(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8> self, in Ticket<T7>.Awaiter awaiter)
            {
                try
                {
                    self.t7 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 8)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8));
                }
            }

            static void TryInvokeContinuationT8(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8> self, in Ticket<T8>.Awaiter awaiter)
            {
                try
                {
                    self.t8 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 8)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8));
                }
            }


            public (T1, T2, T3, T4, T5, T6, T7, T8) GetResult(short token)
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
        
        public static Ticket<(T1, T2, T3, T4, T5, T6, T7, T8, T9)> WhenAll<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4, Ticket<T5> task5, Ticket<T6> task6, Ticket<T7> task7, Ticket<T8> task8, Ticket<T9> task9)
        {
            if (task1.Status.IsCompletedSuccessfully() && task2.Status.IsCompletedSuccessfully() && task3.Status.IsCompletedSuccessfully() && task4.Status.IsCompletedSuccessfully() && task5.Status.IsCompletedSuccessfully() && task6.Status.IsCompletedSuccessfully() && task7.Status.IsCompletedSuccessfully() && task8.Status.IsCompletedSuccessfully() && task9.Status.IsCompletedSuccessfully())
            {
                return new Ticket<(T1, T2, T3, T4, T5, T6, T7, T8, T9)>((task1.GetAwaiter().GetResult(), task2.GetAwaiter().GetResult(), task3.GetAwaiter().GetResult(), task4.GetAwaiter().GetResult(), task5.GetAwaiter().GetResult(), task6.GetAwaiter().GetResult(), task7.GetAwaiter().GetResult(), task8.GetAwaiter().GetResult(), task9.GetAwaiter().GetResult()));
            }

            return new Ticket<(T1, T2, T3, T4, T5, T6, T7, T8, T9)>(new WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>(task1, task2, task3, task4, task5, task6, task7, task8, task9), 0);
        }

        sealed class WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9> : ITicketSource<(T1, T2, T3, T4, T5, T6, T7, T8, T9)>
        {
            T1 t1 = default;
            T2 t2 = default;
            T3 t3 = default;
            T4 t4 = default;
            T5 t5 = default;
            T6 t6 = default;
            T7 t7 = default;
            T8 t8 = default;
            T9 t9 = default;
            int completedCount;
            TicketCompletionSourceCore<(T1, T2, T3, T4, T5, T6, T7, T8, T9)> core;

            public WhenAllPromise(Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4, Ticket<T5> task5, Ticket<T6> task6, Ticket<T7> task7, Ticket<T8> task8, Ticket<T9> task9)
            {
                TaskTracker.TrackActiveTask(this, 3);

                this.completedCount = 0;
                {
                    var awaiter = task1.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT1(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, Ticket<T1>.Awaiter>)state)
                            {
                                TryInvokeContinuationT1(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task2.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT2(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, Ticket<T2>.Awaiter>)state)
                            {
                                TryInvokeContinuationT2(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task3.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT3(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, Ticket<T3>.Awaiter>)state)
                            {
                                TryInvokeContinuationT3(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task4.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT4(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, Ticket<T4>.Awaiter>)state)
                            {
                                TryInvokeContinuationT4(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task5.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT5(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, Ticket<T5>.Awaiter>)state)
                            {
                                TryInvokeContinuationT5(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task6.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT6(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, Ticket<T6>.Awaiter>)state)
                            {
                                TryInvokeContinuationT6(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task7.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT7(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, Ticket<T7>.Awaiter>)state)
                            {
                                TryInvokeContinuationT7(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task8.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT8(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, Ticket<T8>.Awaiter>)state)
                            {
                                TryInvokeContinuationT8(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task9.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT9(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9>, Ticket<T9>.Awaiter>)state)
                            {
                                TryInvokeContinuationT9(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
            }

            static void TryInvokeContinuationT1(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9> self, in Ticket<T1>.Awaiter awaiter)
            {
                try
                {
                    self.t1 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 9)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9));
                }
            }

            static void TryInvokeContinuationT2(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9> self, in Ticket<T2>.Awaiter awaiter)
            {
                try
                {
                    self.t2 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 9)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9));
                }
            }

            static void TryInvokeContinuationT3(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9> self, in Ticket<T3>.Awaiter awaiter)
            {
                try
                {
                    self.t3 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 9)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9));
                }
            }

            static void TryInvokeContinuationT4(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9> self, in Ticket<T4>.Awaiter awaiter)
            {
                try
                {
                    self.t4 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 9)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9));
                }
            }

            static void TryInvokeContinuationT5(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9> self, in Ticket<T5>.Awaiter awaiter)
            {
                try
                {
                    self.t5 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 9)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9));
                }
            }

            static void TryInvokeContinuationT6(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9> self, in Ticket<T6>.Awaiter awaiter)
            {
                try
                {
                    self.t6 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 9)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9));
                }
            }

            static void TryInvokeContinuationT7(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9> self, in Ticket<T7>.Awaiter awaiter)
            {
                try
                {
                    self.t7 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 9)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9));
                }
            }

            static void TryInvokeContinuationT8(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9> self, in Ticket<T8>.Awaiter awaiter)
            {
                try
                {
                    self.t8 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 9)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9));
                }
            }

            static void TryInvokeContinuationT9(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9> self, in Ticket<T9>.Awaiter awaiter)
            {
                try
                {
                    self.t9 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 9)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9));
                }
            }


            public (T1, T2, T3, T4, T5, T6, T7, T8, T9) GetResult(short token)
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
        
        public static Ticket<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10)> WhenAll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4, Ticket<T5> task5, Ticket<T6> task6, Ticket<T7> task7, Ticket<T8> task8, Ticket<T9> task9, Ticket<T10> task10)
        {
            if (task1.Status.IsCompletedSuccessfully() && task2.Status.IsCompletedSuccessfully() && task3.Status.IsCompletedSuccessfully() && task4.Status.IsCompletedSuccessfully() && task5.Status.IsCompletedSuccessfully() && task6.Status.IsCompletedSuccessfully() && task7.Status.IsCompletedSuccessfully() && task8.Status.IsCompletedSuccessfully() && task9.Status.IsCompletedSuccessfully() && task10.Status.IsCompletedSuccessfully())
            {
                return new Ticket<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10)>((task1.GetAwaiter().GetResult(), task2.GetAwaiter().GetResult(), task3.GetAwaiter().GetResult(), task4.GetAwaiter().GetResult(), task5.GetAwaiter().GetResult(), task6.GetAwaiter().GetResult(), task7.GetAwaiter().GetResult(), task8.GetAwaiter().GetResult(), task9.GetAwaiter().GetResult(), task10.GetAwaiter().GetResult()));
            }

            return new Ticket<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10)>(new WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(task1, task2, task3, task4, task5, task6, task7, task8, task9, task10), 0);
        }

        sealed class WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : ITicketSource<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10)>
        {
            T1 t1 = default;
            T2 t2 = default;
            T3 t3 = default;
            T4 t4 = default;
            T5 t5 = default;
            T6 t6 = default;
            T7 t7 = default;
            T8 t8 = default;
            T9 t9 = default;
            T10 t10 = default;
            int completedCount;
            TicketCompletionSourceCore<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10)> core;

            public WhenAllPromise(Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4, Ticket<T5> task5, Ticket<T6> task6, Ticket<T7> task7, Ticket<T8> task8, Ticket<T9> task9, Ticket<T10> task10)
            {
                TaskTracker.TrackActiveTask(this, 3);

                this.completedCount = 0;
                {
                    var awaiter = task1.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT1(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, Ticket<T1>.Awaiter>)state)
                            {
                                TryInvokeContinuationT1(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task2.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT2(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, Ticket<T2>.Awaiter>)state)
                            {
                                TryInvokeContinuationT2(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task3.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT3(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, Ticket<T3>.Awaiter>)state)
                            {
                                TryInvokeContinuationT3(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task4.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT4(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, Ticket<T4>.Awaiter>)state)
                            {
                                TryInvokeContinuationT4(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task5.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT5(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, Ticket<T5>.Awaiter>)state)
                            {
                                TryInvokeContinuationT5(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task6.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT6(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, Ticket<T6>.Awaiter>)state)
                            {
                                TryInvokeContinuationT6(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task7.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT7(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, Ticket<T7>.Awaiter>)state)
                            {
                                TryInvokeContinuationT7(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task8.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT8(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, Ticket<T8>.Awaiter>)state)
                            {
                                TryInvokeContinuationT8(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task9.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT9(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, Ticket<T9>.Awaiter>)state)
                            {
                                TryInvokeContinuationT9(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task10.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT10(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, Ticket<T10>.Awaiter>)state)
                            {
                                TryInvokeContinuationT10(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
            }

            static void TryInvokeContinuationT1(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> self, in Ticket<T1>.Awaiter awaiter)
            {
                try
                {
                    self.t1 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 10)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10));
                }
            }

            static void TryInvokeContinuationT2(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> self, in Ticket<T2>.Awaiter awaiter)
            {
                try
                {
                    self.t2 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 10)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10));
                }
            }

            static void TryInvokeContinuationT3(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> self, in Ticket<T3>.Awaiter awaiter)
            {
                try
                {
                    self.t3 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 10)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10));
                }
            }

            static void TryInvokeContinuationT4(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> self, in Ticket<T4>.Awaiter awaiter)
            {
                try
                {
                    self.t4 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 10)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10));
                }
            }

            static void TryInvokeContinuationT5(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> self, in Ticket<T5>.Awaiter awaiter)
            {
                try
                {
                    self.t5 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 10)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10));
                }
            }

            static void TryInvokeContinuationT6(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> self, in Ticket<T6>.Awaiter awaiter)
            {
                try
                {
                    self.t6 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 10)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10));
                }
            }

            static void TryInvokeContinuationT7(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> self, in Ticket<T7>.Awaiter awaiter)
            {
                try
                {
                    self.t7 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 10)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10));
                }
            }

            static void TryInvokeContinuationT8(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> self, in Ticket<T8>.Awaiter awaiter)
            {
                try
                {
                    self.t8 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 10)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10));
                }
            }

            static void TryInvokeContinuationT9(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> self, in Ticket<T9>.Awaiter awaiter)
            {
                try
                {
                    self.t9 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 10)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10));
                }
            }

            static void TryInvokeContinuationT10(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> self, in Ticket<T10>.Awaiter awaiter)
            {
                try
                {
                    self.t10 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 10)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10));
                }
            }


            public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10) GetResult(short token)
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
        
        public static Ticket<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)> WhenAll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4, Ticket<T5> task5, Ticket<T6> task6, Ticket<T7> task7, Ticket<T8> task8, Ticket<T9> task9, Ticket<T10> task10, Ticket<T11> task11)
        {
            if (task1.Status.IsCompletedSuccessfully() && task2.Status.IsCompletedSuccessfully() && task3.Status.IsCompletedSuccessfully() && task4.Status.IsCompletedSuccessfully() && task5.Status.IsCompletedSuccessfully() && task6.Status.IsCompletedSuccessfully() && task7.Status.IsCompletedSuccessfully() && task8.Status.IsCompletedSuccessfully() && task9.Status.IsCompletedSuccessfully() && task10.Status.IsCompletedSuccessfully() && task11.Status.IsCompletedSuccessfully())
            {
                return new Ticket<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)>((task1.GetAwaiter().GetResult(), task2.GetAwaiter().GetResult(), task3.GetAwaiter().GetResult(), task4.GetAwaiter().GetResult(), task5.GetAwaiter().GetResult(), task6.GetAwaiter().GetResult(), task7.GetAwaiter().GetResult(), task8.GetAwaiter().GetResult(), task9.GetAwaiter().GetResult(), task10.GetAwaiter().GetResult(), task11.GetAwaiter().GetResult()));
            }

            return new Ticket<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)>(new WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(task1, task2, task3, task4, task5, task6, task7, task8, task9, task10, task11), 0);
        }

        sealed class WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : ITicketSource<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)>
        {
            T1 t1 = default;
            T2 t2 = default;
            T3 t3 = default;
            T4 t4 = default;
            T5 t5 = default;
            T6 t6 = default;
            T7 t7 = default;
            T8 t8 = default;
            T9 t9 = default;
            T10 t10 = default;
            T11 t11 = default;
            int completedCount;
            TicketCompletionSourceCore<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)> core;

            public WhenAllPromise(Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4, Ticket<T5> task5, Ticket<T6> task6, Ticket<T7> task7, Ticket<T8> task8, Ticket<T9> task9, Ticket<T10> task10, Ticket<T11> task11)
            {
                TaskTracker.TrackActiveTask(this, 3);

                this.completedCount = 0;
                {
                    var awaiter = task1.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT1(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, Ticket<T1>.Awaiter>)state)
                            {
                                TryInvokeContinuationT1(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task2.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT2(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, Ticket<T2>.Awaiter>)state)
                            {
                                TryInvokeContinuationT2(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task3.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT3(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, Ticket<T3>.Awaiter>)state)
                            {
                                TryInvokeContinuationT3(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task4.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT4(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, Ticket<T4>.Awaiter>)state)
                            {
                                TryInvokeContinuationT4(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task5.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT5(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, Ticket<T5>.Awaiter>)state)
                            {
                                TryInvokeContinuationT5(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task6.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT6(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, Ticket<T6>.Awaiter>)state)
                            {
                                TryInvokeContinuationT6(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task7.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT7(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, Ticket<T7>.Awaiter>)state)
                            {
                                TryInvokeContinuationT7(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task8.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT8(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, Ticket<T8>.Awaiter>)state)
                            {
                                TryInvokeContinuationT8(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task9.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT9(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, Ticket<T9>.Awaiter>)state)
                            {
                                TryInvokeContinuationT9(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task10.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT10(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, Ticket<T10>.Awaiter>)state)
                            {
                                TryInvokeContinuationT10(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task11.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT11(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, Ticket<T11>.Awaiter>)state)
                            {
                                TryInvokeContinuationT11(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
            }

            static void TryInvokeContinuationT1(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> self, in Ticket<T1>.Awaiter awaiter)
            {
                try
                {
                    self.t1 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 11)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11));
                }
            }

            static void TryInvokeContinuationT2(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> self, in Ticket<T2>.Awaiter awaiter)
            {
                try
                {
                    self.t2 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 11)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11));
                }
            }

            static void TryInvokeContinuationT3(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> self, in Ticket<T3>.Awaiter awaiter)
            {
                try
                {
                    self.t3 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 11)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11));
                }
            }

            static void TryInvokeContinuationT4(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> self, in Ticket<T4>.Awaiter awaiter)
            {
                try
                {
                    self.t4 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 11)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11));
                }
            }

            static void TryInvokeContinuationT5(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> self, in Ticket<T5>.Awaiter awaiter)
            {
                try
                {
                    self.t5 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 11)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11));
                }
            }

            static void TryInvokeContinuationT6(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> self, in Ticket<T6>.Awaiter awaiter)
            {
                try
                {
                    self.t6 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 11)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11));
                }
            }

            static void TryInvokeContinuationT7(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> self, in Ticket<T7>.Awaiter awaiter)
            {
                try
                {
                    self.t7 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 11)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11));
                }
            }

            static void TryInvokeContinuationT8(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> self, in Ticket<T8>.Awaiter awaiter)
            {
                try
                {
                    self.t8 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 11)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11));
                }
            }

            static void TryInvokeContinuationT9(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> self, in Ticket<T9>.Awaiter awaiter)
            {
                try
                {
                    self.t9 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 11)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11));
                }
            }

            static void TryInvokeContinuationT10(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> self, in Ticket<T10>.Awaiter awaiter)
            {
                try
                {
                    self.t10 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 11)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11));
                }
            }

            static void TryInvokeContinuationT11(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> self, in Ticket<T11>.Awaiter awaiter)
            {
                try
                {
                    self.t11 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 11)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11));
                }
            }


            public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11) GetResult(short token)
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
        
        public static Ticket<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12)> WhenAll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4, Ticket<T5> task5, Ticket<T6> task6, Ticket<T7> task7, Ticket<T8> task8, Ticket<T9> task9, Ticket<T10> task10, Ticket<T11> task11, Ticket<T12> task12)
        {
            if (task1.Status.IsCompletedSuccessfully() && task2.Status.IsCompletedSuccessfully() && task3.Status.IsCompletedSuccessfully() && task4.Status.IsCompletedSuccessfully() && task5.Status.IsCompletedSuccessfully() && task6.Status.IsCompletedSuccessfully() && task7.Status.IsCompletedSuccessfully() && task8.Status.IsCompletedSuccessfully() && task9.Status.IsCompletedSuccessfully() && task10.Status.IsCompletedSuccessfully() && task11.Status.IsCompletedSuccessfully() && task12.Status.IsCompletedSuccessfully())
            {
                return new Ticket<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12)>((task1.GetAwaiter().GetResult(), task2.GetAwaiter().GetResult(), task3.GetAwaiter().GetResult(), task4.GetAwaiter().GetResult(), task5.GetAwaiter().GetResult(), task6.GetAwaiter().GetResult(), task7.GetAwaiter().GetResult(), task8.GetAwaiter().GetResult(), task9.GetAwaiter().GetResult(), task10.GetAwaiter().GetResult(), task11.GetAwaiter().GetResult(), task12.GetAwaiter().GetResult()));
            }

            return new Ticket<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12)>(new WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(task1, task2, task3, task4, task5, task6, task7, task8, task9, task10, task11, task12), 0);
        }

        sealed class WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : ITicketSource<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12)>
        {
            T1 t1 = default;
            T2 t2 = default;
            T3 t3 = default;
            T4 t4 = default;
            T5 t5 = default;
            T6 t6 = default;
            T7 t7 = default;
            T8 t8 = default;
            T9 t9 = default;
            T10 t10 = default;
            T11 t11 = default;
            T12 t12 = default;
            int completedCount;
            TicketCompletionSourceCore<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12)> core;

            public WhenAllPromise(Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4, Ticket<T5> task5, Ticket<T6> task6, Ticket<T7> task7, Ticket<T8> task8, Ticket<T9> task9, Ticket<T10> task10, Ticket<T11> task11, Ticket<T12> task12)
            {
                TaskTracker.TrackActiveTask(this, 3);

                this.completedCount = 0;
                {
                    var awaiter = task1.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT1(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, Ticket<T1>.Awaiter>)state)
                            {
                                TryInvokeContinuationT1(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task2.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT2(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, Ticket<T2>.Awaiter>)state)
                            {
                                TryInvokeContinuationT2(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task3.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT3(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, Ticket<T3>.Awaiter>)state)
                            {
                                TryInvokeContinuationT3(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task4.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT4(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, Ticket<T4>.Awaiter>)state)
                            {
                                TryInvokeContinuationT4(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task5.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT5(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, Ticket<T5>.Awaiter>)state)
                            {
                                TryInvokeContinuationT5(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task6.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT6(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, Ticket<T6>.Awaiter>)state)
                            {
                                TryInvokeContinuationT6(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task7.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT7(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, Ticket<T7>.Awaiter>)state)
                            {
                                TryInvokeContinuationT7(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task8.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT8(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, Ticket<T8>.Awaiter>)state)
                            {
                                TryInvokeContinuationT8(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task9.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT9(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, Ticket<T9>.Awaiter>)state)
                            {
                                TryInvokeContinuationT9(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task10.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT10(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, Ticket<T10>.Awaiter>)state)
                            {
                                TryInvokeContinuationT10(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task11.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT11(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, Ticket<T11>.Awaiter>)state)
                            {
                                TryInvokeContinuationT11(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task12.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT12(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, Ticket<T12>.Awaiter>)state)
                            {
                                TryInvokeContinuationT12(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
            }

            static void TryInvokeContinuationT1(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in Ticket<T1>.Awaiter awaiter)
            {
                try
                {
                    self.t1 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 12)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12));
                }
            }

            static void TryInvokeContinuationT2(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in Ticket<T2>.Awaiter awaiter)
            {
                try
                {
                    self.t2 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 12)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12));
                }
            }

            static void TryInvokeContinuationT3(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in Ticket<T3>.Awaiter awaiter)
            {
                try
                {
                    self.t3 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 12)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12));
                }
            }

            static void TryInvokeContinuationT4(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in Ticket<T4>.Awaiter awaiter)
            {
                try
                {
                    self.t4 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 12)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12));
                }
            }

            static void TryInvokeContinuationT5(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in Ticket<T5>.Awaiter awaiter)
            {
                try
                {
                    self.t5 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 12)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12));
                }
            }

            static void TryInvokeContinuationT6(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in Ticket<T6>.Awaiter awaiter)
            {
                try
                {
                    self.t6 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 12)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12));
                }
            }

            static void TryInvokeContinuationT7(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in Ticket<T7>.Awaiter awaiter)
            {
                try
                {
                    self.t7 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 12)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12));
                }
            }

            static void TryInvokeContinuationT8(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in Ticket<T8>.Awaiter awaiter)
            {
                try
                {
                    self.t8 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 12)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12));
                }
            }

            static void TryInvokeContinuationT9(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in Ticket<T9>.Awaiter awaiter)
            {
                try
                {
                    self.t9 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 12)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12));
                }
            }

            static void TryInvokeContinuationT10(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in Ticket<T10>.Awaiter awaiter)
            {
                try
                {
                    self.t10 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 12)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12));
                }
            }

            static void TryInvokeContinuationT11(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in Ticket<T11>.Awaiter awaiter)
            {
                try
                {
                    self.t11 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 12)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12));
                }
            }

            static void TryInvokeContinuationT12(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, in Ticket<T12>.Awaiter awaiter)
            {
                try
                {
                    self.t12 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 12)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12));
                }
            }


            public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12) GetResult(short token)
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
        
        public static Ticket<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13)> WhenAll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4, Ticket<T5> task5, Ticket<T6> task6, Ticket<T7> task7, Ticket<T8> task8, Ticket<T9> task9, Ticket<T10> task10, Ticket<T11> task11, Ticket<T12> task12, Ticket<T13> task13)
        {
            if (task1.Status.IsCompletedSuccessfully() && task2.Status.IsCompletedSuccessfully() && task3.Status.IsCompletedSuccessfully() && task4.Status.IsCompletedSuccessfully() && task5.Status.IsCompletedSuccessfully() && task6.Status.IsCompletedSuccessfully() && task7.Status.IsCompletedSuccessfully() && task8.Status.IsCompletedSuccessfully() && task9.Status.IsCompletedSuccessfully() && task10.Status.IsCompletedSuccessfully() && task11.Status.IsCompletedSuccessfully() && task12.Status.IsCompletedSuccessfully() && task13.Status.IsCompletedSuccessfully())
            {
                return new Ticket<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13)>((task1.GetAwaiter().GetResult(), task2.GetAwaiter().GetResult(), task3.GetAwaiter().GetResult(), task4.GetAwaiter().GetResult(), task5.GetAwaiter().GetResult(), task6.GetAwaiter().GetResult(), task7.GetAwaiter().GetResult(), task8.GetAwaiter().GetResult(), task9.GetAwaiter().GetResult(), task10.GetAwaiter().GetResult(), task11.GetAwaiter().GetResult(), task12.GetAwaiter().GetResult(), task13.GetAwaiter().GetResult()));
            }

            return new Ticket<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13)>(new WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(task1, task2, task3, task4, task5, task6, task7, task8, task9, task10, task11, task12, task13), 0);
        }

        sealed class WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : ITicketSource<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13)>
        {
            T1 t1 = default;
            T2 t2 = default;
            T3 t3 = default;
            T4 t4 = default;
            T5 t5 = default;
            T6 t6 = default;
            T7 t7 = default;
            T8 t8 = default;
            T9 t9 = default;
            T10 t10 = default;
            T11 t11 = default;
            T12 t12 = default;
            T13 t13 = default;
            int completedCount;
            TicketCompletionSourceCore<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13)> core;

            public WhenAllPromise(Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4, Ticket<T5> task5, Ticket<T6> task6, Ticket<T7> task7, Ticket<T8> task8, Ticket<T9> task9, Ticket<T10> task10, Ticket<T11> task11, Ticket<T12> task12, Ticket<T13> task13)
            {
                TaskTracker.TrackActiveTask(this, 3);

                this.completedCount = 0;
                {
                    var awaiter = task1.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT1(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, Ticket<T1>.Awaiter>)state)
                            {
                                TryInvokeContinuationT1(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task2.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT2(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, Ticket<T2>.Awaiter>)state)
                            {
                                TryInvokeContinuationT2(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task3.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT3(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, Ticket<T3>.Awaiter>)state)
                            {
                                TryInvokeContinuationT3(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task4.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT4(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, Ticket<T4>.Awaiter>)state)
                            {
                                TryInvokeContinuationT4(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task5.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT5(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, Ticket<T5>.Awaiter>)state)
                            {
                                TryInvokeContinuationT5(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task6.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT6(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, Ticket<T6>.Awaiter>)state)
                            {
                                TryInvokeContinuationT6(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task7.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT7(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, Ticket<T7>.Awaiter>)state)
                            {
                                TryInvokeContinuationT7(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task8.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT8(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, Ticket<T8>.Awaiter>)state)
                            {
                                TryInvokeContinuationT8(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task9.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT9(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, Ticket<T9>.Awaiter>)state)
                            {
                                TryInvokeContinuationT9(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task10.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT10(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, Ticket<T10>.Awaiter>)state)
                            {
                                TryInvokeContinuationT10(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task11.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT11(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, Ticket<T11>.Awaiter>)state)
                            {
                                TryInvokeContinuationT11(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task12.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT12(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, Ticket<T12>.Awaiter>)state)
                            {
                                TryInvokeContinuationT12(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task13.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT13(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, Ticket<T13>.Awaiter>)state)
                            {
                                TryInvokeContinuationT13(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
            }

            static void TryInvokeContinuationT1(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in Ticket<T1>.Awaiter awaiter)
            {
                try
                {
                    self.t1 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 13)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13));
                }
            }

            static void TryInvokeContinuationT2(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in Ticket<T2>.Awaiter awaiter)
            {
                try
                {
                    self.t2 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 13)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13));
                }
            }

            static void TryInvokeContinuationT3(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in Ticket<T3>.Awaiter awaiter)
            {
                try
                {
                    self.t3 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 13)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13));
                }
            }

            static void TryInvokeContinuationT4(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in Ticket<T4>.Awaiter awaiter)
            {
                try
                {
                    self.t4 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 13)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13));
                }
            }

            static void TryInvokeContinuationT5(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in Ticket<T5>.Awaiter awaiter)
            {
                try
                {
                    self.t5 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 13)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13));
                }
            }

            static void TryInvokeContinuationT6(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in Ticket<T6>.Awaiter awaiter)
            {
                try
                {
                    self.t6 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 13)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13));
                }
            }

            static void TryInvokeContinuationT7(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in Ticket<T7>.Awaiter awaiter)
            {
                try
                {
                    self.t7 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 13)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13));
                }
            }

            static void TryInvokeContinuationT8(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in Ticket<T8>.Awaiter awaiter)
            {
                try
                {
                    self.t8 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 13)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13));
                }
            }

            static void TryInvokeContinuationT9(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in Ticket<T9>.Awaiter awaiter)
            {
                try
                {
                    self.t9 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 13)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13));
                }
            }

            static void TryInvokeContinuationT10(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in Ticket<T10>.Awaiter awaiter)
            {
                try
                {
                    self.t10 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 13)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13));
                }
            }

            static void TryInvokeContinuationT11(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in Ticket<T11>.Awaiter awaiter)
            {
                try
                {
                    self.t11 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 13)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13));
                }
            }

            static void TryInvokeContinuationT12(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in Ticket<T12>.Awaiter awaiter)
            {
                try
                {
                    self.t12 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 13)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13));
                }
            }

            static void TryInvokeContinuationT13(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, in Ticket<T13>.Awaiter awaiter)
            {
                try
                {
                    self.t13 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 13)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13));
                }
            }


            public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13) GetResult(short token)
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
        
        public static Ticket<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14)> WhenAll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4, Ticket<T5> task5, Ticket<T6> task6, Ticket<T7> task7, Ticket<T8> task8, Ticket<T9> task9, Ticket<T10> task10, Ticket<T11> task11, Ticket<T12> task12, Ticket<T13> task13, Ticket<T14> task14)
        {
            if (task1.Status.IsCompletedSuccessfully() && task2.Status.IsCompletedSuccessfully() && task3.Status.IsCompletedSuccessfully() && task4.Status.IsCompletedSuccessfully() && task5.Status.IsCompletedSuccessfully() && task6.Status.IsCompletedSuccessfully() && task7.Status.IsCompletedSuccessfully() && task8.Status.IsCompletedSuccessfully() && task9.Status.IsCompletedSuccessfully() && task10.Status.IsCompletedSuccessfully() && task11.Status.IsCompletedSuccessfully() && task12.Status.IsCompletedSuccessfully() && task13.Status.IsCompletedSuccessfully() && task14.Status.IsCompletedSuccessfully())
            {
                return new Ticket<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14)>((task1.GetAwaiter().GetResult(), task2.GetAwaiter().GetResult(), task3.GetAwaiter().GetResult(), task4.GetAwaiter().GetResult(), task5.GetAwaiter().GetResult(), task6.GetAwaiter().GetResult(), task7.GetAwaiter().GetResult(), task8.GetAwaiter().GetResult(), task9.GetAwaiter().GetResult(), task10.GetAwaiter().GetResult(), task11.GetAwaiter().GetResult(), task12.GetAwaiter().GetResult(), task13.GetAwaiter().GetResult(), task14.GetAwaiter().GetResult()));
            }

            return new Ticket<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14)>(new WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(task1, task2, task3, task4, task5, task6, task7, task8, task9, task10, task11, task12, task13, task14), 0);
        }

        sealed class WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : ITicketSource<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14)>
        {
            T1 t1 = default;
            T2 t2 = default;
            T3 t3 = default;
            T4 t4 = default;
            T5 t5 = default;
            T6 t6 = default;
            T7 t7 = default;
            T8 t8 = default;
            T9 t9 = default;
            T10 t10 = default;
            T11 t11 = default;
            T12 t12 = default;
            T13 t13 = default;
            T14 t14 = default;
            int completedCount;
            TicketCompletionSourceCore<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14)> core;

            public WhenAllPromise(Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4, Ticket<T5> task5, Ticket<T6> task6, Ticket<T7> task7, Ticket<T8> task8, Ticket<T9> task9, Ticket<T10> task10, Ticket<T11> task11, Ticket<T12> task12, Ticket<T13> task13, Ticket<T14> task14)
            {
                TaskTracker.TrackActiveTask(this, 3);

                this.completedCount = 0;
                {
                    var awaiter = task1.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT1(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, Ticket<T1>.Awaiter>)state)
                            {
                                TryInvokeContinuationT1(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task2.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT2(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, Ticket<T2>.Awaiter>)state)
                            {
                                TryInvokeContinuationT2(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task3.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT3(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, Ticket<T3>.Awaiter>)state)
                            {
                                TryInvokeContinuationT3(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task4.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT4(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, Ticket<T4>.Awaiter>)state)
                            {
                                TryInvokeContinuationT4(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task5.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT5(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, Ticket<T5>.Awaiter>)state)
                            {
                                TryInvokeContinuationT5(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task6.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT6(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, Ticket<T6>.Awaiter>)state)
                            {
                                TryInvokeContinuationT6(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task7.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT7(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, Ticket<T7>.Awaiter>)state)
                            {
                                TryInvokeContinuationT7(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task8.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT8(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, Ticket<T8>.Awaiter>)state)
                            {
                                TryInvokeContinuationT8(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task9.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT9(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, Ticket<T9>.Awaiter>)state)
                            {
                                TryInvokeContinuationT9(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task10.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT10(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, Ticket<T10>.Awaiter>)state)
                            {
                                TryInvokeContinuationT10(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task11.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT11(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, Ticket<T11>.Awaiter>)state)
                            {
                                TryInvokeContinuationT11(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task12.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT12(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, Ticket<T12>.Awaiter>)state)
                            {
                                TryInvokeContinuationT12(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task13.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT13(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, Ticket<T13>.Awaiter>)state)
                            {
                                TryInvokeContinuationT13(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task14.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT14(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, Ticket<T14>.Awaiter>)state)
                            {
                                TryInvokeContinuationT14(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
            }

            static void TryInvokeContinuationT1(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in Ticket<T1>.Awaiter awaiter)
            {
                try
                {
                    self.t1 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 14)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14));
                }
            }

            static void TryInvokeContinuationT2(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in Ticket<T2>.Awaiter awaiter)
            {
                try
                {
                    self.t2 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 14)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14));
                }
            }

            static void TryInvokeContinuationT3(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in Ticket<T3>.Awaiter awaiter)
            {
                try
                {
                    self.t3 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 14)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14));
                }
            }

            static void TryInvokeContinuationT4(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in Ticket<T4>.Awaiter awaiter)
            {
                try
                {
                    self.t4 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 14)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14));
                }
            }

            static void TryInvokeContinuationT5(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in Ticket<T5>.Awaiter awaiter)
            {
                try
                {
                    self.t5 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 14)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14));
                }
            }

            static void TryInvokeContinuationT6(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in Ticket<T6>.Awaiter awaiter)
            {
                try
                {
                    self.t6 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 14)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14));
                }
            }

            static void TryInvokeContinuationT7(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in Ticket<T7>.Awaiter awaiter)
            {
                try
                {
                    self.t7 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 14)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14));
                }
            }

            static void TryInvokeContinuationT8(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in Ticket<T8>.Awaiter awaiter)
            {
                try
                {
                    self.t8 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 14)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14));
                }
            }

            static void TryInvokeContinuationT9(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in Ticket<T9>.Awaiter awaiter)
            {
                try
                {
                    self.t9 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 14)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14));
                }
            }

            static void TryInvokeContinuationT10(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in Ticket<T10>.Awaiter awaiter)
            {
                try
                {
                    self.t10 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 14)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14));
                }
            }

            static void TryInvokeContinuationT11(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in Ticket<T11>.Awaiter awaiter)
            {
                try
                {
                    self.t11 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 14)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14));
                }
            }

            static void TryInvokeContinuationT12(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in Ticket<T12>.Awaiter awaiter)
            {
                try
                {
                    self.t12 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 14)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14));
                }
            }

            static void TryInvokeContinuationT13(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in Ticket<T13>.Awaiter awaiter)
            {
                try
                {
                    self.t13 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 14)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14));
                }
            }

            static void TryInvokeContinuationT14(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, in Ticket<T14>.Awaiter awaiter)
            {
                try
                {
                    self.t14 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 14)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14));
                }
            }


            public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14) GetResult(short token)
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
        
        public static Ticket<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15)> WhenAll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4, Ticket<T5> task5, Ticket<T6> task6, Ticket<T7> task7, Ticket<T8> task8, Ticket<T9> task9, Ticket<T10> task10, Ticket<T11> task11, Ticket<T12> task12, Ticket<T13> task13, Ticket<T14> task14, Ticket<T15> task15)
        {
            if (task1.Status.IsCompletedSuccessfully() && task2.Status.IsCompletedSuccessfully() && task3.Status.IsCompletedSuccessfully() && task4.Status.IsCompletedSuccessfully() && task5.Status.IsCompletedSuccessfully() && task6.Status.IsCompletedSuccessfully() && task7.Status.IsCompletedSuccessfully() && task8.Status.IsCompletedSuccessfully() && task9.Status.IsCompletedSuccessfully() && task10.Status.IsCompletedSuccessfully() && task11.Status.IsCompletedSuccessfully() && task12.Status.IsCompletedSuccessfully() && task13.Status.IsCompletedSuccessfully() && task14.Status.IsCompletedSuccessfully() && task15.Status.IsCompletedSuccessfully())
            {
                return new Ticket<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15)>((task1.GetAwaiter().GetResult(), task2.GetAwaiter().GetResult(), task3.GetAwaiter().GetResult(), task4.GetAwaiter().GetResult(), task5.GetAwaiter().GetResult(), task6.GetAwaiter().GetResult(), task7.GetAwaiter().GetResult(), task8.GetAwaiter().GetResult(), task9.GetAwaiter().GetResult(), task10.GetAwaiter().GetResult(), task11.GetAwaiter().GetResult(), task12.GetAwaiter().GetResult(), task13.GetAwaiter().GetResult(), task14.GetAwaiter().GetResult(), task15.GetAwaiter().GetResult()));
            }

            return new Ticket<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15)>(new WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(task1, task2, task3, task4, task5, task6, task7, task8, task9, task10, task11, task12, task13, task14, task15), 0);
        }

        sealed class WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : ITicketSource<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15)>
        {
            T1 t1 = default;
            T2 t2 = default;
            T3 t3 = default;
            T4 t4 = default;
            T5 t5 = default;
            T6 t6 = default;
            T7 t7 = default;
            T8 t8 = default;
            T9 t9 = default;
            T10 t10 = default;
            T11 t11 = default;
            T12 t12 = default;
            T13 t13 = default;
            T14 t14 = default;
            T15 t15 = default;
            int completedCount;
            TicketCompletionSourceCore<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15)> core;

            public WhenAllPromise(Ticket<T1> task1, Ticket<T2> task2, Ticket<T3> task3, Ticket<T4> task4, Ticket<T5> task5, Ticket<T6> task6, Ticket<T7> task7, Ticket<T8> task8, Ticket<T9> task9, Ticket<T10> task10, Ticket<T11> task11, Ticket<T12> task12, Ticket<T13> task13, Ticket<T14> task14, Ticket<T15> task15)
            {
                TaskTracker.TrackActiveTask(this, 3);

                this.completedCount = 0;
                {
                    var awaiter = task1.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT1(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, Ticket<T1>.Awaiter>)state)
                            {
                                TryInvokeContinuationT1(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task2.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT2(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, Ticket<T2>.Awaiter>)state)
                            {
                                TryInvokeContinuationT2(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task3.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT3(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, Ticket<T3>.Awaiter>)state)
                            {
                                TryInvokeContinuationT3(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task4.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT4(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, Ticket<T4>.Awaiter>)state)
                            {
                                TryInvokeContinuationT4(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task5.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT5(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, Ticket<T5>.Awaiter>)state)
                            {
                                TryInvokeContinuationT5(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task6.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT6(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, Ticket<T6>.Awaiter>)state)
                            {
                                TryInvokeContinuationT6(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task7.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT7(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, Ticket<T7>.Awaiter>)state)
                            {
                                TryInvokeContinuationT7(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task8.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT8(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, Ticket<T8>.Awaiter>)state)
                            {
                                TryInvokeContinuationT8(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task9.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT9(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, Ticket<T9>.Awaiter>)state)
                            {
                                TryInvokeContinuationT9(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task10.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT10(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, Ticket<T10>.Awaiter>)state)
                            {
                                TryInvokeContinuationT10(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task11.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT11(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, Ticket<T11>.Awaiter>)state)
                            {
                                TryInvokeContinuationT11(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task12.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT12(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, Ticket<T12>.Awaiter>)state)
                            {
                                TryInvokeContinuationT12(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task13.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT13(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, Ticket<T13>.Awaiter>)state)
                            {
                                TryInvokeContinuationT13(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task14.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT14(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, Ticket<T14>.Awaiter>)state)
                            {
                                TryInvokeContinuationT14(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task15.GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT15(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, Ticket<T15>.Awaiter>)state)
                            {
                                TryInvokeContinuationT15(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
            }

            static void TryInvokeContinuationT1(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in Ticket<T1>.Awaiter awaiter)
            {
                try
                {
                    self.t1 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 15)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14, self.t15));
                }
            }

            static void TryInvokeContinuationT2(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in Ticket<T2>.Awaiter awaiter)
            {
                try
                {
                    self.t2 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 15)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14, self.t15));
                }
            }

            static void TryInvokeContinuationT3(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in Ticket<T3>.Awaiter awaiter)
            {
                try
                {
                    self.t3 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 15)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14, self.t15));
                }
            }

            static void TryInvokeContinuationT4(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in Ticket<T4>.Awaiter awaiter)
            {
                try
                {
                    self.t4 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 15)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14, self.t15));
                }
            }

            static void TryInvokeContinuationT5(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in Ticket<T5>.Awaiter awaiter)
            {
                try
                {
                    self.t5 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 15)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14, self.t15));
                }
            }

            static void TryInvokeContinuationT6(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in Ticket<T6>.Awaiter awaiter)
            {
                try
                {
                    self.t6 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 15)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14, self.t15));
                }
            }

            static void TryInvokeContinuationT7(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in Ticket<T7>.Awaiter awaiter)
            {
                try
                {
                    self.t7 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 15)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14, self.t15));
                }
            }

            static void TryInvokeContinuationT8(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in Ticket<T8>.Awaiter awaiter)
            {
                try
                {
                    self.t8 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 15)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14, self.t15));
                }
            }

            static void TryInvokeContinuationT9(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in Ticket<T9>.Awaiter awaiter)
            {
                try
                {
                    self.t9 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 15)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14, self.t15));
                }
            }

            static void TryInvokeContinuationT10(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in Ticket<T10>.Awaiter awaiter)
            {
                try
                {
                    self.t10 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 15)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14, self.t15));
                }
            }

            static void TryInvokeContinuationT11(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in Ticket<T11>.Awaiter awaiter)
            {
                try
                {
                    self.t11 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 15)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14, self.t15));
                }
            }

            static void TryInvokeContinuationT12(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in Ticket<T12>.Awaiter awaiter)
            {
                try
                {
                    self.t12 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 15)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14, self.t15));
                }
            }

            static void TryInvokeContinuationT13(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in Ticket<T13>.Awaiter awaiter)
            {
                try
                {
                    self.t13 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 15)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14, self.t15));
                }
            }

            static void TryInvokeContinuationT14(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in Ticket<T14>.Awaiter awaiter)
            {
                try
                {
                    self.t14 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 15)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14, self.t15));
                }
            }

            static void TryInvokeContinuationT15(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> self, in Ticket<T15>.Awaiter awaiter)
            {
                try
                {
                    self.t15 = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }
                
                if (Interlocked.Increment(ref self.completedCount) == 15)
                {
                    self.core.TrySetResult((self.t1, self.t2, self.t3, self.t4, self.t5, self.t6, self.t7, self.t8, self.t9, self.t10, self.t11, self.t12, self.t13, self.t14, self.t15));
                }
            }


            public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15) GetResult(short token)
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
    }
}