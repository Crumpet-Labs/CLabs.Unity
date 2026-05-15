# CLabs.Tickets

> **Forked from [Cysharp UniTask](https://github.com/Cysharp/UniTask) 2.5.10 (MIT) and heavily modified.** Tickets is a hard fork — the upstream code has been split into an engine-agnostic pure-C# core + engine-specific adapters, rebranded from the `Cysharp.Threading.Tasks` / `UniTask` surface to `CLabs.Tickets` / `Ticket`, reorganized into Buttr's folder conventions, reworked to dispatch through static function pointers instead of direct `PlayerLoopHelper` calls, and is no longer tracking upstream. Cysharp's original copyright is preserved in [`LICENSE.md`](../LICENSE.md) and all modifications are also MIT-licensed.

A cross-engine async/await task primitive for Unity 6+ (and eventually Godot 4), engine-decoupled into a pure-C# core with engine-specific adapters.

Tickets exists so the CLabs ecosystem can use a single async primitive across Unity and Godot without depending on an externally-owned package, and so async work integrates cleanly with Buttr's dependency injection.

## Features

- **Full UniTask 2.5.10 surface** — `Ticket`, `Ticket<T>`, `TicketCompletionSource`, `ITicketAsyncEnumerable`, all ~70 LINQ operators, `WhenAll`/`WhenAny` generated overloads, threading switches, async reactive properties, channels
- **Engine-agnostic core** — `com.clabs.tickets` has `noEngineReferences: true`, runs anywhere .NET runs
- **Unity bindings via adapter** — `com.clabs.adapter.unity.tickets` provides PlayerLoop integration, MonoBehaviour message triggers, `AsyncOperation`/`Coroutine`/`UnityWebRequest` awaiters, and optional Addressables/DOTween/TextMeshPro integrations gated by version defines
- **Zero-allocation hot path** — function-pointer dispatch from core to engine, zero-alloc engine-object detection for `WaitUntilValueChanged` / `EveryValueChanged`, upstream UniTask perf parity
- **Buttr-aware** — the Unity adapter auto-registers via `[RuntimeInitializeOnLoadMethod]`; or call `builder.UseTicketUnityPackage()` during `ApplicationBuilder` startup; `[Inject]` on MonoBehaviours
- **MIT licensed**, Cysharp's original copyright preserved in `LICENSE.md`

## Installation

Core (required):

```
https://github.com/Crumpet-Labs/CLabs.Core.git?path=packages/com.clabs.tickets
```

Unity adapter (required for Unity projects):

```
https://github.com/Crumpet-Labs/CLabs.Core.git?path=adapters/unity/com.clabs.adapter.unity.tickets
```

### Dependencies

- **Core**: none — pure .NET, no external dependencies
- **Unity adapter**: `com.clabs.tickets`, `com.clabs.utility`, `com.crumpetlabs.buttr`

## Quick Start

### 1. Register the packages

The Unity adapter registers itself automatically via `[RuntimeInitializeOnLoadMethod]`, so `await Ticket.Delay(...)` works even before `ApplicationBuilder.Build()` runs.

If you use an explicit `ApplicationBuilder` startup, call the adapter extension:

```csharp
using Buttr.Core;
using CLabs.Tickets;

var builder = new ApplicationBuilder();
builder.UseTicketUnityPackage();  // Unity adapter
var app = builder.Build();
```

### 2. Use the task primitive

```csharp
using CLabs.Tickets;

public partial class GameLoop : MonoBehaviour
{
    private async TicketVoid Start()
    {
        await Ticket.Delay(1000);
        Debug.Log("One second later");

        await Ticket.NextFrame();
        Debug.Log("Next frame");

        await Ticket.WhenAll(
            Ticket.Delay(500),
            Ticket.Delay(750),
            Ticket.Delay(1000));
        Debug.Log("Three concurrent delays complete");
    }
}
```

> **Note on naming**: the namespace is `CLabs.Tickets` (plural), the primary type is `Ticket` (singular). This mirrors .NET's `System.Threading.Tasks` / `Task` convention and avoids a namespace-type collision.

## Architecture

```
com.clabs.tickets                          ← engine-agnostic core
  ├── CLabs.Tickets                        primary Ticket struct + public API
  ├── CLabs.Tickets.Linq                   ~70 AsyncEnumerable LINQ operators
  ├── CLabs.Tickets.Internal               pools, continuation queues, helpers
  └── CLabs.Tickets.CompilerServices       AsyncTicketMethodBuilder, state machine runner

com.clabs.adapter.unity.tickets            ← Unity bindings
  ├── CLabs.Tickets.Unity                  PlayerLoopHelper, Triggers, UnityAsyncExtensions
  ├── CLabs.Tickets.Unity.Editor           Ticket tracker window
  ├── CLabs.Tickets.Addressables           Addressables integration (version-define gated)
  ├── CLabs.Tickets.DOTween                DOTween integration (version-define gated)
  └── CLabs.Tickets.TextMeshPro            TextMeshPro integration (version-define gated)
```

Core dispatches to the engine via static function pointers registered by the adapter — no interface dispatch on the hot path, identical codegen to upstream UniTask's direct `PlayerLoopHelper.X` calls.

## Roadmap

| Phase | Goal | Status |
|-------|------|--------|
| A | Lift UniTask fork into UPM layout | ✅ |
| A.5 | Buttrise — asmdef, docs, adapter bootstrapping | ✅ |
| B1 | Engine separation — abstraction, Unity adapter, file moves | ✅ |
| B2 | Cleanup — `noEngineReferences: true` flip | ✅ |
| D | Rebrand, Buttr folder conventions, class sealing, perf parity via function pointers | ✅ |
| C | Godot adapter | Deferred (gated on Buttr cross-engine split + Unity smoke test) |

## Attribution

This package is derived from [Cysharp UniTask](https://github.com/Cysharp/UniTask) 2.5.10. UniTask is © 2019 Yoshifumi Kawai / Cysharp, Inc. and is distributed under the MIT License. The full original notice is preserved in [`LICENSE.md`](../LICENSE.md). Modifications by Crumpet Labs are also released under the MIT License.

## See Also

- [Guide.md](Guide.md) — longer-form usage walkthrough
- [Buttr documentation](https://github.com/Crumpet-Labs/Buttr/blob/main/Assets/Docs/README.md) — DI framework conventions
