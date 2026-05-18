using System.Collections.Generic;

namespace CLabs.Fork {
    public sealed class SaveMigrationPipeline {
        private readonly SortedDictionary<int, ISaveMigrationStep> m_Steps = new();

        public void RegisterStep(ISaveMigrationStep step) {
            m_Steps[step.FromVersion] = step;
        }

        public MigrationResult Migrate(string rawJson, int fromVersion, int toVersion) {
            if (fromVersion >= toVersion) {
                return MigrationResult.Ok(rawJson, fromVersion);
            }

            var currentJson = rawJson;
            var currentVersion = fromVersion;

            while (currentVersion < toVersion) {
                if (!m_Steps.TryGetValue(currentVersion, out var step)) {
                    return MigrationResult.Fail(
                        $"Missing migration step from v{currentVersion} to v{currentVersion + 1}"
                    );
                }

                if (step.ToVersion != currentVersion + 1) {
                    return MigrationResult.Fail(
                        $"Migration step mismatch: expected v{currentVersion}→v{currentVersion + 1}, " +
                        $"got v{step.FromVersion}→v{step.ToVersion}"
                    );
                }

                currentJson = step.Migrate(currentJson);

                if (currentJson == null) {
                    return MigrationResult.Fail(
                        $"Migration step v{currentVersion}→v{step.ToVersion} returned null"
                    );
                }

                currentVersion = step.ToVersion;
            }

            return MigrationResult.Ok(currentJson, currentVersion);
        }
    }
}
