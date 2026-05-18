# CLabs.Unity

A Unity UPM distribution of the **CLabs** game-development standard library — a curated, opinionated set of packages and bridges for building Unity 6+ games in C#, designed to compose cleanly with the [Buttr](https://github.com/Crumpet-Labs/Buttr.Core) dependency-injection framework.

This repo is a **single Unity UPM umbrella package** that ships every public CLabs package, bridge, and Unity adapter as one installable unit. Drop it in your project from the Package Manager and you have a complete CLabs runtime.

If you're building a non-Unity .NET project, use [CLabs.Core](https://github.com/Crumpet-Labs/CLabs.Core) instead.

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

Pin versions by appending a tag (e.g. `#v1.3.3` for Buttr.Core, `#v1.3.3` for Buttr.Unity, `#v1.1.0` for CLabs.Unity). See the [Releases](https://github.com/Crumpet-Labs/CLabs.Unity/releases) tab. Requires Unity 6.0+.

## Single-package install

Prefer to install one CLabs package without pulling the whole umbrella? Add the package's UPM URL directly via `Window > Package Manager` → **+** → **Install package from git URL**. Each package's Unity adapter is a separate UPM URL — install both for full functionality. Buttr.Core and Buttr.Unity from the umbrella install above are still required prerequisites.

## Packages

- **Belfry**

  ```
  https://github.com/Crumpet-Labs/CLabs.Unity.git?path=core/com.clabs.belfry
  ```

  Unity adapter:

  ```
  https://github.com/Crumpet-Labs/CLabs.Unity.git?path=adapters/unity/com.clabs.adapter.unity.belfry
  ```

- **Crumb**

  ```
  https://github.com/Crumpet-Labs/CLabs.Unity.git?path=core/com.clabs.crumb
  ```

  Unity adapter:

  ```
  https://github.com/Crumpet-Labs/CLabs.Unity.git?path=adapters/unity/com.clabs.adapter.unity.crumb
  ```

- **Fork**

  ```
  https://github.com/Crumpet-Labs/CLabs.Unity.git?path=core/com.clabs.fork
  ```

  Unity adapter:

  ```
  https://github.com/Crumpet-Labs/CLabs.Unity.git?path=adapters/unity/com.clabs.adapter.unity.fork
  ```

- **Spoon**

  ```
  https://github.com/Crumpet-Labs/CLabs.Unity.git?path=core/com.clabs.spoon
  ```

  Unity adapter:

  ```
  https://github.com/Crumpet-Labs/CLabs.Unity.git?path=adapters/unity/com.clabs.adapter.unity.spoon
  ```

- **Tickets**

  ```
  https://github.com/Crumpet-Labs/CLabs.Unity.git?path=core/com.clabs.tickets
  ```

  Unity adapter:

  ```
  https://github.com/Crumpet-Labs/CLabs.Unity.git?path=adapters/unity/com.clabs.adapter.unity.tickets
  ```

- **Utility**

  ```
  https://github.com/Crumpet-Labs/CLabs.Unity.git?path=core/com.clabs.utility
  ```

  Unity adapter:

  ```
  https://github.com/Crumpet-Labs/CLabs.Unity.git?path=adapters/unity/com.clabs.adapter.unity.utility
  ```

## Bridges

- **Spoon ↔ Belfry**

  ```
  https://github.com/Crumpet-Labs/CLabs.Unity.git?path=bridges/com.clabs.bridge.spoon-belfry
  ```

## Versioning

CLabs.Unity ships **unified versions** — a single semver per release covering every package in the umbrella. Releases are tagged `v<X.Y.Z>` and get GitHub Releases with grouped notes (Breaking changes / Features / Fixes) — see the [Releases](https://github.com/Crumpet-Labs/CLabs.Unity/releases) tab.

Per-package versions inside the umbrella bump independently (PATCH on fix, MINOR on feat, MAJOR on breaking change); the umbrella version is `max()` across them.

## License

MIT, unless a package's own `LICENSE.md` says otherwise (e.g. CLabs.Tickets preserves Cysharp's original UniTask copyright). See each package directory inside the umbrella for specifics.

## Project status

CLabs is in active development. The public surface is intentionally small while we get the publishing pipeline solid; expect more packages to be promoted as they're stabilised.
