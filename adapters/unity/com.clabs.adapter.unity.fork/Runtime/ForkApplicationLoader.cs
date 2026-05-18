using System.Threading;
using Buttr.Core;
using Buttr.Unity;
using CLabs.Fork;
using UnityEngine;

namespace CLabs.Adapters {
    /// <summary>
    /// UnityApplicationLoaderBase that brings up a Fork application container. Optionally accepts
    /// a <see cref="ForkConfigurationSO"/>; if none is assigned, the core package's default
    /// <see cref="DefaultForkConfiguration"/> is used (which writes to a relative <c>"Saves"</c>
    /// directory — useful for tests, but Unity consumers should supply an SO so saves land under
    /// <c>Application.persistentDataPath</c>).
    /// </summary>
    [CreateAssetMenu(fileName = "ForkApplicationLoader", menuName = "CLabs/Fork/Application Loader")]
    public sealed class ForkApplicationLoader : UnityApplicationLoaderBase {
        [Tooltip("Optional ForkConfigurationSO asset. Leave empty to use the core package's default DefaultForkConfiguration.")]
        [SerializeField] private ForkConfigurationSO m_Configuration;

        private ApplicationContainer m_Application;

        public override Awaitable LoadAsync(CancellationToken cancellationToken) {
            var builder = new ApplicationBuilder();
            var collection = builder.UseForkPackage();

            if (m_Configuration != null) {
                collection.WithFactory<IForkConfiguration>(() => m_Configuration);
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
