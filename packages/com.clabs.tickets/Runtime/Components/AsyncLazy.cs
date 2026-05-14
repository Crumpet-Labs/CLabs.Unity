#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading;

namespace CLabs.Tickets
{
    public sealed class AsyncLazy
    {
        static Action<object> continuation = SetCompletionSource;

        Func<Ticket> taskFactory;
        TicketCompletionSource completionSource;
        Ticket.Awaiter awaiter;

        object syncLock;
        bool initialized;

        public AsyncLazy(Func<Ticket> taskFactory)
        {
            this.taskFactory = taskFactory;
            this.completionSource = new TicketCompletionSource();
            this.syncLock = new object();
            this.initialized = false;
        }

        internal AsyncLazy(Ticket task)
        {
            this.taskFactory = null;
            this.completionSource = new TicketCompletionSource();
            this.syncLock = null;
            this.initialized = true;

            var awaiter = task.GetAwaiter();
            if (awaiter.IsCompleted)
            {
                SetCompletionSource(awaiter);
            }
            else
            {
                this.awaiter = awaiter;
                awaiter.SourceOnCompleted(continuation, this);
            }
        }

        public Ticket Task
        {
            get
            {
                EnsureInitialized();
                return completionSource.Task;
            }
        }


        public Ticket.Awaiter GetAwaiter() => Task.GetAwaiter();

        void EnsureInitialized()
        {
            if (Volatile.Read(ref initialized))
            {
                return;
            }

            EnsureInitializedCore();
        }

        void EnsureInitializedCore()
        {
            lock (syncLock)
            {
                if (!Volatile.Read(ref initialized))
                {
                    var f = Interlocked.Exchange(ref taskFactory, null);
                    if (f != null)
                    {
                        var task = f();
                        var awaiter = task.GetAwaiter();
                        if (awaiter.IsCompleted)
                        {
                            SetCompletionSource(awaiter);
                        }
                        else
                        {
                            this.awaiter = awaiter;
                            awaiter.SourceOnCompleted(continuation, this);
                        }

                        Volatile.Write(ref initialized, true);
                    }
                }
            }
        }

        void SetCompletionSource(in Ticket.Awaiter awaiter)
        {
            try
            {
                awaiter.GetResult();
                completionSource.TrySetResult();
            }
            catch (Exception ex)
            {
                completionSource.TrySetException(ex);
            }
        }

        static void SetCompletionSource(object state)
        {
            var self = (AsyncLazy)state;
            try
            {
                self.awaiter.GetResult();
                self.completionSource.TrySetResult();
            }
            catch (Exception ex)
            {
                self.completionSource.TrySetException(ex);
            }
            finally
            {
                self.awaiter = default;
            }
        }
    }

    public sealed class AsyncLazy<T>
    {
        static Action<object> continuation = SetCompletionSource;

        Func<Ticket<T>> taskFactory;
        TicketCompletionSource<T> completionSource;
        Ticket<T>.Awaiter awaiter;

        object syncLock;
        bool initialized;

        public AsyncLazy(Func<Ticket<T>> taskFactory)
        {
            this.taskFactory = taskFactory;
            this.completionSource = new TicketCompletionSource<T>();
            this.syncLock = new object();
            this.initialized = false;
        }

        internal AsyncLazy(Ticket<T> task)
        {
            this.taskFactory = null;
            this.completionSource = new TicketCompletionSource<T>();
            this.syncLock = null;
            this.initialized = true;

            var awaiter = task.GetAwaiter();
            if (awaiter.IsCompleted)
            {
                SetCompletionSource(awaiter);
            }
            else
            {
                this.awaiter = awaiter;
                awaiter.SourceOnCompleted(continuation, this);
            }
        }

        public Ticket<T> Task
        {
            get
            {
                EnsureInitialized();
                return completionSource.Task;
            }
        }


        public Ticket<T>.Awaiter GetAwaiter() => Task.GetAwaiter();

        void EnsureInitialized()
        {
            if (Volatile.Read(ref initialized))
            {
                return;
            }

            EnsureInitializedCore();
        }

        void EnsureInitializedCore()
        {
            lock (syncLock)
            {
                if (!Volatile.Read(ref initialized))
                {
                    var f = Interlocked.Exchange(ref taskFactory, null);
                    if (f != null)
                    {
                        var task = f();
                        var awaiter = task.GetAwaiter();
                        if (awaiter.IsCompleted)
                        {
                            SetCompletionSource(awaiter);
                        }
                        else
                        {
                            this.awaiter = awaiter;
                            awaiter.SourceOnCompleted(continuation, this);
                        }

                        Volatile.Write(ref initialized, true);
                    }
                }
            }
        }

        void SetCompletionSource(in Ticket<T>.Awaiter awaiter)
        {
            try
            {
                var result = awaiter.GetResult();
                completionSource.TrySetResult(result);
            }
            catch (Exception ex)
            {
                completionSource.TrySetException(ex);
            }
        }

        static void SetCompletionSource(object state)
        {
            var self = (AsyncLazy<T>)state;
            try
            {
                var result = self.awaiter.GetResult();
                self.completionSource.TrySetResult(result);
            }
            catch (Exception ex)
            {
                self.completionSource.TrySetException(ex);
            }
            finally
            {
                self.awaiter = default;
            }
        }
    }
}
