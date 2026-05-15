# CLabs.Unity

A Unity UPM distribution of the **CLabs** game-development standard library — a curated, opinionated set of packages and bridges for building Unity 6+ games in C#, designed to compose cleanly with the [Buttr](https://github.com/Crumpet-Labs/Buttr.Core) dependency-injection framework.

This repo is a **single Unity UPM umbrella package** that ships every public CLabs package, bridge, and Unity adapter as one installable unit. Drop it in your `Packages/manifest.json` and you have a complete CLabs runtime for your Unity project — with Unity adapters wired in for `OwnerId`, `Color`, Unity PlayerLoop integration for `Ticket`, and the rest.

If you're building a non-Unity .NET project, use [CLabs.Core](https://github.com/Crumpet-Labs/CLabs.Core) instead — CLabs.Unity is Unity-specific.

## What's inside

Inside the umbrella you'll find the full live CLabs surface laid out for Unity:

```
CLabs.Unity
+- core/         domain packages (Belfry, Tickets, Utility, ...)
+- bridges/      cross-package wiring
+- adapters/     Unity engine adapters (PlayerLoop, OwnerId<->Unity types, Color<->Unity Color, ...)
+- package.json  the UPM umbrella manifest
```

Highlights of what you get:

- **Belfry** — type-safe key-scoped pub/sub messaging (Tower / Rope / Ring).
- **Tickets** — cross-engine async/await primitive, full UniTask surface with Unity PlayerLoop integration, MonoBehaviour message triggers, and `AsyncOperation` / `Coroutine` / `UnityWebRequest` awaiters.
- **Utility** — foundation utilities: `OwnerId`, `Color`, `Registry<,>`, `Disposable`, plus the Unity adapter mapping these to engine-native equivalents (`Color` ⇄ `UnityEngine.Color`, etc.).

Each package's `Documentation~/README.md` (inside the umbrella) covers package-specific usage in detail.

## What CLabs is for

Building games — across engines, across teams, with stable conventions. The library was extracted from years of shipping commercial Unity games; it's deliberately concrete (no abstract-framework navel-gazing), Buttr-first (DI everywhere, no static singletons), and engine-pluggable (cores are pure C#, Unity specifics live in adapters).

## Installation

CLabs.Unity depends on Buttr.Core and Buttr.Unity. UPM doesn't auto-resolve git-URL dependencies, so install them in order. In `Window > Package Manager` → **+** → **Install package from git URL**:

1. Install Buttr.Core:

   ```
   https://github.com/Crumpet-Labs/Buttr.Core.git?path=package
   ```

2. Install Buttr.Unity:

   ```
   https://github.com/Crumpet-Labs/Buttr.Unity.git?path=Assets/Plugins/Buttr
   ```

3. Install CLabs.Unity:

   ```
   https://github.com/Crumpet-Labs/CLabs.Unity.git
   ```

Pin versions by appending a tag (e.g. `#v1.3.3` for Buttr.Core, `#v1.3.3` for Buttr.Unity, `#v1.1.0` for CLabs.Unity). See the [Releases](https://github.com/Crumpet-Labs/CLabs.Unity/releases) tab for what's available. Requires Unity 6.0+.

## Using it

Each package registers its services on Buttr's `ApplicationBuilder`. A minimal bootstrap:

```csharp
using Buttr.Core;
using CLabs.Belfry;

public sealed class Bootstrap : MonoBehaviour {
    private void Awake() {
        var builder = new ApplicationBuilder();

        builder.UseBelfry();    // registers IBellTower, IBelfry, IPealFactory
        // builder.Use<...>();  // any other CLabs packages you depend on

        var app = builder.Build();
    }
}
```

Once built, anywhere in your code:

```csharp
using Buttr.Injection;
using CLabs.Belfry;
using UnityEngine;

public sealed class CombatSystem : MonoBehaviour {
    [Inject] private IBellTower i_Tower;

    public void DefeatEnemy(int entityId, int xp) {
        i_Tower
            .Rope("combat")
            .Ring(new EnemyDefeated(entityId, xp));
    }
}
```

See each package's `Documentation~/README.md` (inside the imported package, accessible from Unity's Package Manager window) for the full API and usage walkthroughs.

## Versioning

CLabs.Unity ships **unified versions** — a single semver per release covering every package in the umbrella. Releases are tagged `v<X.Y.Z>` and get GitHub Releases with grouped notes (Breaking changes / Features / Fixes) — see the [Releases](https://github.com/Crumpet-Labs/CLabs.Unity/releases) tab.

Per-package versions inside the umbrella bump independently (PATCH on fix, MINOR on feat, MAJOR on breaking change); the umbrella version is `max()` across them.

## License

MIT, unless a package's own `LICENSE.md` says otherwise (e.g. CLabs.Tickets preserves Cysharp's original UniTask copyright). See each package directory inside the umbrella for specifics.

## Project status

CLabs is in active development. The public surface is intentionally small while we get the publishing pipeline solid; expect more packages to be promoted as they're stabilised.
