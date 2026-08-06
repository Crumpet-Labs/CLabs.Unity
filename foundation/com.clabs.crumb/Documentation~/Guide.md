# CLabs Crumb: Full Guide

## Mental model

Crumb is a per-type structured logger. Every class that wants to log gets its own `CrumbLogger` instance, registered in a central `CrumbRegistry` keyed by `Type`. Each logger has:

- An `Enabled` flag (master switch).
- A `Filters` bitmask (`CrumbFilters`) controlling which log levels are active.
- A `Type` (set via `Initialize(type)`) used as the row name in console output, file output, and the editor manager window.

Log calls pass through the configured `ICrumbSink` (console output) and through `CrumbFileSink` (durable disk output). The Unity adapter swaps `ICrumbSink` for `UnityCrumbSink`, which delegates to `Debug.Log`.

## The four pieces

```
ICrumbConfiguration            : defaults: file logging, log dir, rotation, default filters
ICrumbSink                     : the console sink contract; pluggable
CrumbRegistry                  : Registry<Type, CrumbLogger>; the global "who's logging" lookup
CrumbLogger                    : per-consumer, transient, owns one Type
```

`CrumbFileSink` is registered as a concrete service and always runs alongside whatever `ICrumbSink` is bound, so file output is independent of the console sink.

## Levels and format

| Level | Tag | Method | Use case |
|---|---|---|---|
| Verbose | `VRB` | `Verbose(string)` | Trace output |
| Info | `INF` | `Info(string)` | Normal operational messages |
| Warning | `WRN` | `Warn(string)` | Potential issues |
| Error | `ERR` | `Error(string)` | Failures that need attention |
| Fatal | `FTL` | `Fatal(string, Exception)` | Unrecoverable errors (includes the exception's stack trace) |

Console format (via `ConsoleCrumbSink` or `UnityCrumbSink`):

```
[LVL] [TypeName] message
```

File format (via `CrumbFileSink`):

```
yyyy-MM-dd HH:mm:ss.fff [LVL] [TypeName] message
```

## Getting a logger

`CrumbLogger` is registered as **transient**, so every consumer gets a fresh instance. Call `Initialize(type)` to register it in the `CrumbRegistry` keyed by the owning type:

```csharp
public sealed partial class EnemyAI : MonoBehaviour {
    [Inject] private CrumbLogger i_Logger;

    private void Awake() {
        i_Logger.Initialize(GetType());
    }

    private void Update() {
        i_Logger.Verbose($"Evaluating {targets.Count} targets");
    }
}
```

Until `Initialize` is called the logger writes with `TypeName == "Uninitialized"`. Repeated `Initialize` calls re-key the registration in the registry.

`CrumbLogger` implements `IDisposable`, and disposing unregisters the logger from the registry. The Buttr container disposes transients when the container itself is disposed, so most consumers never call `Dispose` directly.

### Without Buttr

For tests and pure-C# scenarios:

```csharp
var config   = new CrumbConfiguration();
var registry = new CrumbRegistry();
var fileSink = new CrumbFileSink(config);
var console  = new ConsoleCrumbSink();

var logger = new CrumbLogger(registry, fileSink, console, config);
logger.Initialize(typeof(MySystem));
logger.Info("ready");
```

The constructor parameter order is `(registry, fileSink, consoleSink, configuration)`. Reverse-engineer from `CrumbLogger.cs` if it changes.

## Filtering

`CrumbFilters` is a `[Flags]` enum controlling active levels:

```csharp
[Flags]
public enum CrumbFilters {
    None    = 0,
    Verbose = 1,
    Info    = 2,
    Warning = 4,
    Error   = 8,
    Fatal   = 16,
    All     = Verbose | Info | Warning | Error | Fatal
}
```

### Per-logger overrides

Each logger inherits `ICrumbConfiguration.DefaultFilters` on construction but can be overridden at runtime:

```csharp
// Drop verbose on this logger only.
i_Logger.Filters = CrumbFilters.Info | CrumbFilters.Warning | CrumbFilters.Error | CrumbFilters.Fatal;

// Mute the logger entirely.
i_Logger.Enabled = false;
```

### Default filters via configuration

Set `DefaultFilters` on your `ICrumbConfiguration` implementation. New loggers pick it up at construction. Common shapes:

| Profile | Filters | Purpose |
|---|---|---|
| Development | `All` | See everything |
| QA | `Info \| Warning \| Error \| Fatal` | Skip verbose noise |
| Production | `Error \| Fatal` | Only failures |

## The platform-DI pattern

`ICrumbConfiguration` is the contract; the package ships a plain-C# `CrumbConfiguration` as the default with reasonable values (`LogDirectory = "Logs"`, 5MB rotation, 5 file retention, all levels enabled).

To swap in your own implementation, whether a Unity SO, an environment-variable-driven config or an integration-test fixture, use the `IConfigurableCollection` returned by `UseCrumbPackage()`:

```csharp
var builder = new ApplicationBuilder();
builder.UseCrumbPackage()
    .WithImplementation<ICrumbConfiguration>(() => myConfigSource)
    .WithImplementation<ICrumbSink>(() => new MyCustomSink());
using var app = builder.Build();
```

The same pattern works for `ICrumbSink`: the package binds `ConsoleCrumbSink` by default, and the Unity adapter overrides it to `UnityCrumbSink` through this mechanism.

## File sink and rotation

`CrumbFileSink` writes every log line to `ICrumbConfiguration.LogDirectory/current.log`. On every `Write` call:

1. Append the formatted line.
2. Flush.
3. If `CurrentFileSize >= MaxFileSizeBytes`, rotate: rename `current.log` to `log_{yyyyMMdd_HHmmss}.log`, open a fresh `current.log`, prune older `log_*.log` files keeping only `MaxFileCount`.

Writes are guarded by a single `lock`, so they are safe to call from any thread.

Directory defaults differ by configuration source:

| Configuration | `LogDirectory` |
|---|---|
| `CrumbConfiguration` (default) | `"Logs"` (relative to working directory) |
| Unity `CrumbConfigurationSO` | `Path.Combine(Application.persistentDataPath, "Logs")` |

## Editor tooling

`CrumbManagerWindow` (`Window > Crumpet Labs > Crumb Manager`, in the Unity adapter) lists every registered logger live in Play mode:

- Search bar to filter by type name.
- Per-logger toggle (`Enabled`).
- Per-level filter chips (`VRB`/`INF`/`WRN`/`ERR`/`FTL`). Clicking one flips the corresponding `CrumbFilters` flag on the logger.
- Bulk "All On" / "All Off" buttons.
- Status bar showing the file-logging status and `LogDirectory`.

The window discovers loggers via `Application<CrumbRegistry>.Get()` and reflects the live state. No extra wiring is required if Crumb is registered.

## Disposal

- `CrumbLogger.Dispose` releases its registration from `CrumbRegistry`. Buttr disposes transients when the container disposes.
- `CrumbFileSink.Dispose` flushes and closes the file writer. Registered as a singleton; Buttr disposes it on container disposal.
- `ConsoleCrumbSink`, `NullCrumbSink`, `CrumbConfiguration` are not `IDisposable`.

## Anti-patterns

- **Don't share one `CrumbLogger` across types.** Initialize one per consumer; the registry's per-type keying is what makes the manager window and per-class filtering work.
- **Don't write to the file sink at high frequency.** Every `Write` flushes; for hot-path logging consider a custom `ICrumbSink` that batches.
- **Don't expect log loss to be visible.** Filters silently drop; if you can't see a level in your sink, check `Enabled` and `Filters` first.
- **Don't lose the configuration override.** If you call `UseCrumbPackage()` and never `.WithImplementation<ICrumbConfiguration>(...)`, you get `CrumbConfiguration`'s defaults, including a relative `"Logs"` directory that may not be where you want logs to land.
