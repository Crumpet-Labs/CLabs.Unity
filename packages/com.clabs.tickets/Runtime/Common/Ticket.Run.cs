#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading;

namespace CLabs.Tickets
{
    public partial struct Ticket
    {
        #region OBSOLETE_RUN

        [Obsolete("Ticket.Run is similar as Task.Run, it uses ThreadPool. For equivalent behaviour, use Ticket.RunOnThreadPool instead. If you don't want to use ThreadPool, you can use Ticket.Void(async void) or Ticket.Create(async Ticket) too.")]
        public static Ticket Run(Action action, bool configureAwait = true, CancellationToken cancellationToken = default)
        {
            return RunOnThreadPool(action, configureAwait, cancellationToken);
        }

        [Obsolete("Ticket.Run is similar as Task.Run, it uses ThreadPool. For equivalent behaviour, use Ticket.RunOnThreadPool instead. If you don't want to use ThreadPool, you can use Ticket.Void(async void) or Ticket.Create(async Ticket) too.")]
        public static Ticket Run(Action<object> action, object state, bool configureAwait = true, CancellationToken cancellationToken = default)
        {
            return RunOnThreadPool(action, state, configureAwait, cancellationToken);
        }

        [Obsolete("Ticket.Run is similar as Task.Run, it uses ThreadPool. For equivalent behaviour, use Ticket.RunOnThreadPool instead. If you don't want to use ThreadPool, you can use Ticket.Void(async void) or Ticket.Create(async Ticket) too.")]
        public static Ticket Run(Func<Ticket> action, bool configureAwait = true, CancellationToken cancellationToken = default)
        {
            return RunOnThreadPool(action, configureAwait, cancellationToken);
        }

        [Obsolete("Ticket.Run is similar as Task.Run, it uses ThreadPool. For equivalent behaviour, use Ticket.RunOnThreadPool instead. If you don't want to use ThreadPool, you can use Ticket.Void(async void) or Ticket.Create(async Ticket) too.")]
        public static Ticket Run(Func<object, Ticket> action, object state, bool configureAwait = true, CancellationToken cancellationToken = default)
        {
            return RunOnThreadPool(action, state, configureAwait, cancellationToken);
        }

        [Obsolete("Ticket.Run is similar as Task.Run, it uses ThreadPool. For equivalent behaviour, use Ticket.RunOnThreadPool instead. If you don't want to use ThreadPool, you can use Ticket.Void(async void) or Ticket.Create(async Ticket) too.")]
        public static Ticket<T> Run<T>(Func<T> func, bool configureAwait = true, CancellationToken cancellationToken = default)
        {
            return RunOnThreadPool(func, configureAwait, cancellationToken);
        }

        [Obsolete("Ticket.Run is similar as Task.Run, it uses ThreadPool. For equivalent behaviour, use Ticket.RunOnThreadPool instead. If you don't want to use ThreadPool, you can use Ticket.Void(async void) or Ticket.Create(async Ticket) too.")]
        public static Ticket<T> Run<T>(Func<Ticket<T>> func, bool configureAwait = true, CancellationToken cancellationToken = default)
        {
            return RunOnThreadPool(func, configureAwait, cancellationToken);
        }

        [Obsolete("Ticket.Run is similar as Task.Run, it uses ThreadPool. For equivalent behaviour, use Ticket.RunOnThreadPool instead. If you don't want to use ThreadPool, you can use Ticket.Void(async void) or Ticket.Create(async Ticket) too.")]
        public static Ticket<T> Run<T>(Func<object, T> func, object state, bool configureAwait = true, CancellationToken cancellationToken = default)
        {
            return RunOnThreadPool(func, state, configureAwait, cancellationToken);
        }

        [Obsolete("Ticket.Run is similar as Task.Run, it uses ThreadPool. For equivalent behaviour, use Ticket.RunOnThreadPool instead. If you don't want to use ThreadPool, you can use Ticket.Void(async void) or Ticket.Create(async Ticket) too.")]
        public static Ticket<T> Run<T>(Func<object, Ticket<T>> func, object state, bool configureAwait = true, CancellationToken cancellationToken = default)
        {
            return RunOnThreadPool(func, state, configureAwait, cancellationToken);
        }

        #endregion

        /// <summary>Run action on the threadPool and return to main thread if configureAwait = true.</summary>
        public static async Ticket RunOnThreadPool(Action action, bool configureAwait = true, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await Ticket.SwitchToThreadPool();

            cancellationToken.ThrowIfCancellationRequested();

            if (configureAwait)
            {
                try
                {
                    action();
                }
                finally
                {
                    await Ticket.Yield();
                }
            }
            else
            {
                action();
            }

            cancellationToken.ThrowIfCancellationRequested();
        }

        /// <summary>Run action on the threadPool and return to main thread if configureAwait = true.</summary>
        public static async Ticket RunOnThreadPool(Action<object> action, object state, bool configureAwait = true, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await Ticket.SwitchToThreadPool();

            cancellationToken.ThrowIfCancellationRequested();

            if (configureAwait)
            {
                try
                {
                    action(state);
                }
                finally
                {
                    await Ticket.Yield();
                }
            }
            else
            {
                action(state);
            }

            cancellationToken.ThrowIfCancellationRequested();
        }

        /// <summary>Run action on the threadPool and return to main thread if configureAwait = true.</summary>
        public static async Ticket RunOnThreadPool(Func<Ticket> action, bool configureAwait = true, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await Ticket.SwitchToThreadPool();

            cancellationToken.ThrowIfCancellationRequested();

            if (configureAwait)
            {
                try
                {
                    await action();
                }
                finally
                {
                    await Ticket.Yield();
                }
            }
            else
            {
                await action();
            }

            cancellationToken.ThrowIfCancellationRequested();
        }

        /// <summary>Run action on the threadPool and return to main thread if configureAwait = true.</summary>
        public static async Ticket RunOnThreadPool(Func<object, Ticket> action, object state, bool configureAwait = true, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await Ticket.SwitchToThreadPool();

            cancellationToken.ThrowIfCancellationRequested();

            if (configureAwait)
            {
                try
                {
                    await action(state);
                }
                finally
                {
                    await Ticket.Yield();
                }
            }
            else
            {
                await action(state);
            }

            cancellationToken.ThrowIfCancellationRequested();
        }

        /// <summary>Run action on the threadPool and return to main thread if configureAwait = true.</summary>
        public static async Ticket<T> RunOnThreadPool<T>(Func<T> func, bool configureAwait = true, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await Ticket.SwitchToThreadPool();

            cancellationToken.ThrowIfCancellationRequested();

            if (configureAwait)
            {
                try
                {
                    return func();
                }
                finally
                {
                    await Ticket.Yield();
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
            else
            {
                return func();
            }
        }

        /// <summary>Run action on the threadPool and return to main thread if configureAwait = true.</summary>
        public static async Ticket<T> RunOnThreadPool<T>(Func<Ticket<T>> func, bool configureAwait = true, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await Ticket.SwitchToThreadPool();

            cancellationToken.ThrowIfCancellationRequested();

            if (configureAwait)
            {
                try
                {
                    return await func();
                }
                finally
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await Ticket.Yield();
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
            else
            {
                var result = await func();
                cancellationToken.ThrowIfCancellationRequested();
                return result;
            }
        }

        /// <summary>Run action on the threadPool and return to main thread if configureAwait = true.</summary>
        public static async Ticket<T> RunOnThreadPool<T>(Func<object, T> func, object state, bool configureAwait = true, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await Ticket.SwitchToThreadPool();

            cancellationToken.ThrowIfCancellationRequested();

            if (configureAwait)
            {
                try
                {
                    return func(state);
                }
                finally
                {
                    await Ticket.Yield();
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
            else
            {
                return func(state);
            }
        }

        /// <summary>Run action on the threadPool and return to main thread if configureAwait = true.</summary>
        public static async Ticket<T> RunOnThreadPool<T>(Func<object, Ticket<T>> func, object state, bool configureAwait = true, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await Ticket.SwitchToThreadPool();

            cancellationToken.ThrowIfCancellationRequested();

            if (configureAwait)
            {
                try
                {
                    return await func(state);
                }
                finally
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await Ticket.Yield();
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
            else
            {
                var result = await func(state);
                cancellationToken.ThrowIfCancellationRequested();
                return result;
            }
        }
    }
}

