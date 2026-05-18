# Fork by Example

A recipe cookbook covering the canonical save-load patterns. Each recipe is independent; copy whichever you need.

## The mental model

A Fork slot is a versioned, integrity-checked save file with a backup. The flow:

```
       SaveAsync<T>(slotId, data)
            │
            ├─ serialize T → SaveEnvelope (with SchemaVersion)
            ├─ embed SHA256 checksum
            ├─ write to a new temp file
            ├─ read back, validate
            └─ if valid: promote temp → current, demote current → backup
                                          (existing save stays intact on failure)

       LoadAsync<T>(slotId)
            │
            ├─ read current file
            ├─ validate checksum
            ├─ migrate to current schema if older
            └─ deserialize → T
                  (falls back to backup file if current is corrupt)
```

Concepts in one breath:

- **Slot** — `SaveSlotInfo` { SlotId, CurrentFile, BackupFile, LastSaveTime, SchemaVersion }.
- **Envelope** — JSON wrapper with SchemaVersion + Timestamp + DataJson + Checksum.
- **`ISaveDataProvider`** — raw byte IO, swappable for cloud / console SDKs.
- **`ISaveSerializer`** — `T ↔ byte[]` + current schema version. Default: JSON.
- **`ISaveIntegrityValidator`** — checksum generate + verify. Default: SHA256.
- **`SaveMigrationPipeline`** — composed `ISaveMigrationStep`s that walk old saves forward.

---

## Recipes

### 1. Define a save type

```csharp
public sealed class GameSave
{
    public string PlayerName { get; set; }
    public int Level { get; set; }
    public float PlayTimeSeconds { get; set; }
}
```

Any class with a parameter-less constructor and JSON-serialisable properties. Keep it flat and self-describing — your future self has to migrate it.

### 2. Save to a slot

```csharp
public sealed partial class SaveService
{
    [Inject] private IForkService i_Fork;

    public async Ticket QuickSave()
    {
        var data = new GameSave { PlayerName = "Jamie", Level = 42, PlayTimeSeconds = 12345f };
        var result = await i_Fork.SaveAsync("quicksave", data);

        if (!result.Success)
        {
            // Report — see Recipe 8 for handling failures
        }
    }
}
```

`SaveAsync` is idempotent against the same slot — every call replaces the current file and demotes the previous current to backup.

### 3. Load from a slot

```csharp
public async Ticket QuickLoad()
{
    var result = await i_Fork.LoadAsync<GameSave>("quicksave");

    switch (result.Status)
    {
        case SaveLoadStatus.Success:
            ApplyGameData(result.Data);
            break;

        case SaveLoadStatus.SuccessFromBackup:
            // Primary was corrupt, backup loaded. Tell the player.
            ApplyGameData(result.Data);
            Notify("Loaded backup save — your last save was corrupt.");
            break;

        case SaveLoadStatus.SuccessMigrated:
        case SaveLoadStatus.SuccessMigratedFromBackup:
            // Save came from an older schema version — Fork migrated it.
            ApplyGameData(result.Data);
            // Re-save to bake the migration to disk.
            await i_Fork.SaveAsync("quicksave", result.Data);
            break;

        case SaveLoadStatus.NoValidSave:
            StartNewGame();
            break;

        case SaveLoadStatus.MigrationFailed:
        case SaveLoadStatus.ProviderError:
            // Show a recovery dialog.
            break;
    }
}
```

`SaveLoadResult.Success` is true for the four success-shaped statuses. Use the explicit switch when you want to differentiate (e.g. surface a UI notification on backup fallback).

### 4. List all slots

```csharp
foreach (var slot in i_Fork.GetAvailableSlots())
{
    // slot.SlotId           e.g. "quicksave"
    // slot.LastSaveTime     UTC DateTime
    // slot.SchemaVersion    int (compare against i_Fork.CurrentSchemaVersion if you expose it)
    // slot.IsAutoSave       bool (set on the slot info — your code decides)
}
```

The list reflects whatever was last persisted into `_fork_index.json`. Call `IForkService.LoadRegistryAsync()` once on app start to hydrate it from disk.

### 5. Delete a slot

```csharp
var ok = await i_Fork.DeleteSlotAsync("quicksave");
```

Removes both current and backup files plus the registry entry. Safe to call on a missing slot — returns false.

### 6. Multiple-slot UI

Fork doesn't prescribe a slot-naming scheme. Two patterns:

```csharp
// Fixed slots: "manual-1", "manual-2", "manual-3", "quicksave", "autosave"
await i_Fork.SaveAsync("manual-1", data);

// Open slots: discover via GetAvailableSlots, present an "Empty slot N" UI for missing ones.
var occupied = i_Fork.GetAvailableSlots()
    .Select(s => s.SlotId)
    .ToHashSet();
```

Whichever you pick: be consistent across save and load sites.

### 7. Bump the schema version and migrate

When your save format changes, bump `IForkConfiguration.CurrentSchemaVersion` and register a migration step.

```csharp
public sealed class MigrateV1ToV2 : ISaveMigrationStep
{
    public int FromVersion => 1;
    public int ToVersion => 2;

    public string Migrate(string rawJson)
    {
        var obj = JObject.Parse(rawJson);

        // Renamed field
        obj["health"] = obj["hp"];
        obj.Remove("hp");

        // New field with default
        obj["maxHealth"] = 100;

        return obj.ToString();
    }
}

// Register before any LoadAsync
i_Fork.RegisterMigrationStep(new MigrateV1ToV2());
```

If a v1 save is loaded against a v2 service, `LoadAsync` runs the step, returns `SuccessMigrated`. For a v0 save and `CurrentSchemaVersion = 5`, register every step `0→1`, `1→2`, …, `4→5`; Fork walks the chain.

If a step is missing from the chain, `LoadAsync` returns `MigrationFailed` and the save is untouched on disk.

### 8. Survive a corrupted save

When the primary file fails checksum validation, Fork automatically tries the backup file. No code from you required:

```csharp
var result = await i_Fork.LoadAsync<GameSave>("quicksave");
if (result.Status == SaveLoadStatus.SuccessFromBackup) {
    // Primary was corrupt, backup loaded. You probably want to:
    //   1. Tell the player they lost their last save.
    //   2. Re-save immediately to refresh the primary file.
    await i_Fork.SaveAsync("quicksave", result.Data);
}
```

If both primary AND backup fail validation, `NoValidSave` is returned. There's no third fallback — this is the "corrupted, hardware failure, ran out of disk mid-write" case.

### 9. Custom storage provider (Steam Cloud, console, server)

Implement `ISaveDataProvider`:

```csharp
public sealed class SteamCloudSaveDataProvider : ISaveDataProvider
{
    public string RootPath => "steam-cloud://";

    public Ticket<bool> WriteAsync(string relativePath, byte[] data) {
        // SteamRemoteStorage.FileWrite(relativePath, data, data.Length)
        return Ticket.FromResult(true);
    }

    public Ticket<byte[]> ReadAsync(string relativePath) {
        // SteamRemoteStorage.FileRead(...)
        return Ticket.FromResult(bytes);
    }

    public Ticket<bool> DeleteAsync(string relativePath) {
        // SteamRemoteStorage.FileDelete(relativePath)
        return Ticket.FromResult(true);
    }

    public Ticket<bool> ExistsAsync(string relativePath) {
        // SteamRemoteStorage.FileExists(relativePath)
        return Ticket.FromResult(true);
    }
}
```

Register via override:

```csharp
builder.UseForkPackage()
    .WithFactory<ISaveDataProvider>(() => new SteamCloudSaveDataProvider());
```

The provider only handles raw byte IO. Serialization, checksums, and versioning are all handled inside Fork.

### 10. Custom serializer (binary, encrypted, …)

Implement `ISaveSerializer`:

```csharp
public sealed class EncryptedJsonSerializer : ISaveSerializer
{
    private readonly IForkConfiguration m_Configuration;
    public int CurrentSchemaVersion => m_Configuration.CurrentSchemaVersion;

    public EncryptedJsonSerializer(IForkConfiguration configuration) {
        m_Configuration = configuration;
    }

    public byte[] Serialize<T>(T data) where T : class {
        var json = JsonConvert.SerializeObject(/* envelope wrapping data */);
        return MyAes.Encrypt(Encoding.UTF8.GetBytes(json));
    }

    public SaveDeserializeResult<T> Deserialize<T>(byte[] data) where T : class {
        var json = Encoding.UTF8.GetString(MyAes.Decrypt(data));
        // ... parse envelope, return SaveDeserializeResult<T>.Ok / Fail
    }
}
```

Register:

```csharp
builder.UseForkPackage()
    .WithFactory<ISaveSerializer>(() => new EncryptedJsonSerializer(
        Application<IForkConfiguration>.Get()));
```

### 11. Wire Fork under Unity

Use the Unity adapter's `ForkApplicationLoader`:

1. Create a `ForkConfigurationSO` asset (`Create > CLabs > Fork > Configuration`). Set the folder name and current schema version.
2. Create a `ForkApplicationLoader` asset (`Create > CLabs > Fork > Application Loader`).
3. Assign the configuration SO to the loader's field.
4. Add the loader to your `UnityApplicationBoot`.

The loader does the override for you. If you leave the SO unassigned, the core package's `DefaultForkConfiguration` is used (which writes to a relative `"Saves"` directory — useful for tests, but not where you want a real Unity build's saves to land).

### 12. Test Fork without Unity

For pure-C# tests, construct everything by hand or wire via Buttr:

```csharp
// Pure C# — see Tests/Clusters/ForkBasics/ForkRecipes.cs for the full pattern
var configuration = new DefaultForkConfiguration(rootPath: "/tmp/test-saves", currentSchemaVersion: 1);
var provider      = new FileSaveDataProvider(configuration);
var serializer    = new JsonSaveSerializer(configuration);
var validator     = new Sha256IntegrityValidator();
var logger        = /* a CrumbLogger */;

var service = new ForkService(provider, serializer, validator, logger);

await service.SaveAsync("test", new GameSave { /* ... */ });
```

Or with a stub provider for in-memory tests, avoiding disk:

```csharp
public sealed class StubSaveDataProvider : ISaveDataProvider
{
    public readonly Dictionary<string, byte[]> Files = new();
    public string RootPath => "stub://";

    public Ticket<bool> WriteAsync(string p, byte[] d)    { Files[p] = d; return Ticket.FromResult(true); }
    public Ticket<byte[]> ReadAsync(string p)             => Ticket.FromResult(Files.GetValueOrDefault(p));
    public Ticket<bool> DeleteAsync(string p)             { Files.Remove(p); return Ticket.FromResult(true); }
    public Ticket<bool> ExistsAsync(string p)             => Ticket.FromResult(Files.ContainsKey(p));
}
```

---

## Common mistakes

- **Don't skip `LoadRegistryAsync` on app start.** Without it, `GetAvailableSlots` returns empty even though save files exist on disk. The registry is populated from `_fork_index.json` only when you ask.
- **Don't forget to register migration steps before loading.** Fork won't synthesise missing steps; it'll return `MigrationFailed` and leave the save untouched.
- **Don't change `T` between save and load for the same slot.** The serializer round-trips a specific shape. If you must change shape, do it via schema migration, not by silently swapping types.
- **Don't write your own write-then-swap on top of Fork.** It already does this internally — adding another layer just means two temp files and two reads. Just call `SaveAsync` and trust the result.
- **Don't expect `LoadAsync<T>` to throw on failure.** Failures return `SaveLoadResult<T>` with a non-success `Status` — check it before using `result.Data`.
