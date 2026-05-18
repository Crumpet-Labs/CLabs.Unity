using System.Threading;
using Buttr.Core;
using Buttr.Unity;
using CLabs.Crumb;
using UnityEngine;

namespace CLabs.Adapters {
    /// <summary>
    /// UnityApplicationLoaderBase that brings up a Crumb application container with the Unity-specific
    /// console sink wired in. Optionally accepts a <see cref="CrumbConfigurationSO"/>; if none is assigned,
    /// the core package's default <see cref="CrumbConfiguration"/> is used unchanged.
    /// </summary>
    [CreateAssetMenu(fileName = "CrumbApplicationLoader", menuName = "CLabs/Crumb/Application Loader")]
    public sealed class CrumbApplicationLoader : UnityApplicationLoaderBase {
        [Tooltip("Optional CrumbConfigurationSO asset. Leave empty to use the core package's default CrumbConfiguration.")]
        [SerializeField] private CrumbConfigurationSO m_Configuration;

        private ApplicationContainer m_Application;

        public override Awaitable LoadAsync(CancellationToken cancellationToken) {
            var builder = new ApplicationBuilder();
            var collection = builder.UseCrumbPackage()
                .WithFactory<ICrumbSink>(() => new UnityCrumbSink());

            if (m_Configuration != null) {
                collection.WithFactory<ICrumbConfiguration>(() => m_Configuration);
            }

            m_Application = builder.Build();
            return AwaitableUtility.CompletedTask;
        }

        public override Awaitable UnloadAsync() {
            m_Application?.Dispose();
            m_Application = null;
            return AwaitableUtility.CompletedTask;
        }
    }
}
