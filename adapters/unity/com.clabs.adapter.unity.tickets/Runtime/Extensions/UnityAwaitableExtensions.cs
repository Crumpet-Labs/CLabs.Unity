#if UNITY_2023_1_OR_NEWER
namespace CLabs.Tickets
{
    public static class UnityAwaitableExtensions
    {
        public static async Ticket AsTicket(this UnityEngine.Awaitable awaitable)
        {
            await awaitable;
        }
        
        public static async Ticket<T> AsTicket<T>(this UnityEngine.Awaitable<T> awaitable)
        {
            return await awaitable;
        }
    }
}
#endif
