using System;

namespace CLabs.Tickets
{
    public abstract class MoveNextSource : ITicketSource<bool>
    {
        protected TicketCompletionSourceCore<bool> completionSource;

        public bool GetResult(short token)
        {
            return completionSource.GetResult(token);
        }

        public TicketStatus GetStatus(short token)
        {
            return completionSource.GetStatus(token);
        }

        public void OnCompleted(Action<object> continuation, object state, short token)
        {
            completionSource.OnCompleted(continuation, state, token);
        }

        public TicketStatus UnsafeGetStatus()
        {
            return completionSource.UnsafeGetStatus();
        }

        void ITicketSource.GetResult(short token)
        {
            completionSource.GetResult(token);
        }

        protected bool TryGetResult<T>(Ticket<T>.Awaiter awaiter, out T result)
        {
            try
            {
                result = awaiter.GetResult();
                return true;
            }
            catch (Exception ex)
            {
                completionSource.TrySetException(ex);
                result = default;
                return false;
            }
        }

        protected bool TryGetResult(Ticket.Awaiter awaiter)
        {
            try
            {
                awaiter.GetResult();
                return true;
            }
            catch (Exception ex)
            {
                completionSource.TrySetException(ex);
                return false;
            }
        }
    }
}
