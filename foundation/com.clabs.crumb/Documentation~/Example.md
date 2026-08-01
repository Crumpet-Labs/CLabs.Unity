# Crumb by Example

A recipe cookbook for getting Crumb wired in correctly the first time. Each recipe is independent; copy whichever you need.

## The mental model

A `CrumbLogger` is per-type. Every class that wants to log gets its own injected instance, calls `Initialize(GetType())` once, and then logs at five severity levels. Behind the scenes:

- The logger writes to **two sinks** every call: an `ICrumbSink` (console output, swappable) and a `CrumbFileSink` (durable file output, always-on, rotating).
- A central **`CrumbRegistry`** holds every initialized logger keyed by `Type`. Editor tooling lists them; you can flip filters live in Play mode.
- A **`CrumbFilters`** bitmask controls which levels actually emit. Each logger inherits the default from configuration but can be overridden per-instance.

```
   [Inject] CrumbLogger      ↘
                              ┌─── ICrumbSink     (Console / Debug.Log / your custom)
   Initialize(GetType())  ──→ ┤
                              └─── CrumbFileSink  (rotating file)
                              ↗
        Registry tracks loggers by Type
```

---

## Recipes

### 1. Define a logger consumer

```csharp
using Buttr.Injection;
using CLabs.Crumb;
using UnityEngine;

public sealed partial class PlayerController : MonoBehaviour
{
    [Inject] private CrumbLogger i_Logger;

    private void Awake()
    {
        i_Logger.Initialize(GetType());
    }
}
```

Three rules:

- `sealed partial class` — Buttr's source generator needs `partial` to populate `[Inject]` fields.
- `[Inject] private CrumbLogger i_Logger;` — `i_` prefix marks injected fields.
- `Initialize(GetType())` in `Awake` — registers the logger in `CrumbRegistry` keyed by your type so editor tooling can find it.

### 2. Log at all five levels

```csharp
i_Logger.Verbose("Frame-by-frame trace info");
i_Logger.Info("System initialised");
i_Logger.Warn("Low ammo");
i_Logger.Error("Save failed");
i_Logger.Fatal("Unrecoverable", new InvalidOperationException("save dir gone"));
```

Output format on console:

```
[VRB] [PlayerController] Frame-by-frame trace info
[INF] [PlayerController] System initialised
[WRN] [PlayerController] Low ammo
[ERR] [PlayerController] Save failed
[FTL] [PlayerController] Unrecoverable
System.InvalidOperationException: save dir gone   ← stack trace appended
   at ...
```

File output adds an ISO-8601 timestamp prefix to every line.

### 3. Filter levels per logger at runtime

Each logger has a `Filters` bitmask (`CrumbFilters`) controlling which levels emit. Override at any time:

```csharp
// Mute Verbose on this logger only — keep everything else.
i_Logger.Filters = CrumbFilters.Info | CrumbFilters.Warning | CrumbFilters.Error | CrumbFilters.Fatal;

// Only show failures.
i_Logger.Filters = CrumbFilters.Error | CrumbFilters.Fatal;
```

The default value comes from `ICrumbConfiguration.DefaultFilters` at construction. Per-logger overrides take effect immediately.

### 4. Mute a logger entirely

```csharp
i_Logger.Enabled = false;
i_Logger.Error("nobody hears this"); // dropped before reaching any sink
i_Logger.Enabled = true;             // back on
```

`Enabled = false` short-circuits BEFORE the filter check. Useful for noisy classes you want to silence without losing the registration.

### 5. Replace the console sink

The default `ICrumbSink` is `ConsoleCrumbSink` (writes to `System.Console`). Override via the `IConfigurableCollection` returned from `UseCrumbPackage`:

```csharp
// Unity adapter wires this for you via UnityCrumbSink — but you can do it yourself:
builder.UseCrumbPackage()
    .WithImplementation<ICrumbSink>(() => new UnityCrumbSink());
```

For tests that should never see Crumb output, swap in `NullCrumbSink`:

```csharp
builder.UseCrumbPackage()
    .WithImplementation<ICrumbSink>(() => new NullCrumbSink());
```

Custom sinks are equally easy — implement `ICrumbSink.Write(level, typeName, message)` and route to whatever you like (in-memory buffer, network endpoint, structured-logging backend).

### 6. Replace the configuration

`ICrumbConfiguration` controls the file directory, rotation thresholds, and the default filter mask new loggers inherit. The package ships a plain-C# `CrumbConfiguration` with reasonable defaults; override to point logs at the right place for your platform:

```csharp
builder.UseCrumbPackage()
    .WithImplementation<ICrumbConfiguration>(() => new CrumbConfiguration(
        logDirectory: "/var/log/myapp",
        fileLoggingEnabled: true,
        maxFileSizeBytes: 10 * 1024 * 1024,
        maxFileCount: 10,
        defaultFilters: CrumbFilters.Info | CrumbFilters.Warning | CrumbFilters.Error | CrumbFilters.Fatal));
```

In Unity, the adapter ships `CrumbConfigurationSO` (a `ScriptableObject` implementation) and `CrumbApplicationLoader` that wires it. See the `com.clabs.adapter.unity.crumb` README for the asset-driven workflow.

### 7. Standalone usage without Buttr

For tests or pure-C# scenarios you can wire Crumb by hand:

```csharp
var config   = new CrumbConfiguration();
var registry = new CrumbRegistry();
var fileSink = new CrumbFileSink(config);
var console  = new ConsoleCrumbSink();

var logger = new CrumbLogger(registry, fileSink, console, config);
logger.Initialize(typeof(MySystem));

logger.Info("ready");
```

Constructor order is `(registry, fileSink, consoleSink, configuration)`. Look at `CrumbLogger.cs` if that ever changes.

### 8. Browse loggers in the editor

In Unity, `Window > Crumpet Labs > Crumb Manager` lists every registered logger in Play mode:

- Per-logger toggle to flip `Enabled`.
- Per-level chips (`VRB`/`INF`/`WRN`/`ERR`/`FTL`) — click to flip the bit in `Filters`.
- Bulk "All On" / "All Off".
- Search by type name.
- Status bar shows file-logging state and the live `LogDirectory`.

Nothing extra to wire — the manager discovers loggers via the registry that `Initialize` populates.

---

## Common mistakes

- **Don't share one logger across types.** Each consumer gets its own injected `CrumbLogger`, and `Initialize(GetType())` keys it correctly. The manager window groups by type — sharing one logger means everything reports under whichever type registered first.
- **Don't forget to call `Initialize`.** Without it the type name is `"Uninitialized"` in every log line, and the registry never learns about your logger.
- **Don't fight the file sink.** It always runs in parallel with the console sink. If you want zero disk output, set `ICrumbConfiguration.FileLoggingEnabled = false`.
- **Don't expect filter changes to be retroactive.** Filters short-circuit on the way IN; once a line is in your console / file, it's there.
