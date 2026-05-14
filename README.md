# CLabs.Unity

Unity adapters for the [CLabs.Core](https://github.com/Crumpet-Labs/CLabs.Core)
package library — the ScriptableObject wrappers, MonoBehaviour controllers and
editor tooling that bind the engine-agnostic packages to Unity.

Adapters here are published by Crumpet Labs — don't edit them in this repo
directly; open an issue or discussion instead.

## Install

A CLabs.Unity adapter builds on four packages, added in dependency order:
**Buttr.Core → Buttr.Unity → the CLabs.Core package it adapts → the CLabs.Unity
adapter itself**.

### Step by step

1. Open your Unity project (Unity 6 or newer), then open **Window → Package
   Manager**.
2. Click the **+** button in the top-left, then choose **Add package from git
   URL…**.
3. Paste this URL and press Enter — **Buttr.Core**:
   `https://github.com/Crumpet-Labs/Buttr.Core.git?path=package#v1.3.3`
4. Once it finishes importing, repeat for **Buttr.Unity**:
   `https://github.com/Crumpet-Labs/Buttr.Unity.git?path=Assets/Plugins/Buttr#v2.4.0`
5. Then add the **CLabs.Core package** you want — for example, Dough:
   `https://github.com/Crumpet-Labs/CLabs.Core.git?path=packages/com.clabs.dough#v0.1.0`
6. Finally, add the matching **CLabs.Unity adapter**:
   `https://github.com/Crumpet-Labs/CLabs.Unity.git?path=adapters/unity/com.clabs.adapter.unity.dough#v0.1.0`

Add them in this order — each one depends on the packages before it. Swap
`dough` for whichever package and adapter you need.

### Or edit your manifest directly

Add the same four entries to your project's `Packages/manifest.json`:

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
