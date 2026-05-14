using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static ITicketAsyncEnumerable<TSource> ToTicketAsyncEnumerable<TSource>(this IEnumerable<TSource> source)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return new ToTicketAsyncEnumerable<TSource>(source);
        }

        public static ITicketAsyncEnumerable<TSource> ToTicketAsyncEnumerable<TSource>(this Task<TSource> source)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return new ToTicketAsyncEnumerableTask<TSource>(source);
        }

        public static ITicketAsyncEnumerable<TSource> ToTicketAsyncEnumerable<TSource>(this Ticket<TSource> source)
        {
            return new ToTicketAsyncEnumerableTicket<TSource>(source);
        }

        public static ITicketAsyncEnumerable<TSource> ToTicketAsyncEnumerable<TSource>(this IObservable<TSource> source)
        {
            Error.ThrowArgumentNullException(source, nameof(source));

            return new ToTicketAsyncEnumerableObservable<TSource>(source);
        }
    }

    internal sealed class ToTicketAsyncEnumerable<T> : ITicketAsyncEnumerable<T>
    {
        readonly IEnumerable<T> source;

        public ToTicketAsyncEnumerable(IEnumerable<T> source)
        {
            this.source = source;
        }

        public ITicketAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _ToTicketAsyncEnumerable(source, cancellationToken);
        }

        class _ToTicketAsyncEnumerable : ITicketAsyncEnumerator<T>
        {
            readonly IEnumerable<T> source;
            CancellationToken cancellationToken;

            IEnumerator<T> enumerator;

            public _ToTicketAsyncEnumerable(IEnumerable<T> source, CancellationToken cancellationToken)
            {
                this.source = source;
                this.cancellationToken = cancellationToken;
            }

            public T Current => enumerator.Current;

            public Ticket<bool> MoveNextAsync()
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (enumerator == null)
                {
                    enumerator = source.GetEnumerator();
                }

                if (enumerator.MoveNext())
                {
                    return CompletedTasks.True;
                }

                return CompletedTasks.False;
            }

            public Ticket DisposeAsync()
            {
                enumerator.Dispose();
                return default;
            }
        }
    }

    internal sealed class ToTicketAsyncEnumerableTask<T> : ITicketAsyncEnumerable<T>
    {
        readonly Task<T> source;

        public ToTicketAsyncEnumerableTask(Task<T> source)
        {
            this.source = source;
        }

        public ITicketAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _ToTicketAsyncEnumerableTask(source, cancellationToken);
        }

        class _ToTicketAsyncEnumerableTask : ITicketAsyncEnumerator<T>
        {
            readonly Task<T> source;
            CancellationToken cancellationToken;

            T current;
            bool called;

            public _ToTicketAsyncEnumerableTask(Task<T> source, CancellationToken cancellationToken)
            {
                this.source = source;
                this.cancellationToken = cancellationToken;

                this.called = false;
            }

            public T Current => current;

            public async Ticket<bool> MoveNextAsync()
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (called)
                {
                    return false;
                }
                called = true;

                current = await source;
                return true;
            }

            public Ticket DisposeAsync()
            {
                return default;
            }
        }
    }

    internal sealed class ToTicketAsyncEnumerableTicket<T> : ITicketAsyncEnumerable<T>
    {
        readonly Ticket<T> source;

        public ToTicketAsyncEnumerableTicket(Ticket<T> source)
        {
            this.source = source;
        }

        public ITicketAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _ToTicketAsyncEnumerableTicket(source, cancellationToken);
        }

        class _ToTicketAsyncEnumerableTicket : ITicketAsyncEnumerator<T>
        {
            readonly Ticket<T> source;
            CancellationToken cancellationToken;

            T current;
            bool called;

            public _ToTicketAsyncEnumerableTicket(Ticket<T> source, CancellationToken cancellationToken)
            {
                this.source = source;
                this.cancellationToken = cancellationToken;

                this.called = false;
            }

            public T Current => current;

            public async Ticket<bool> MoveNextAsync()
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (called)
                {
                    return false;
                }
                called = true;

                current = await source;
                return true;
            }

            public Ticket DisposeAsync()
            {
                return default;
            }
        }
    }

    internal sealed class ToTicketAsyncEnumerableObservable<T> : ITicketAsyncEnumerable<T>
    {
        readonly IObservable<T> source;

        public ToTicketAsyncEnumerableObservable(IObservable<T> source)
        {
            this.source = source;
        }

        public ITicketAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _ToTicketAsyncEnumerableObservable(source, cancellationToken);
        }

        class _ToTicketAsyncEnumerableObservable : MoveNextSource, ITicketAsyncEnumerator<T>, IObserver<T>
        {
            static readonly Action<object> OnCanceledDelegate = OnCanceled;

            readonly IObservable<T> source;
            CancellationToken cancellationToken;


            bool useCachedCurrent;
            T current;
            bool subscribeCompleted;
            readonly Queue<T> queuedResult;
            Exception error;
            IDisposable subscription;
            CancellationTokenRegistration cancellationTokenRegistration;

            public _ToTicketAsyncEnumerableObservable(IObservable<T> source, CancellationToken cancellationToken)
            {
                this.source = source;
                this.cancellationToken = cancellationToken;
                this.queuedResult = new Queue<T>();

                if (cancellationToken.CanBeCanceled)
                {
                    cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(OnCanceledDelegate, this);
                }
            }

            public T Current
            {
                get
                {
                    if (useCachedCurrent)
                    {
                        return current;
                    }

                    lock (queuedResult)
                    {
                        if (queuedResult.Count != 0)
                        {
                            current = queuedResult.Dequeue();
                            useCachedCurrent = true;
                            return current;
                        }
                        else
                        {
                            return default; // undefined.
                        }
                    }
                }
            }

            public Ticket<bool> MoveNextAsync()
            {
                lock (queuedResult)
                {
                    useCachedCurrent = false;

                    if (cancellationToken.IsCancellationRequested)
                    {
                        return Ticket.FromCanceled<bool>(cancellationToken);
                    }

                    if (subscription == null)
                    {
                        subscription = source.Subscribe(this);
                    }

                    if (error != null)
                    {
                        return Ticket.FromException<bool>(error);
                    }

                    if (queuedResult.Count != 0)
                    {
                        return CompletedTasks.True;
                    }

                    if (subscribeCompleted)
                    {
                        return CompletedTasks.False;
                    }

                    completionSource.Reset();
                    return new Ticket<bool>(this, completionSource.Version);
                }
            }

            public Ticket DisposeAsync()
            {
                subscription.Dispose();
                cancellationTokenRegistration.Dispose();
                completionSource.Reset();
                return default;
            }

            public void OnCompleted()
            {
                lock (queuedResult)
                {
                    subscribeCompleted = true;
                    completionSource.TrySetResult(false);
                }
            }

            public void OnError(Exception error)
            {
                lock (queuedResult)
                {
                    this.error = error;
                    completionSource.TrySetException(error);
                }
            }

            public void OnNext(T value)
            {
                lock (queuedResult)
                {
                    queuedResult.Enqueue(value);
                    completionSource.TrySetResult(true); // include callback execution, too long lock?
                }
            }

            static void OnCanceled(object state)
            {
                var self = (_ToTicketAsyncEnumerableObservable)state;
                lock (self.queuedResult)
                {
                    self.completionSource.TrySetCanceled(self.cancellationToken);
                }
            }
        }
    }
}































































































































































































































































































































































































































































































































































































































































































































































































