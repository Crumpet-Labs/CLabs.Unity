# CLabs Crumb

Per-type structured logging with level filtering, per-type filter overrides, a pluggable `ICrumbSink`, and a rotating file sink.

Engine-agnostic. The core package has no Unity dependency; Unity consumers use the `com.clabs.adapter.unity.crumb` adapter for `Debug.Log` output and a `ScriptableObject`-backed configuration.

## Quick start

### 1. Register the package

```csharp
builder.UseCrumbPackage();
```

This registers `ICrumbConfiguration` (with a sensible default), `ICrumbSink` (console-bound by default), `CrumbFileSink`, `CrumbRegistry`, and a transient `CrumbLogger`. Adapters override the defaults via the `WithFactory<T>` mechanism on the returned `IConfigurableCollection` — see the Unity adapter for a worked loader.

### 2. Inject and initialise a logger

```csharp
[Inject] private CrumbLogger i_Logger;

private void Awake() {
    i_Logger.Initialize(GetType());
}

private void Start() {
    i_Logger.Info("System initialized");
    i_Logger.Warn("Low memory");
    i_Logger.Error("Connection failed");
}
```

Each consumer gets its own `CrumbLogger` (it's registered as transient). Calling `Initialize(type)` registers the logger in the global `CrumbRegistry` keyed by that type, so editor tooling can list and filter loggers by their owning type.

## Log levels

| Level | Tag | Method | Use case |
|---|---|---|---|
| Verbose | `VRB` | `Verbose(message)` | Trace output |
| Info | `INF` | `Info(message)` | Normal operational messages |
| Warning | `WRN` | `Warn(message)` | Potential issues |
| Error | `ERR` | `Error(message)` | Failures that need attention |
| Fatal | `FTL` | `Fatal(message, exception)` | Unrecoverable errors (includes stack trace) |

Each logger has an `Enabled` flag and a `Filters` (`CrumbFilters`) bitmask inherited from `ICrumbConfiguration.DefaultFilters`. Both can be overridden at runtime.

## Sinks

`ICrumbSink` defines the single-method contract:

```csharp
void Write(string level, string typeName, string message);
```

The package ships three concrete sinks:

- **`ConsoleCrumbSink`** — `System.Console.WriteLine` output (default for pure-C# consumers).
- **`CrumbFileSink`** — rotating file sink writing to `ICrumbConfiguration.LogDirectory`, rotating at `MaxFileSizeBytes`, pruning to `MaxFileCount`.
- **`NullCrumbSink`** — discards all output (useful for tests or muted contexts).

The Unity adapter adds a fourth: `UnityCrumbSink`, mapping levels to `Debug.Log` / `Debug.LogWarning` / `Debug.LogError`.

## Dependencies

- `Buttr.Core` — DI + lifecycle.
- `CLabs.Utility` — `Registry<TKey, TValue>` for the per-type logger lookup.

Pure C#. `noEngineReferences: true`. Runs in tests without Unity.

## Further reading

- [Example.md](Example.md) — recipe cookbook for the canonical wiring + filtering + sink-override patterns.
- [Guide.md](Guide.md) — full walkthrough of the platform-DI override pattern, filter semantics, file rotation, and the `CrumbRegistry` editor surface.
