# CLabs Fork — Full Guide

## Overview

Fork manages save slots — versioned, integrity-checked save files with a backup fallback. It sits above your game's in-memory state (Knife, Spoon, anything else) and handles the mechanics of persisting that state to disk safely.

## Architecture

```
Game Code
    │
    ├── SaveAsync<T>(slotId, data)
    │   ├── ISaveSerializer → SaveEnvelope (versioned JSON)
    │   ├── ISaveIntegrityValidator → SHA256 checksum
    │   ├── ISaveDataProvider → write to filesystem
    │   └── Write-then-swap → promote to current, demote to backup
    │
    └── LoadAsync<T>(slotId)
        ├── ISaveDataProvider → read from filesystem
        ├── ISaveIntegrityValidator → validate checksum
        ├── SaveMigrationPipeline → migrate old versions
        └── ISaveSerializer → deserialize to T
```

## Save slots

Each slot has:

- **Current file** — the latest valid save.
- **Backup file** — the previous save, kept as a fallback.
- **Metadata** — last save time, schema version, auto-save flag.

Slots are tracked in a lightweight `_fork_index.json` registry file in the configured `RootPath`. The registry is loaded on `IForkService.LoadRegistryAsync()` and rewritten on every save / delete.

## Write-then-swap strategy

Fork never overwrites an existing save directly. The save process:

1. Serialize data with the current schema version.
2. Embed an SHA256 checksum in the save envelope.
3. Write to a **new temporary file** named `{slotId}_{timestamp}.sav`.
4. **Read back** the written file and validate its integrity.
5. If valid: the existing current file becomes backup, the new file becomes current.
6. If invalid: delete the temp file. The existing save is untouched.

This guarantees you always have at least one valid save file per slot, even when the disk lies or the process is killed mid-write.

## Integrity validation

Every save file contains a SHA256 checksum over the serialized game data. On load:

1. Read raw bytes from disk.
2. Deserialize the envelope to extract the embedded checksum.
3. Compute checksum over the `DataJson` field.
4. Compare. If they mismatch, the file is corrupt → fall back to the backup file.

Failure reasons surfaced through `IntegrityResult.Reason`: `ChecksumMismatch`, `MissingChecksum`, `IncompleteFile`, `EmptyFile`, `UnreadableFormat`.

## Schema migration

Games evolve. Save format v1 won't match v5. Fork handles this with a step-based migration pipeline.

### How it works

1. Each `ISaveMigrationStep` converts from version `N` to `N+1`.
2. Steps work on **raw JSON strings** — no need for old type definitions in your codebase.
3. If a save is v2 and the current schema is v5, Fork runs: `v2→v3 → v3→v4 → v4→v5`.
4. If any step in the chain is missing, migration fails gracefully and the load reports `MigrationFailed`.

### Registering steps

```csharp
i_Fork.RegisterMigrationStep(new MigrateV1ToV2());
i_Fork.RegisterMigrationStep(new MigrateV2ToV3());
```

Register all steps **before** calling `LoadAsync`.

### Writing a migration step

```csharp
public sealed class MigrateV1ToV2 : ISaveMigrationStep {
    public int FromVersion => 1;
    public int ToVersion => 2;

    public string Migrate(string rawJson) {
        var obj = JObject.Parse(rawJson);

        // Rename field
        obj["health"] = obj["hp"];
        obj.Remove("hp");

        // Add new field with default
        obj["maxHealth"] = 100;

        return obj.ToString();
    }
}
```

## Save file format

Each file on disk is a JSON `SaveEnvelope`:

```json
{
    "SchemaVersion": 3,
    "Timestamp": "2026-04-12T14:30:00Z",
    "TotalPlayTimeSeconds": 12345.6,
    "DataJson": "{\"playerName\":\"Jamie\",\"level\":42}",
    "Checksum": "a1b2c3d4..."
}
```

`DataJson` is the serialized payload as a string so the checksum can be computed over a stable byte representation.

## The platform-DI override pattern

`UseForkPackage` registers `IForkConfiguration` (default: `DefaultForkConfiguration` with `RootPath = "Saves"` relative) and `ISaveDataProvider` (default: `FileSaveDataProvider` rooted at the configuration's `RootPath`). Both can be overridden via the `IConfigurableCollection` returned from `UseForkPackage`:

```csharp
var builder = new ApplicationBuilder();
builder.UseForkPackage()
    .WithFactory<IForkConfiguration>(() => mySourceOfTruth)
    .WithFactory<ISaveDataProvider>(() => new MyCloudProvider());
using var app = builder.Build();
```

The same shape applies to `ISaveSerializer` and `ISaveIntegrityValidator` if you ever need to swap them — both are registered through `IConfigurableCollection` keyed by the interface.

## Custom providers

| Platform | Provider |
|---|---|
| Local filesystem | `FileSaveDataProvider` (built-in) |
| Steam Cloud | Custom — use Steamworks API |
| Console (PS5, Xbox) | Custom — use platform SDK |
| Cloud sync | Custom — your backend API |

The provider only handles raw byte IO. Serialization, checksums, and versioning are handled inside Fork; your provider just reads and writes bytes.

## Editor tools

The Unity adapter ships a `Fork Viewer` editor window (`Window > Crumpet Labs > Fork Viewer`) listing every save slot, showing last-save time, schema version, current/backup file paths, and offering "Delete Slot" + "Open Folder" actions. Open Folder uses the live `IForkConfiguration.RootPath`, so it reflects whatever override is in place.

## Disposal

- `IForkService` doesn't need explicit disposal — Buttr disposes registered singletons on container disposal.
- The save / load operations are atomic per call; nothing is left half-written even on abrupt shutdown (the write-then-swap step is the guarantee).
