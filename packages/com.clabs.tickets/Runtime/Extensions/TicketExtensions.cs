#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets
{
    public static partial class TicketExtensions
    {
        /// <summary>
        /// Convert Task[T] -> Ticket[T].
        /// </summary>
        public static Ticket<T> AsTicket<T>(this Task<T> task, bool useCurrentSynchronizationContext = true)
        {
            var promise = new TicketCompletionSource<T>();

            task.ContinueWith((x, state) =>
            {
                var p = (TicketCompletionSource<T>)state;

                switch (x.Status)
                {
                    case TaskStatus.Canceled:
                        p.TrySetCanceled();
                        break;
                    case TaskStatus.Faulted:
                        p.TrySetException(x.Exception.InnerException ?? x.Exception);
                        break;
                    case TaskStatus.RanToCompletion:
                        p.TrySetResult(x.Result);
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }, promise, useCurrentSynchronizationContext ? TaskScheduler.FromCurrentSynchronizationContext() : TaskScheduler.Current);

            return promise.Task;
        }

        /// <summary>
        /// Convert Task -> Ticket.
        /// </summary>
        public static Ticket AsTicket(this Task task, bool useCurrentSynchronizationContext = true)
        {
            var promise = new TicketCompletionSource();

            task.ContinueWith((x, state) =>
            {
                var p = (TicketCompletionSource)state;

                switch (x.Status)
                {
                    case TaskStatus.Canceled:
                        p.TrySetCanceled();
                        break;
                    case TaskStatus.Faulted:
                        p.TrySetException(x.Exception.InnerException ?? x.Exception);
                        break;
                    case TaskStatus.RanToCompletion:
                        p.TrySetResult();
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }, promise, useCurrentSynchronizationContext ? TaskScheduler.FromCurrentSynchronizationContext() : TaskScheduler.Current);

            return promise.Task;
        }

        public static Task<T> AsTask<T>(this Ticket<T> task)
        {
            try
            {
                Ticket<T>.Awaiter awaiter;
                try
                {
                    awaiter = task.GetAwaiter();
                }
                catch (Exception ex)
                {
                    return Task.FromException<T>(ex);
                }

                if (awaiter.IsCompleted)
                {
                    try
                    {
                        var result = awaiter.GetResult();
                        return Task.FromResult(result);
                    }
                    catch (Exception ex)
                    {
                        return Task.FromException<T>(ex);
                    }
                }

                var tcs = new TaskCompletionSource<T>();

                awaiter.SourceOnCompleted(state =>
                {
                    using (var tuple = (StateTuple<TaskCompletionSource<T>, Ticket<T>.Awaiter>)state)
                    {
                        var (inTcs, inAwaiter) = tuple;
                        try
                        {
                            var result = inAwaiter.GetResult();
                            inTcs.SetResult(result);
                        }
                        catch (Exception ex)
                        {
                            inTcs.SetException(ex);
                        }
                    }
                }, StateTuple.Create(tcs, awaiter));

                return tcs.Task;
            }
            catch (Exception ex)
            {
                return Task.FromException<T>(ex);
            }
        }

        public static Task AsTask(this Ticket task)
        {
            try
            {
                Ticket.Awaiter awaiter;
                try
                {
                    awaiter = task.GetAwaiter();
                }
                catch (Exception ex)
                {
                    return Task.FromException(ex);
                }

                if (awaiter.IsCompleted)
                {
                    try
                    {
                        awaiter.GetResult(); // check token valid on Succeeded
                        return Task.CompletedTask;
                    }
                    catch (Exception ex)
                    {
                        return Task.FromException(ex);
                    }
                }

                var tcs = new TaskCompletionSource<object>();

                awaiter.SourceOnCompleted(state =>
                {
                    using (var tuple = (StateTuple<TaskCompletionSource<object>, Ticket.Awaiter>)state)
                    {
                        var (inTcs, inAwaiter) = tuple;
                        try
                        {
                            inAwaiter.GetResult();
                            inTcs.SetResult(null);
                        }
                        catch (Exception ex)
                        {
                            inTcs.SetException(ex);
                        }
                    }
                }, StateTuple.Create(tcs, awaiter));

                return tcs.Task;
            }
            catch (Exception ex)
            {
                return Task.FromException(ex);
            }
        }

        public static AsyncLazy ToAsyncLazy(this Ticket task)
        {
            return new AsyncLazy(task);
        }

        public static AsyncLazy<T> ToAsyncLazy<T>(this Ticket<T> task)
        {
            return new AsyncLazy<T>(task);
        }

        /// <summary>
        /// Ignore task result when cancel raised first.
        /// </summary>
        public static Ticket AttachExternalCancellation(this Ticket task, CancellationToken cancellationToken)
        {
            if (!cancellationToken.CanBeCanceled)
            {
                return task;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                task.Forget();
                return Ticket.FromCanceled(cancellationToken);
            }

            if (task.Status.IsCompleted())
            {
                return task;
            }

            return new Ticket(new AttachExternalCancellationSource(task, cancellationToken), 0);
        }

        /// <summary>
        /// Ignore task result when cancel raised first.
        /// </summary>
        public static Ticket<T> AttachExternalCancellation<T>(this Ticket<T> task, CancellationToken cancellationToken)
        {
            if (!cancellationToken.CanBeCanceled)
            {
                return task;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                task.Forget();
                return Ticket.FromCanceled<T>(cancellationToken);
            }

            if (task.Status.IsCompleted())
            {
                return task;
            }

            return new Ticket<T>(new AttachExternalCancellationSource<T>(task, cancellationToken), 0);
        }

        sealed class AttachExternalCancellationSource : ITicketSource
        {
            static readonly Action<object> cancellationCallbackDelegate = CancellationCallback;

            CancellationToken cancellationToken;
            CancellationTokenRegistration tokenRegistration;
            TicketCompletionSourceCore<AsyncUnit> core;

            public AttachExternalCancellationSource(Ticket task, CancellationToken cancellationToken)
            {
                this.cancellationToken = cancellationToken;
                this.tokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(cancellationCallbackDelegate, this);
                RunTask(task).Forget();
            }

            async TicketVoid RunTask(Ticket task)
            {
                try
                {
                    await task;
                    core.TrySetResult(AsyncUnit.Default);
                }
                catch (Exception ex)
                {
                    core.TrySetException(ex);
                }
                finally
                {
                    tokenRegistration.Dispose();
                }
            }

            static void CancellationCallback(object state)
            {
                var self = (AttachExternalCancellationSource)state;
                self.core.TrySetCanceled(self.cancellationToken);
            }

            public void GetResult(short token)
            {
                core.GetResult(token);
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
        }

        sealed class AttachExternalCancellationSource<T> : ITicketSource<T>
        {
            static readonly Action<object> cancellationCallbackDelegate = CancellationCallback;

            CancellationToken cancellationToken;
            CancellationTokenRegistration tokenRegistration;
            TicketCompletionSourceCore<T> core;

            public AttachExternalCancellationSource(Ticket<T> task, CancellationToken cancellationToken)
            {
                this.cancellationToken = cancellationToken;
                this.tokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(cancellationCallbackDelegate, this);
                RunTask(task).Forget();
            }

            async TicketVoid RunTask(Ticket<T> task)
            {
                try
                {
                    core.TrySetResult(await task);
                }
                catch (Exception ex)
                {
                    core.TrySetException(ex);
                }
                finally
                {
                    tokenRegistration.Dispose();
                }
            }

            static void CancellationCallback(object state)
            {
                var self = (AttachExternalCancellationSource<T>)state;
                self.core.TrySetCanceled(self.cancellationToken);
            }

            void ITicketSource.GetResult(short token)
            {
                core.GetResult(token);
            }

            public T GetResult(short token)
            {
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
        }

#if UNITY_2018_3_OR_NEWER

        public static IEnumerator ToCoroutine<T>(this Ticket<T> task, Action<T> resultHandler = null, Action<Exception> exceptionHandler = null)
        {
            return new ToCoroutineEnumerator<T>(task, resultHandler, exceptionHandler);
        }

        public static IEnumerator ToCoroutine(this Ticket task, Action<Exception> exceptionHandler = null)
        {
            return new ToCoroutineEnumerator(task, exceptionHandler);
        }

        public static async Ticket Timeout(this Ticket task, TimeSpan timeout, DelayType delayType = DelayType.DeltaTime, PlayerLoopTiming timeoutCheckTiming = PlayerLoopTiming.Update, CancellationTokenSource taskCancellationTokenSource = null)
        {
            var delayCancellationTokenSource = new CancellationTokenSource();
            var timeoutTask = Ticket.Delay(timeout, delayType, timeoutCheckTiming, delayCancellationTokenSource.Token).SuppressCancellationThrow();

            int winArgIndex;
            bool taskResultIsCanceled;
            try
            {
                (winArgIndex, taskResultIsCanceled, _) = await Ticket.WhenAny(task.SuppressCancellationThrow(), timeoutTask);
            }
            catch
            {
                delayCancellationTokenSource.Cancel();
                delayCancellationTokenSource.Dispose();
                throw;
            }

            // timeout
            if (winArgIndex == 1)
            {
                if (taskCancellationTokenSource != null)
                {
                    taskCancellationTokenSource.Cancel();
                    taskCancellationTokenSource.Dispose();
                }

                throw new TimeoutException("Exceed Timeout:" + timeout);
            }
            else
            {
                delayCancellationTokenSource.Cancel();
                delayCancellationTokenSource.Dispose();
            }

            if (taskResultIsCanceled)
            {
                Error.ThrowOperationCanceledException();
            }
        }

        public static async Ticket<T> Timeout<T>(this Ticket<T> task, TimeSpan timeout, DelayType delayType = DelayType.DeltaTime, PlayerLoopTiming timeoutCheckTiming = PlayerLoopTiming.Update, CancellationTokenSource taskCancellationTokenSource = null)
        {
            var delayCancellationTokenSource = new CancellationTokenSource();
            var timeoutTask = Ticket.Delay(timeout, delayType, timeoutCheckTiming, delayCancellationTokenSource.Token).SuppressCancellationThrow();

            int winArgIndex;
            (bool IsCanceled, T Result) taskResult;
            try
            {
                (winArgIndex, taskResult, _) = await Ticket.WhenAny(task.SuppressCancellationThrow(), timeoutTask);
            }
            catch
            {
                delayCancellationTokenSource.Cancel();
                delayCancellationTokenSource.Dispose();
                throw;
            }

            // timeout
            if (winArgIndex == 1)
            {
                if (taskCancellationTokenSource != null)
                {
                    taskCancellationTokenSource.Cancel();
                    taskCancellationTokenSource.Dispose();
                }

                throw new TimeoutException("Exceed Timeout:" + timeout);
            }
            else
            {
                delayCancellationTokenSource.Cancel();
                delayCancellationTokenSource.Dispose();
            }

            if (taskResult.IsCanceled)
            {
                Error.ThrowOperationCanceledException();
            }

            return taskResult.Result;
        }

        /// <summary>
        /// Timeout with suppress OperationCanceledException. Returns (bool, IsCanceled).
        /// </summary>
        public static async Ticket<bool> TimeoutWithoutException(this Ticket task, TimeSpan timeout, DelayType delayType = DelayType.DeltaTime, PlayerLoopTiming timeoutCheckTiming = PlayerLoopTiming.Update, CancellationTokenSource taskCancellationTokenSource = null)
        {
            var delayCancellationTokenSource = new CancellationTokenSource();
            var timeoutTask = Ticket.Delay(timeout, delayType, timeoutCheckTiming, delayCancellationTokenSource.Token).SuppressCancellationThrow();

            int winArgIndex;
            bool taskResultIsCanceled;
            try
            {
                (winArgIndex, taskResultIsCanceled, _) = await Ticket.WhenAny(task.SuppressCancellationThrow(), timeoutTask);
            }
            catch
            {
                delayCancellationTokenSource.Cancel();
                delayCancellationTokenSource.Dispose();
                return true;
            }

            // timeout
            if (winArgIndex == 1)
            {
                if (taskCancellationTokenSource != null)
                {
                    taskCancellationTokenSource.Cancel();
                    taskCancellationTokenSource.Dispose();
                }

                return true;
            }
            else
            {
                delayCancellationTokenSource.Cancel();
                delayCancellationTokenSource.Dispose();
            }

            if (taskResultIsCanceled)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Timeout with suppress OperationCanceledException. Returns (bool IsTimeout, T Result).
        /// </summary>
        public static async Ticket<(bool IsTimeout, T Result)> TimeoutWithoutException<T>(this Ticket<T> task, TimeSpan timeout, DelayType delayType = DelayType.DeltaTime, PlayerLoopTiming timeoutCheckTiming = PlayerLoopTiming.Update, CancellationTokenSource taskCancellationTokenSource = null)
        {
            var delayCancellationTokenSource = new CancellationTokenSource();
            var timeoutTask = Ticket.Delay(timeout, delayType, timeoutCheckTiming, delayCancellationTokenSource.Token).SuppressCancellationThrow();

            int winArgIndex;
            (bool IsCanceled, T Result) taskResult;
            try
            {
                (winArgIndex, taskResult, _) = await Ticket.WhenAny(task.SuppressCancellationThrow(), timeoutTask);
            }
            catch
            {
                delayCancellationTokenSource.Cancel();
                delayCancellationTokenSource.Dispose();
                return (true, default);
            }

            // timeout
            if (winArgIndex == 1)
            {
                if (taskCancellationTokenSource != null)
                {
                    taskCancellationTokenSource.Cancel();
                    taskCancellationTokenSource.Dispose();
                }

                return (true, default);
            }
            else
            {
                delayCancellationTokenSource.Cancel();
                delayCancellationTokenSource.Dispose();
            }

            if (taskResult.IsCanceled)
            {
                return (true, default);
            }

            return (false, taskResult.Result);
        }

#endif

        public static void Forget(this Ticket task)
        {
            var awaiter = task.GetAwaiter();
            if (awaiter.IsCompleted)
            {
                try
                {
                    awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    TicketScheduler.PublishUnobservedTaskException(ex);
                }
            }
            else
            {
                awaiter.SourceOnCompleted(state =>
                {
                    using (var t = (StateTuple<Ticket.Awaiter>)state)
                    {
                        try
                        {
                            t.Item1.GetResult();
                        }
                        catch (Exception ex)
                        {
                            TicketScheduler.PublishUnobservedTaskException(ex);
                        }
                    }
                }, StateTuple.Create(awaiter));
            }
        }

        public static void Forget(this Ticket task, Action<Exception> exceptionHandler, bool handleExceptionOnMainThread = true)
        {
            if (exceptionHandler == null)
            {
                Forget(task);
            }
            else
            {
                ForgetCoreWithCatch(task, exceptionHandler, handleExceptionOnMainThread).Forget();
            }
        }

        static async TicketVoid ForgetCoreWithCatch(Ticket task, Action<Exception> exceptionHandler, bool handleExceptionOnMainThread)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                try
                {
                    if (handleExceptionOnMainThread)
                    {
#if UNITY_2018_3_OR_NEWER
                        await Ticket.SwitchToMainThread();
#endif
                    }
                    exceptionHandler(ex);
                }
                catch (Exception ex2)
                {
                    TicketScheduler.PublishUnobservedTaskException(ex2);
                }
            }
        }

        public static void Forget<T>(this Ticket<T> task)
        {
            var awaiter = task.GetAwaiter();
            if (awaiter.IsCompleted)
            {
                try
                {
                    awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    TicketScheduler.PublishUnobservedTaskException(ex);
                }
            }
            else
            {
                awaiter.SourceOnCompleted(state =>
                {
                    using (var t = (StateTuple<Ticket<T>.Awaiter>)state)
                    {
                        try
                        {
                            t.Item1.GetResult();
                        }
                        catch (Exception ex)
                        {
                            TicketScheduler.PublishUnobservedTaskException(ex);
                        }
                    }
                }, StateTuple.Create(awaiter));
            }
        }

        public static void Forget<T>(this Ticket<T> task, Action<Exception> exceptionHandler, bool handleExceptionOnMainThread = true)
        {
            if (exceptionHandler == null)
            {
                task.Forget();
            }
            else
            {
                ForgetCoreWithCatch(task, exceptionHandler, handleExceptionOnMainThread).Forget();
            }
        }

        static async TicketVoid ForgetCoreWithCatch<T>(Ticket<T> task, Action<Exception> exceptionHandler, bool handleExceptionOnMainThread)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                try
                {
                    if (handleExceptionOnMainThread)
                    {
#if UNITY_2018_3_OR_NEWER
                        await Ticket.SwitchToMainThread();
#endif
                    }
                    exceptionHandler(ex);
                }
                catch (Exception ex2)
                {
                    TicketScheduler.PublishUnobservedTaskException(ex2);
                }
            }
        }

        public static async Ticket ContinueWith<T>(this Ticket<T> task, Action<T> continuationFunction)
        {
            continuationFunction(await task);
        }

        public static async Ticket ContinueWith<T>(this Ticket<T> task, Func<T, Ticket> continuationFunction)
        {
            await continuationFunction(await task);
        }

        public static async Ticket<TR> ContinueWith<T, TR>(this Ticket<T> task, Func<T, TR> continuationFunction)
        {
            return continuationFunction(await task);
        }

        public static async Ticket<TR> ContinueWith<T, TR>(this Ticket<T> task, Func<T, Ticket<TR>> continuationFunction)
        {
            return await continuationFunction(await task);
        }

        public static async Ticket ContinueWith(this Ticket task, Action continuationFunction)
        {
            await task;
            continuationFunction();
        }

        public static async Ticket ContinueWith(this Ticket task, Func<Ticket> continuationFunction)
        {
            await task;
            await continuationFunction();
        }

        public static async Ticket<T> ContinueWith<T>(this Ticket task, Func<T> continuationFunction)
        {
            await task;
            return continuationFunction();
        }

        public static async Ticket<T> ContinueWith<T>(this Ticket task, Func<Ticket<T>> continuationFunction)
        {
            await task;
            return await continuationFunction();
        }

        public static async Ticket<T> Unwrap<T>(this Ticket<Ticket<T>> task)
        {
            return await await task;
        }

        public static async Ticket Unwrap(this Ticket<Ticket> task)
        {
            await await task;
        }

        public static async Ticket<T> Unwrap<T>(this Task<Ticket<T>> task)
        {
            return await await task;
        }

        public static async Ticket<T> Unwrap<T>(this Task<Ticket<T>> task, bool continueOnCapturedContext)
        {
            return await await task.ConfigureAwait(continueOnCapturedContext);
        }

        public static async Ticket Unwrap(this Task<Ticket> task)
        {
            await await task;
        }

        public static async Ticket Unwrap(this Task<Ticket> task, bool continueOnCapturedContext)
        {
            await await task.ConfigureAwait(continueOnCapturedContext);
        }

        public static async Ticket<T> Unwrap<T>(this Ticket<Task<T>> task)
        {
            return await await task;
        }

        public static async Ticket<T> Unwrap<T>(this Ticket<Task<T>> task, bool continueOnCapturedContext)
        {
            return await (await task).ConfigureAwait(continueOnCapturedContext);
        }

        public static async Ticket Unwrap(this Ticket<Task> task)
        {
            await await task;
        }

        public static async Ticket Unwrap(this Ticket<Task> task, bool continueOnCapturedContext)
        {
            await (await task).ConfigureAwait(continueOnCapturedContext);
        }

#if UNITY_2018_3_OR_NEWER

        sealed class ToCoroutineEnumerator : IEnumerator
        {
            bool completed;
            Ticket task;
            Action<Exception> exceptionHandler = null;
            bool isStarted = false;
            ExceptionDispatchInfo exception;

            public ToCoroutineEnumerator(Ticket task, Action<Exception> exceptionHandler)
            {
                completed = false;
                this.exceptionHandler = exceptionHandler;
                this.task = task;
            }

            async TicketVoid RunTask(Ticket task)
            {
                try
                {
                    await task;
                }
                catch (Exception ex)
                {
                    if (exceptionHandler != null)
                    {
                        exceptionHandler(ex);
                    }
                    else
                    {
                        this.exception = ExceptionDispatchInfo.Capture(ex);
                    }
                }
                finally
                {
                    completed = true;
                }
            }

            public object Current => null;

            public bool MoveNext()
            {
                if (!isStarted)
                {
                    isStarted = true;
                    RunTask(task).Forget();
                }

                if (exception != null)
                {
                    exception.Throw();
                    return false;
                }

                return !completed;
            }

            void IEnumerator.Reset()
            {
            }
        }

        sealed class ToCoroutineEnumerator<T> : IEnumerator
        {
            bool completed;
            Action<T> resultHandler = null;
            Action<Exception> exceptionHandler = null;
            bool isStarted = false;
            Ticket<T> task;
            object current = null;
            ExceptionDispatchInfo exception;

            public ToCoroutineEnumerator(Ticket<T> task, Action<T> resultHandler, Action<Exception> exceptionHandler)
            {
                completed = false;
                this.task = task;
                this.resultHandler = resultHandler;
                this.exceptionHandler = exceptionHandler;
            }

            async TicketVoid RunTask(Ticket<T> task)
            {
                try
                {
                    var value = await task;
                    current = value; // boxed if T is struct...
                    if (resultHandler != null)
                    {
                        resultHandler(value);
                    }
                }
                catch (Exception ex)
                {
                    if (exceptionHandler != null)
                    {
                        exceptionHandler(ex);
                    }
                    else
                    {
                        this.exception = ExceptionDispatchInfo.Capture(ex);
                    }
                }
                finally
                {
                    completed = true;
                }
            }

            public object Current => current;

            public bool MoveNext()
            {
                if (!isStarted)
                {
                    isStarted = true;
                    RunTask(task).Forget();
                }

                if (exception != null)
                {
                    exception.Throw();
                    return false;
                }

                return !completed;
            }

            void IEnumerator.Reset()
            {
            }
        }

#endif
    }
}

