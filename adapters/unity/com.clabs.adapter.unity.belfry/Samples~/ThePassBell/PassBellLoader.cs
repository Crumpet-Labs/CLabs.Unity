using System.Threading;
using Buttr.Core;
using Buttr.Unity;
using CLabs.Belfry;
using UnityEngine;

namespace CLabs.Belfry.Samples {
    /// <summary>
    /// Brings up a Belfry container so the pass-bell MonoBehaviours can be injected with
    /// <see cref="IBellTower"/>. Create the asset via CLabs/Belfry Samples/Pass Bell Loader and add
    /// it to your app's loader set the same way the Fork/Crumb application loaders are wired.
    /// </summary>
    [CreateAssetMenu(fileName = "PassBellLoader", menuName = "CLabs/Belfry Samples/Pass Bell Loader")]
    public sealed class PassBellLoader : UnityApplicationLoaderBase {
        private ApplicationContainer m_Application;

        public override Awaitable LoadAsync(CancellationToken cancellationToken) {
            var builder = new ApplicationBuilder();
            builder.UseBelfry();
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
