#pragma warning disable CS1591

using CLabs.Tickets.Internal;
using System;
using System.Linq;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CLabs.Tickets.CompilerServices
{
    // #ENABLE_IL2CPP in this file is to avoid bug of IL2CPP VM.
    // Issue is tracked on https://issuetracker.unity3d.com/issues/il2cpp-incorrect-results-when-calling-a-method-from-outside-class-in-a-struct
    // but currently it is labeled `Won't Fix`.

    internal interface IStateMachineRunner
    {
        Action MoveNext { get; }
        void Return();

#if ENABLE_IL2CPP
        Action ReturnAction { get; }
#endif
    }

    internal interface IStateMachineRunnerPromise : ITicketSource
    {
        Action MoveNext { get; }
        Ticket Task { get; }
        void SetResult();
        void SetException(Exception exception);
    }

    internal interface IStateMachineRunnerPromise<T> : ITicketSource<T>
    {
        Action MoveNext { get; }
        Ticket<T> Task { get; }
        void SetResult(T result);
        void SetException(Exception exception);
    }

    internal static class StateMachineUtility
    {
        // Get AsyncStateMachine internal state to check IL2CPP bug
        public static int GetState(IAsyncStateMachine stateMachine)
        {
            var info = stateMachine.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .First(x => x.Name.EndsWith("__state"));
            return (int)info.GetValue(stateMachine);
        }
    }

    internal sealed class AsyncTicketVoid<TStateMachine> : IStateMachineRunner, ITaskPoolNode<AsyncTicketVoid<TStateMachine>>, ITicketSource
        where TStateMachine : IAsyncStateMachine
    {
        static TaskPool<AsyncTicketVoid<TStateMachine>> pool;

#if ENABLE_IL2CPP
        public Action ReturnAction { get; }
#endif

        TStateMachine stateMachine;

        public Action MoveNext { get; }

        public AsyncTicketVoid()
        {
            MoveNext = Run;
#if ENABLE_IL2CPP
            ReturnAction = Return;
#endif
        }

        public static void SetStateMachine(ref TStateMachine stateMachine, ref IStateMachineRunner runnerFieldRef)
        {
            if (!pool.TryPop(out var result))
            {
                result = new AsyncTicketVoid<TStateMachine>();
            }
            TaskTracker.TrackActiveTask(result, 3);

            runnerFieldRef = result; // set runner before copied.
            result.stateMachine = stateMachine; // copy struct StateMachine(in release build).
        }

        static AsyncTicketVoid()
        {
            TaskPool.RegisterSizeGetter(typeof(AsyncTicketVoid<TStateMachine>), () => pool.Size);
        }

        AsyncTicketVoid<TStateMachine> nextNode;
        public ref AsyncTicketVoid<TStateMachine> NextNode => ref nextNode;

        public void Return()
        {
            TaskTracker.RemoveTracking(this);
            stateMachine = default;
            pool.TryPush(this);
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Run()
        {
            stateMachine.MoveNext();
        }

        // dummy interface implementation for TaskTracker.

        TicketStatus ITicketSource.GetStatus(short token)
        {
            return TicketStatus.Pending;
        }

        TicketStatus ITicketSource.UnsafeGetStatus()
        {
            return TicketStatus.Pending;
        }

        void ITicketSource.OnCompleted(Action<object> continuation, object state, short token)
        {
        }

        void ITicketSource.GetResult(short token)
        {
        }
    }

    internal sealed class AsyncTicket<TStateMachine> : IStateMachineRunnerPromise, ITicketSource, ITaskPoolNode<AsyncTicket<TStateMachine>>
        where TStateMachine : IAsyncStateMachine
    {
        static TaskPool<AsyncTicket<TStateMachine>> pool;

#if ENABLE_IL2CPP
        readonly Action returnDelegate;  
#endif
        public Action MoveNext { get; }

        TStateMachine stateMachine;
        TicketCompletionSourceCore<AsyncUnit> core;

        AsyncTicket()
        {
            MoveNext = Run;
#if ENABLE_IL2CPP
            returnDelegate = Return;
#endif
        }

        public static void SetStateMachine(ref TStateMachine stateMachine, ref IStateMachineRunnerPromise runnerPromiseFieldRef)
        {
            if (!pool.TryPop(out var result))
            {
                result = new AsyncTicket<TStateMachine>();
            }
            TaskTracker.TrackActiveTask(result, 3);

            runnerPromiseFieldRef = result; // set runner before copied.
            result.stateMachine = stateMachine; // copy struct StateMachine(in release build).
        }

        AsyncTicket<TStateMachine> nextNode;
        public ref AsyncTicket<TStateMachine> NextNode => ref nextNode;

        static AsyncTicket()
        {
            TaskPool.RegisterSizeGetter(typeof(AsyncTicket<TStateMachine>), () => pool.Size);
        }

        void Return()
        {
            TaskTracker.RemoveTracking(this);
            core.Reset();
            stateMachine = default;
            pool.TryPush(this);
        }

        bool TryReturn()
        {
            TaskTracker.RemoveTracking(this);
            core.Reset();
            stateMachine = default;
            return pool.TryPush(this);
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Run()
        {
            stateMachine.MoveNext();
        }

        public Ticket Task
        {
            [DebuggerHidden]
            get
            {
                return new Ticket(this, core.Version);
            }
        }

        [DebuggerHidden]
        public void SetResult()
        {
            core.TrySetResult(AsyncUnit.Default);
        }

        [DebuggerHidden]
        public void SetException(Exception exception)
        {
            core.TrySetException(exception);
        }

        [DebuggerHidden]
        public void GetResult(short token)
        {
            try
            {
                core.GetResult(token);
            }
            finally
            {
#if ENABLE_IL2CPP
                // workaround for IL2CPP bug.
                TicketRuntime.AddContinuation(PlayerLoopTiming.LastPostLateUpdate, returnDelegate);
#else
                TryReturn();
#endif
            }
        }

        [DebuggerHidden]
        public TicketStatus GetStatus(short token)
        {
            return core.GetStatus(token);
        }

        [DebuggerHidden]
        public TicketStatus UnsafeGetStatus()
        {
            return core.UnsafeGetStatus();
        }

        [DebuggerHidden]
        public void OnCompleted(Action<object> continuation, object state, short token)
        {
            core.OnCompleted(continuation, state, token);
        }
    }

    internal sealed class AsyncTicket<TStateMachine, T> : IStateMachineRunnerPromise<T>, ITicketSource<T>, ITaskPoolNode<AsyncTicket<TStateMachine, T>>
        where TStateMachine : IAsyncStateMachine
    {
        static TaskPool<AsyncTicket<TStateMachine, T>> pool;

#if ENABLE_IL2CPP
        readonly Action returnDelegate;  
#endif

        public Action MoveNext { get; }

        TStateMachine stateMachine;
        TicketCompletionSourceCore<T> core;

        AsyncTicket()
        {
            MoveNext = Run;
#if ENABLE_IL2CPP
            returnDelegate = Return;
#endif
        }

        public static void SetStateMachine(ref TStateMachine stateMachine, ref IStateMachineRunnerPromise<T> runnerPromiseFieldRef)
        {
            if (!pool.TryPop(out var result))
            {
                result = new AsyncTicket<TStateMachine, T>();
            }
            TaskTracker.TrackActiveTask(result, 3);

            runnerPromiseFieldRef = result; // set runner before copied.
            result.stateMachine = stateMachine; // copy struct StateMachine(in release build).
        }

        AsyncTicket<TStateMachine, T> nextNode;
        public ref AsyncTicket<TStateMachine, T> NextNode => ref nextNode;

        static AsyncTicket()
        {
            TaskPool.RegisterSizeGetter(typeof(AsyncTicket<TStateMachine, T>), () => pool.Size);
        }

        void Return()
        {
            TaskTracker.RemoveTracking(this);
            core.Reset();
            stateMachine = default;
            pool.TryPush(this);
        }

        bool TryReturn()
        {
            TaskTracker.RemoveTracking(this);
            core.Reset();
            stateMachine = default;
            return pool.TryPush(this);
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Run()
        {
            stateMachine.MoveNext();
        }

        public Ticket<T> Task
        {
            [DebuggerHidden]
            get
            {
                return new Ticket<T>(this, core.Version);
            }
        }

        [DebuggerHidden]
        public void SetResult(T result)
        {
            core.TrySetResult(result);
        }

        [DebuggerHidden]
        public void SetException(Exception exception)
        {
            core.TrySetException(exception);
        }

        [DebuggerHidden]
        public T GetResult(short token)
        {
            try
            {
                return core.GetResult(token);
            }
            finally
            {
#if ENABLE_IL2CPP
                // workaround for IL2CPP bug.
                TicketRuntime.AddContinuation(PlayerLoopTiming.LastPostLateUpdate, returnDelegate);
#else
                TryReturn();
#endif
            }
        }

        [DebuggerHidden]
        void ITicketSource.GetResult(short token)
        {
            GetResult(token);
        }

        [DebuggerHidden]
        public TicketStatus GetStatus(short token)
        {
            return core.GetStatus(token);
        }

        [DebuggerHidden]
        public TicketStatus UnsafeGetStatus()
        {
            return core.UnsafeGetStatus();
        }

        [DebuggerHidden]
        public void OnCompleted(Action<object> continuation, object state, short token)
        {
            core.OnCompleted(continuation, state, token);
        }
    }
}

