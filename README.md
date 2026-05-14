# CLabs.Unity

Unity adapters for the [CLabs.Core](https://github.com/Crumpet-Labs/CLabs.Core)
package library — the ScriptableObject wrappers, MonoBehaviour controllers and
editor tooling that bind the engine-agnostic packages to Unity.

Adapters here are published by Crumpet Labs — don't edit them in this repo
directly; open an issue or discussion instead.

## Install

A Unity adapter needs four things: **Buttr.Core**, **Buttr.Unity**, the
**CLabs.Core** package it adapts, and the **CLabs.Unity** adapter itself. Add
them all to your Unity project's `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.crumpetlabs.buttr": "https://github.com/Crumpet-Labs/Buttr.Core.git?path=package#v1.3.3",
    "com.crumpetlabs.buttr.unity": "https://github.com/Crumpet-Labs/Buttr.Unity.git?path=Assets/Plugins/Buttr#v2.4.0",
    "com.clabs.dough": "https://github.com/Crumpet-Labs/CLabs.Core.git?path=packages/com.clabs.dough#v0.1.0",
    "com.clabs.adapter.unity.dough": "https://github.com/Crumpet-Labs/CLabs.Unity.git?path=adapters/unity/com.clabs.adapter.unity.dough#v0.1.0"
  }
}
```

So: **Buttr.Core → Buttr.Unity → CLabs.Core package → CLabs.Unity adapter** — four
entries for a manual install.

## Layout

| Path | What |
|------|------|
| `adapters/unity/` | Unity adapter packages (`com.clabs.adapter.unity.*`) |

Adapters come in three kinds — package adapters (wrap one package), bridge
adapters (wrap a bridge's SerializeField surface), and entity adapters
(MonoBehaviour controllers with entity ownership). The engine-agnostic packages
they adapt live in [CLabs.Core](https://github.com/Crumpet-Labs/CLabs.Core).

## Requirements

Unity 6+. Depends on [Buttr.Unity](https://github.com/Crumpet-Labs/Buttr.Unity)
and the matching packages from [CLabs.Core](https://github.com/Crumpet-Labs/CLabs.Core).

## License

See [LICENSE](LICENSE).
