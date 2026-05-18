using System.IO;
using CLabs.Fork;
using UnityEngine;

namespace CLabs.Adapters {
    /// <summary>
    /// ScriptableObject implementation of <see cref="IForkConfiguration"/> that roots Fork's save
    /// directory at <c>Application.persistentDataPath</c>. Drop one of these into a
    /// <c>ForkApplicationLoader</c> to override the core package's default configuration.
    /// </summary>
    [CreateAssetMenu(fileName = "ForkConfiguration", menuName = "CLabs/Fork/Configuration")]
    public sealed class ForkConfigurationSO : ScriptableObject, IForkConfiguration {
        [Tooltip("Subdirectory under Application.persistentDataPath where save files live.")]
        [SerializeField] private string m_FolderName = "Saves";

        [Tooltip("Schema version stamped onto new save envelopes. Bump when your save format changes; pair with an ISaveMigrationStep for the upgrade.")]
        [SerializeField] private int m_CurrentSchemaVersion = 1;

        public string RootPath => Path.Combine(Application.persistentDataPath, m_FolderName);
        public int CurrentSchemaVersion => m_CurrentSchemaVersion;
    }
}
