using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace CLabs.Utility
{
    public static class CoroutineUtils
    {
        /**
      * Usage: StartCoroutine(CoroutineUtils.Chain(...))
      * For example:
      *     StartCoroutine(CoroutineUtils.Chain(
      *         CoroutineUtils.Do(() => Debug.Log("A")),
      *         CoroutineUtils.WaitForSeconds(2),
      *         CoroutineUtils.Do(() => Debug.Log("B"))));
      */
        public static IEnumerator ExecuteCoroutines(this MonoBehaviour behaviour, params IEnumerator[] actions)
            => actions.Select(behaviour.StartCoroutine).GetEnumerator();
        
        // public static IEnumerator ChainCoroutines(this MonoBehaviour behaviour, params IEnumerator[] actions)
        //     => behaviour.StartCoroutine(actions.GetEnumerator());

        public static IEnumerator Chain(this IEnumerator[] actions)
            => actions.GetEnumerator();
        
        public static IEnumerator DelaySeconds(Action action, float delay)
        {
            yield return new WaitForSeconds(delay);
            action();
        }

        public static IEnumerator WaitUntil(Func<bool> predicate)
        {
            yield return new WaitUntil(predicate);
        }

        public static IEnumerator WaitWhile(Func<bool> predicate)
        {
            yield return new WaitWhile(predicate);
        }

        public static IEnumerator WaitForUpdate()
        {
            yield return null;
        }

        public static IEnumerator WaitForFixedUpdate()
        {
            yield return new WaitForFixedUpdate();
        }
        
        public static IEnumerator WaitForEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
        }

        public static IEnumerator WaitForSeconds(float time)
        {
            yield return new WaitForSeconds(time);
        }
        
        public static IEnumerator WaitForSecondsRealtime(float time)
        {
            yield return new WaitForSecondsRealtime(time);
        }

        public static IEnumerator Do(Action action)
        {
            action();
            yield return null;
        }
    }
}