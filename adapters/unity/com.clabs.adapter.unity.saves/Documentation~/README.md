# CLabs Adapter: Fork (Unity)

Unity bindings for `com.clabs.saves`. Roots Fork's save directory at `Application.persistentDataPath`, ships an application loader that wires the package with the Unity defaults, and provides a save-slot viewer editor window.

## What this provides

| Type | Purpose |
|---|---|
| `ForkConfigurationSO` | `ScriptableObject` implementing `IForkConfiguration`; `RootPath` resolves to `Application.persistentDataPath/{folderName}` (default: `"Saves"`) |
| `ForkApplicationLoader` | `UnityApplicationLoaderBase` SO that builds a Fork application container, optionally overriding `IForkConfiguration` from an assigned `ForkConfigurationSO` |
| `ForkViewerWindow` | `Window > Crumpet Labs > Fork Viewer`: browse active save slots, see their metadata, delete slots, open the save folder |

## Setup

### Option A: use the application loader (recommended)

1. Create a `ForkConfigurationSO` asset: `Create > CLabs > Fork > Configuration`. Set the folder name if you want something other than `"Saves"`.
2. Create a `ForkApplicationLoader` asset: `Create > CLabs > Fork > Application Loader`.
3. Assign the configuration SO to the loader's `m_Configuration` field. This is optional: leaving it empty falls back to the core package's `DefaultForkConfiguration`, which writes to a relative `"Saves"` directory.
4. Add the loader to your `UnityApplicationBoot`'s loader list.

The loader builds a container, calls `UseForkPackage()`, then overrides `IForkConfiguration` only if the SO is assigned:

```csharp
var builder = new ApplicationBuilder();
var collection = builder.UseForkPackage();
if (m_Configuration != null) {
    collection.WithImplementation<IForkConfiguration>(() => m_Configuration);
}
m_Application = builder.Build();
```

### Option B: register Fork yourself inside a larger loader

If your project has a single composite loader registering many packages, replicate the override block manually:

```csharp
public override Awaitable LoadAsync(CancellationToken cancellationToken) {
    var builder = new ApplicationBuilder();
    builder.UseForkPackage()
        .WithImplementation<IForkConfiguration>(() => m_ForkConfig);
    // ... other packages ...
    m_App = builder.Build();
    return AwaitableUtility.CompletedTask;
}
```

`UseForkPackage` registers the configuration abstract-to-concrete (`AddSingleton<Abstract, Concrete>()`); `WithImplementation<IForkConfiguration>` finds that entry by its abstract type and swaps the concrete behind it.

## Dependencies

- `com.clabs.saves`: core save-slot manager
- `com.clabs.tickets`: `Ticket` async primitive
- `com.crumpetlabs.buttr` + `com.crumpetlabs.buttr.unity`: DI and `UnityApplicationLoaderBase`
- `com.clabs.adapter.unity.editor`: viewer window base

## See also

- The `com.clabs.saves` package's `Documentation~/Guide.md`: full save-slot walkthrough.
- [../Code-Index.md](../Code-Index.md): auto-generated public surface listing.
