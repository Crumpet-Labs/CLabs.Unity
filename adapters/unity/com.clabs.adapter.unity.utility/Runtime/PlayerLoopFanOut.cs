using System;
using System.Collections.Generic;

namespace CLabs.Utility {
    /// <summary>
    /// Per-type fan-out registry behind <see cref="PlayerLoopInjector"/>. Each distinct channel key owns exactly
    /// one PlayerLoop subsystem, inserted on the first subscribe and removed when the channel empties, and every
    /// subscriber of that key shares it. Disposing a subscription removes only that one callback, so sibling
    /// subscribers keep ticking. The PlayerLoop coupling is supplied through the constructor seams, leaving this
    /// type engine-agnostic and unit-testable.
    /// </summary>
    internal sealed class PlayerLoopFanOut {
        private readonly Dictionary<Type, Channel> m_Channels = new();
        private readonly Action<Type, Action> m_InsertSubsystem;
        private readonly Action<Type> m_RemoveSubsystem;

        public PlayerLoopFanOut(Action<Type, Action> insertSubsystem, Action<Type> removeSubsystem) {
            m_InsertSubsystem = insertSubsystem;
            m_RemoveSubsystem = removeSubsystem;
        }

        public IDisposable Subscribe(Type channelKey, Action callback) {
            if (callback == null) throw new ArgumentNullException(nameof(callback));

            if (!m_Channels.TryGetValue(channelKey, out var channel)) {
                channel = new Channel();
                m_Channels[channelKey] = channel;
                m_InsertSubsystem(channelKey, channel.Invoke);
            }

            channel.Add(callback);
            return new Subscription(this, channelKey, callback);
        }

        private void Unsubscribe(Type channelKey, Action callback) {
            if (!m_Channels.TryGetValue(channelKey, out var channel)) return;

            channel.Remove(callback);
            if (channel.Count > 0) return;

            m_Channels.Remove(channelKey);
            m_RemoveSubsystem(channelKey);
        }

        private sealed class Channel {
            private readonly List<Action> m_Callbacks = new();

            public int Count => m_Callbacks.Count;
            public void Add(Action callback) => m_Callbacks.Add(callback);
            public void Remove(Action callback) => m_Callbacks.Remove(callback);

            public void Invoke() {
                var snapshot = m_Callbacks.ToArray();
                foreach (var callback in snapshot) callback();
            }
        }

        private sealed class Subscription : IDisposable {
            private readonly Type m_ChannelKey;
            private PlayerLoopFanOut m_Owner;
            private Action m_Callback;

            public Subscription(PlayerLoopFanOut owner, Type channelKey, Action callback) {
                m_Owner = owner;
                m_ChannelKey = channelKey;
                m_Callback = callback;
            }

            public void Dispose() {
                if (m_Callback == null) return;

                m_Owner.Unsubscribe(m_ChannelKey, m_Callback);
                m_Callback = null;
                m_Owner = null;
            }
        }
    }
}
