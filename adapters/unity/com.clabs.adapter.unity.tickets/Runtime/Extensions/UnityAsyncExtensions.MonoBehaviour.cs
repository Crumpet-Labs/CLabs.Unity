using System;
using System.Threading;

namespace CLabs.Tickets
{
    public static partial class UnityAsyncExtensions
    {
        public static Ticket StartAsyncCoroutine(this UnityEngine.MonoBehaviour monoBehaviour, Func<CancellationToken, Ticket> asyncCoroutine)
        {
            var token = monoBehaviour.GetCancellationTokenOnDestroy();
            return asyncCoroutine(token);
        }
    }
}