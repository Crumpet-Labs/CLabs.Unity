using System.Threading;
using Buttr.Core;
using Buttr.Unity;
using CLabs.Belfry;
using UnityEngine;

namespace CLabs.Adapters {
    public sealed class BellLoader : UnityApplicationLoaderBase {
        [SerializeField] private PealConfigSo m_GlobalBufferConfig;

        private ApplicationContainer m_Lifetime;

        public override Awaitable LoadAsync(CancellationToken cancellationToken) {
            var builder = new ApplicationBuilder();

            if (m_GlobalBufferConfig != null) {
                builder.Resolvers.AddSingleton<IPeal, Peal>()
                    .WithFactory(() => new Peal(m_GlobalBufferConfig));
            }

            m_Lifetime = builder.Build();
            return AwaitableUtility.CompletedTask;
        }

        public override Awaitable UnloadAsync() {
            m_Lifetime?.Dispose();
            return base.UnloadAsync();
        }
    }
}
