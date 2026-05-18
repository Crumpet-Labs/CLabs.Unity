## CLabs Fork

Save-slot manager. Versioned save slots with write-then-swap file safety, SHA256 integrity validation, schema migration pipelines, and pluggable storage providers.

Engine-agnostic. The core package has no Unity dependency; Unity consumers use the `com.clabs.adapter.unity.fork` adapter for `Application.persistentDataPath`-backed configuration and an editor viewer window.

## Features

- **Save slots** — multiple save files with current + backup per slot.
- **Write-then-swap** — never overwrites existing saves; validates the new file before promoting.
- **Integrity validation** — SHA256 checksums detect corruption before deserialization.
- **Schema migration** — versioned saves with a step-by-step migration pipeline.
- **Platform abstraction** — swap `ISaveDataProvider` for Steam Cloud, consoles, etc.
- **Slot registry** — persistent index of all saves; no filesystem scanning.
- **Backup fallback** — automatically tries the backup file if the primary is corrupt.
- **Buttr DI integration** — register with `builder.UseForkPackage()` and inject `IForkService`.

## Quick start

### 1. Register the package

```csharp
builder.UseForkPackage();
```

`UseForkPackage` registers a default `IForkConfiguration` (writes to a relative `"Saves"` directory), the registry, the JSON serializer, the SHA256 validator, the file provider, and `IForkService`. To override the configuration — for a Unity build, a Steam Cloud provider, or anything else — chain `.WithFactory<T>(...)` on the returned `IConfigurableCollection`:

```csharp
builder.UseForkPackage()
    .WithFactory<IForkConfiguration>(() => myConfigSource)
    .WithFactory<ISaveDataProvider>(() => new MyCloudProvider());
```

### 2. Save

```csharp
public sealed partial class SaveManager {
    [Inject] private IForkService i_Fork;

    public async Ticket Save() {
        var result = await i_Fork.SaveAsync("slot1", new GameSaveData { /* ... */ });
        if (result.Success) {
            // Saved successfully. result.FilePath has the final file location.
        }
    }
}
```

### 3. Load

```csharp
var result = await i_Fork.LoadAsync<GameSaveData>("slot1");
switch (result.Status) {
    case SaveLoadStatus.Success:
    case SaveLoadStatus.SuccessFromBackup:
    case SaveLoadStatus.SuccessMigrated:
        ApplyGameData(result.Data);
        break;
    case SaveLoadStatus.NoValidSave:
        StartNewGame();
        break;
}
```

### 4. List + delete

```csharp
foreach (var slot in i_Fork.GetAvailableSlots()) {
    // slot.SlotId, slot.LastSaveTime, slot.SchemaVersion
}

await i_Fork.DeleteSlotAsync("slot1");
```

## Migration

```csharp
public sealed class MigrateV1ToV2 : ISaveMigrationStep {
    public int FromVersion => 1;
    public int ToVersion => 2;
    public string Migrate(string rawJson) { /* transform JSON */ }
}

i_Fork.RegisterMigrationStep(new MigrateV1ToV2());
```

Register migration steps before calling `LoadAsync`.

## Custom providers

Implement `ISaveDataProvider` for platform-specific storage (Steam Cloud, console APIs, your own backend). The provider handles raw byte IO only — serialization, checksums, and versioning are handled by Fork. Override with `.WithFactory<ISaveDataProvider>(() => new MyProvider())`.

## Dependencies

- `Buttr.Core` — DI + lifecycle.
- `com.clabs.tickets` — `Ticket` async primitive.
- `com.clabs.crumb` — operator-visible logging on serialization / validation failures.
- `com.unity.nuget.newtonsoft-json` — JSON serialization.

Pure C#. `noEngineReferences: true`. Runs in tests without Unity.

## Further reading

- [Example.md](Example.md) — recipe cookbook covering save / load / migration / custom-provider patterns end-to-end.
- [Guide.md](Guide.md) — full walkthrough of slots, write-then-swap, integrity, the migration pipeline, custom providers, and the platform-DI override pattern.
