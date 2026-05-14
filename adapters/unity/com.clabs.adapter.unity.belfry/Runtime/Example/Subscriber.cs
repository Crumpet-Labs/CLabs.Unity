using System;
using Buttr.Core;
using CLabs.Belfry;
using UnityEngine;

namespace CLabs.Adapters {
    public sealed class Subscriber : MonoBehaviour {
        private IDisposable m_Subscription;
        private BellRope m_Rope;

        private void Awake() {
            m_Rope = Application<IBellTower>.Get().Rope(typeof(Publisher));
        }

        private void OnEnable() {
            // Batch subscription — all in one place, one IDisposable
            m_Subscription = m_Rope.On(
                new BellListener<bool>(Foo),
                new BellListener<int>(Bar),
                new BellListener<ExampleMessage>(Fox)
            );
        }

        private void OnDisable() => m_Subscription?.Dispose();

        private void Foo(in bool state) => Debug.Log($"Foo: {state}");
        private void Bar(in int state) => Debug.Log($"Bar: {state}");
        private void Fox(in ExampleMessage message) => Debug.Log($"Fox: {message.Message}");
    }
}
