#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using CLabs.Tickets.Internal;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace CLabs.Tickets
{
    public partial struct Ticket
    {
        static readonly Ticket CanceledTicket = new Func<Ticket>(() =>
        {
            return new Ticket(new CanceledResultSource(CancellationToken.None), 0);
        })();

        static class CanceledTicketCache<T>
        {
            public static readonly Ticket<T> Task;

            static CanceledTicketCache()
            {
                Task = new Ticket<T>(new CanceledResultSource<T>(CancellationToken.None), 0);
            }
        }

        public static readonly Ticket CompletedTask = new Ticket();

        public static Ticket FromException(Exception ex)
        {
            if (ex is OperationCanceledException oce)
            {
                return FromCanceled(oce.CancellationToken);
            }

            return new Ticket(new ExceptionResultSource(ex), 0);
        }

        public static Ticket<T> FromException<T>(Exception ex)
        {
            if (ex is OperationCanceledException oce)
            {
                return FromCanceled<T>(oce.CancellationToken);
            }

            return new Ticket<T>(new ExceptionResultSource<T>(ex), 0);
        }

        public static Ticket<T> FromResult<T>(T value)
        {
            return new Ticket<T>(value);
        }

        public static Ticket FromCanceled(CancellationToken cancellationToken = default)
        {
            if (cancellationToken == CancellationToken.None)
            {
                return CanceledTicket;
            }
            else
            {
                return new Ticket(new CanceledResultSource(cancellationToken), 0);
            }
        }

        public static Ticket<T> FromCanceled<T>(CancellationToken cancellationToken = default)
        {
            if (cancellationToken == CancellationToken.None)
            {
                return CanceledTicketCache<T>.Task;
            }
            else
            {
                return new Ticket<T>(new CanceledResultSource<T>(cancellationToken), 0);
            }
        }

        public static Ticket Create(Func<Ticket> factory)
        {
            return factory();
        }

        public static Ticket Create(Func<CancellationToken, Ticket> factory, CancellationToken cancellationToken)
        {
            return factory(cancellationToken);
        }

        public static Ticket Create<T>(T state, Func<T, Ticket> factory)
        {
            return factory(state);
        }

        public static Ticket<T> Create<T>(Func<Ticket<T>> factory)
        {
            return factory();
        }

        public static AsyncLazy Lazy(Func<Ticket> factory)
        {
            return new AsyncLazy(factory);
        }

        public static AsyncLazy<T> Lazy<T>(Func<Ticket<T>> factory)
        {
            return new AsyncLazy<T>(factory);
        }

        /// <summary>
        /// helper of fire and forget void action.
        /// </summary>
        public static void Void(Func<TicketVoid> asyncAction)
        {
            asyncAction().Forget();
        }

        /// <summary>
        /// helper of fire and forget void action.
        /// </summary>
        public static void Void(Func<CancellationToken, TicketVoid> asyncAction, CancellationToken cancellationToken)
        {
            asyncAction(cancellationToken).Forget();
        }

        /// <summary>
        /// helper of fire and forget void action.
        /// </summary>
        public static void Void<T>(Func<T, TicketVoid> asyncAction, T state)
        {
            asyncAction(state).Forget();
        }

        /// <summary>
        /// helper of create add TicketVoid to delegate.
        /// For example: FooAction = Ticket.Action(async () => { /* */ })
        /// </summary>
        public static Action Action(Func<TicketVoid> asyncAction)
        {
            return () => asyncAction().Forget();
        }

        /// <summary>
        /// helper of create add TicketVoid to delegate.
        /// </summary>
        public static Action Action(Func<CancellationToken, TicketVoid> asyncAction, CancellationToken cancellationToken)
        {
            return () => asyncAction(cancellationToken).Forget();
        }

        /// <summary>
        /// helper of create add TicketVoid to delegate.
        /// </summary>
        public static Action Action<T>(T state, Func<T, TicketVoid> asyncAction)
        {
            return () => asyncAction(state).Forget();
        }

        // Ticket.UnityAction(...) factory overloads were moved to
        // TicketUnityActions.UnityAction(...) in com.clabs.adapter.unity.ticket
        // as part of Phase B engine separation. C# partial classes cannot span
        // assemblies, so the engine-typed UnityAction overloads could not stay
        // as partials of Ticket in core. The signature is identical; only the
        // static class qualifier changed.

        /// <summary>
        /// Defer the task creation just before call await.
        /// </summary>
        public static Ticket Defer(Func<Ticket> factory)
        {
            return new Ticket(new DeferPromise(factory), 0);
        }

        /// <summary>
        /// Defer the task creation just before call await.
        /// </summary>
        public static Ticket<T> Defer<T>(Func<Ticket<T>> factory)
        {
            return new Ticket<T>(new DeferPromise<T>(factory), 0);
        }

        /// <summary>
        /// Defer the task creation just before call await.
        /// </summary>
        public static Ticket Defer<TState>(TState state, Func<TState, Ticket> factory)
        {
            return new Ticket(new DeferPromiseWithState<TState>(state, factory), 0);
        }

        /// <summary>
        /// Defer the task creation just before call await.
        /// </summary>
        public static Ticket<TResult> Defer<TState, TResult>(TState state, Func<TState, Ticket<TResult>> factory)
        {
            return new Ticket<TResult>(new DeferPromiseWithState<TState, TResult>(state, factory), 0);
        }

        /// <summary>
        /// Never complete.
        /// </summary>
        public static Ticket Never(CancellationToken cancellationToken)
        {
            return new Ticket<AsyncUnit>(new NeverPromise<AsyncUnit>(cancellationToken), 0);
        }

        /// <summary>
        /// Never complete.
        /// </summary>
        public static Ticket<T> Never<T>(CancellationToken cancellationToken)
        {
            return new Ticket<T>(new NeverPromise<T>(cancellationToken), 0);
        }

        sealed class ExceptionResultSource : ITicketSource
        {
            readonly ExceptionDispatchInfo exception;
            bool calledGet;

            public ExceptionResultSource(Exception exception)
            {
                this.exception = ExceptionDispatchInfo.Capture(exception);
            }

            public void GetResult(short token)
            {
                if (!calledGet)
                {
                    calledGet = true;
                    GC.SuppressFinalize(this);
                }
                exception.Throw();
            }

            public TicketStatus GetStatus(short token)
            {
                return TicketStatus.Faulted;
            }

            public TicketStatus UnsafeGetStatus()
            {
                return TicketStatus.Faulted;
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                continuation(state);
            }

            ~ExceptionResultSource()
            {
                if (!calledGet)
                {
                    TicketScheduler.PublishUnobservedTaskException(exception.SourceException);
                }
            }
        }

        sealed class ExceptionResultSource<T> : ITicketSource<T>
        {
            readonly ExceptionDispatchInfo exception;
            bool calledGet;

            public ExceptionResultSource(Exception exception)
            {
                this.exception = ExceptionDispatchInfo.Capture(exception);
            }

            public T GetResult(short token)
            {
                if (!calledGet)
                {
                    calledGet = true;
                    GC.SuppressFinalize(this);
                }
                exception.Throw();
                return default;
            }

            void ITicketSource.GetResult(short token)
            {
                if (!calledGet)
                {
                    calledGet = true;
                    GC.SuppressFinalize(this);
                }
                exception.Throw();
            }

            public TicketStatus GetStatus(short token)
            {
                return TicketStatus.Faulted;
            }

            public TicketStatus UnsafeGetStatus()
            {
                return TicketStatus.Faulted;
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                continuation(state);
            }

            ~ExceptionResultSource()
            {
                if (!calledGet)
                {
                    TicketScheduler.PublishUnobservedTaskException(exception.SourceException);
                }
            }
        }

        sealed class CanceledResultSource : ITicketSource
        {
            readonly CancellationToken cancellationToken;

            public CanceledResultSource(CancellationToken cancellationToken)
            {
                this.cancellationToken = cancellationToken;
            }

            public void GetResult(short token)
            {
                throw new OperationCanceledException(cancellationToken);
            }

            public TicketStatus GetStatus(short token)
            {
                return TicketStatus.Canceled;
            }

            public TicketStatus UnsafeGetStatus()
            {
                return TicketStatus.Canceled;
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                continuation(state);
            }
        }

        sealed class CanceledResultSource<T> : ITicketSource<T>
        {
            readonly CancellationToken cancellationToken;

            public CanceledResultSource(CancellationToken cancellationToken)
            {
                this.cancellationToken = cancellationToken;
            }

            public T GetResult(short token)
            {
                throw new OperationCanceledException(cancellationToken);
            }

            void ITicketSource.GetResult(short token)
            {
                throw new OperationCanceledException(cancellationToken);
            }

            public TicketStatus GetStatus(short token)
            {
                return TicketStatus.Canceled;
            }

            public TicketStatus UnsafeGetStatus()
            {
                return TicketStatus.Canceled;
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                continuation(state);
            }
        }

        sealed class DeferPromise : ITicketSource
        {
            Func<Ticket> factory;
            Ticket task;
            Ticket.Awaiter awaiter;

            public DeferPromise(Func<Ticket> factory)
            {
                this.factory = factory;
            }

            public void GetResult(short token)
            {
                awaiter.GetResult();
            }

            public TicketStatus GetStatus(short token)
            {
                var f = Interlocked.Exchange(ref factory, null);
                if (f != null)
                {
                    task = f();
                    awaiter = task.GetAwaiter();
                }

                return task.Status;
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                awaiter.SourceOnCompleted(continuation, state);
            }

            public TicketStatus UnsafeGetStatus()
            {
                return task.Status;
            }
        }

        sealed class DeferPromise<T> : ITicketSource<T>
        {
            Func<Ticket<T>> factory;
            Ticket<T> task;
            Ticket<T>.Awaiter awaiter;

            public DeferPromise(Func<Ticket<T>> factory)
            {
                this.factory = factory;
            }

            public T GetResult(short token)
            {
                return awaiter.GetResult();
            }

            void ITicketSource.GetResult(short token)
            {
                awaiter.GetResult();
            }

            public TicketStatus GetStatus(short token)
            {
                var f = Interlocked.Exchange(ref factory, null);
                if (f != null)
                {
                    task = f();
                    awaiter = task.GetAwaiter();
                }

                return task.Status;
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                awaiter.SourceOnCompleted(continuation, state);
            }

            public TicketStatus UnsafeGetStatus()
            {
                return task.Status;
            }
        }

        sealed class DeferPromiseWithState<TState> : ITicketSource
        {
            Func<TState, Ticket> factory;
            TState argument;
            Ticket task;
            Ticket.Awaiter awaiter;

            public DeferPromiseWithState(TState argument, Func<TState, Ticket> factory)
            {
                this.argument = argument;
                this.factory = factory;
            }

            public void GetResult(short token)
            {
                awaiter.GetResult();
            }

            public TicketStatus GetStatus(short token)
            {
                var f = Interlocked.Exchange(ref factory, null);
                if (f != null)
                {
                    task = f(argument);
                    awaiter = task.GetAwaiter();
                }

                return task.Status;
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                awaiter.SourceOnCompleted(continuation, state);
            }

            public TicketStatus UnsafeGetStatus()
            {
                return task.Status;
            }
        }

        sealed class DeferPromiseWithState<TState, TResult> : ITicketSource<TResult>
        {
            Func<TState, Ticket<TResult>> factory;
            TState argument;
            Ticket<TResult> task;
            Ticket<TResult>.Awaiter awaiter;

            public DeferPromiseWithState(TState argument, Func<TState, Ticket<TResult>> factory)
            {
                this.argument = argument;
                this.factory = factory;
            }

            public TResult GetResult(short token)
            {
                return awaiter.GetResult();
            }

            void ITicketSource.GetResult(short token)
            {
                awaiter.GetResult();
            }

            public TicketStatus GetStatus(short token)
            {
                var f = Interlocked.Exchange(ref factory, null);
                if (f != null)
                {
                    task = f(argument);
                    awaiter = task.GetAwaiter();
                }

                return task.Status;
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                awaiter.SourceOnCompleted(continuation, state);
            }

            public TicketStatus UnsafeGetStatus()
            {
                return task.Status;
            }
        }

        sealed class NeverPromise<T> : ITicketSource<T>
        {
            static readonly Action<object> cancellationCallback = CancellationCallback;

            CancellationToken cancellationToken;
            TicketCompletionSourceCore<T> core;

            public NeverPromise(CancellationToken cancellationToken)
            {
                this.cancellationToken = cancellationToken;
                if (this.cancellationToken.CanBeCanceled)
                {
                    this.cancellationToken.RegisterWithoutCaptureExecutionContext(cancellationCallback, this);
                }
            }

            static void CancellationCallback(object state)
            {
                var self = (NeverPromise<T>)state;
                self.core.TrySetCanceled(self.cancellationToken);
            }

            public T GetResult(short token)
            {
                return core.GetResult(token);
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

            void ITicketSource.GetResult(short token)
            {
                core.GetResult(token);
            }
        }
    }

    internal static class CompletedTasks
    {
        public static readonly Ticket<AsyncUnit> AsyncUnit = Ticket.FromResult(CLabs.Tickets.AsyncUnit.Default);
        public static readonly Ticket<bool> True = Ticket.FromResult(true);
        public static readonly Ticket<bool> False = Ticket.FromResult(false);
        public static readonly Ticket<int> Zero = Ticket.FromResult(0);
        public static readonly Ticket<int> MinusOne = Ticket.FromResult(-1);
        public static readonly Ticket<int> One = Ticket.FromResult(1);
    }
}
