using System.IO;
using CLabs.Crumb;
using UnityEngine;

namespace CLabs.Adapters {
    /// <summary>ScriptableObject <see cref="ICrumbConfiguration"/>: authors the file-logging toggle, size/retention caps, and default filters. Logs under <c>Application.persistentDataPath/Logs</c>.</summary>
    [CreateAssetMenu(fileName = "CrumbConfiguration", menuName = "CLabs/Crumb/Configuration")]
    public sealed class CrumbConfigurationSO : ScriptableObject, ICrumbConfiguration {
        [Header("File Logging")]
        [SerializeField] private bool m_FileLoggingEnabled = true;
        [SerializeField] private long m_MaxFileSizeBytes = 5_242_880;
        [SerializeField] private int m_MaxFileCount = 5;

        [Header("Defaults")]
        [SerializeField] private CrumbFilters m_DefaultFilters = CrumbFilters.All;

        public bool FileLoggingEnabled => m_FileLoggingEnabled;
        public string LogDirectory => Path.Combine(Application.persistentDataPath, "Logs");
        public long MaxFileSizeBytes => m_MaxFileSizeBytes;
        public int MaxFileCount => m_MaxFileCount;
        public CrumbFilters DefaultFilters => m_DefaultFilters;
    }
}
