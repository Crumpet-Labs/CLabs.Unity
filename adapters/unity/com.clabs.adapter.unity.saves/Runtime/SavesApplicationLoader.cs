using System.Threading;
using Buttr.Core;
using Buttr.Unity;
using CLabs.Saves;
using UnityEngine;

namespace CLabs.Adapters {
    /// <summary>
    /// UnityApplicationLoaderBase that brings up a Fork application container. Optionally accepts
    /// a <see cref="SavesConfigurationSO"/>; if none is assigned, the core package's default
    /// <see cref="DefaultSavesConfiguration"/> is used (which writes to a relative <c>"Saves"</c>
    /// directory — useful for tests, but Unity consumers should supply an SO so saves land under
    /// <c>Application.persistentDataPath</c>).
    /// </summary>
    [CreateAssetMenu(fileName = "SavesApplicationLoader", menuName = "CLabs/Saves/Application Loader")]
    public sealed class SavesApplicationLoader : UnityApplicationLoaderBase {
        [Tooltip("Optional SavesConfigurationSO asset. Leave empty to use the core package's default DefaultSavesConfiguration.")]
        [SerializeField] private SavesConfigurationSO m_Configuration;

        private ApplicationContainer m_Application;

        public override Awaitable LoadAsync(CancellationToken cancellationToken) {
            var builder = new ApplicationBuilder();
            var collection = builder.UseSavesPackage();

            if (m_Configuration != null) {
                collection.WithImplementation<ISavesConfiguration>(() => m_Configuration);
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
