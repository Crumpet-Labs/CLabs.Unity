# CLabs.Tickets: Usage Guide

> **Forked from [Cysharp UniTask](https://github.com/Cysharp/UniTask) 2.5.10 (MIT) and heavily modified.** The core has been split into a pure-C# assembly + engine-specific adapters, rebranded from `Cysharp.Threading.Tasks` / `UniTask` to `CLabs.Tickets` / `Ticket`, reorganized into Buttr's folder conventions, reworked to dispatch through static function pointers instead of interface calls, and is no longer tracking upstream. Cysharp's copyright is preserved in [`LICENSE.md`](../LICENSE.md).

This guide covers how to use Tickets in a CLabs project. The mental model is identical to UniTask's: the type name changed and the namespace pluralised, but every operator still exists.

## Why Tickets exists

CLabs uses async/await across the ecosystem, and in any feature that does timing, polling or animation orchestration. Before Tickets, those packages depended on `com.cysharp.unitask` directly via `externalDependencies`, which:

1. Coupled CLabs to an externally-owned Unity-only package
2. Blocked Godot support (UniTask is Unity-only)
3. Didn't fit the Buttr DI ecosystem (no adapter bootstrapping entry point)

Tickets solves all three by hard-forking UniTask, splitting the engine integration into a separate adapter package, and exposing a Buttr-conformant entry point. The core has `noEngineReferences: true`, so it runs in a pure .NET context with no Unity SDK present.

## Architecture

```
com.clabs.tickets                          ← engine-agnostic core
├── package.json                           com.clabs.tickets, Buttr 1.3.3 dep
├── LICENSE.md                             Cysharp MIT + CLabs MIT
└── Runtime/
    ├── CLabs.Tickets.asmdef               allowUnsafeCode: true, noEngineReferences: true
    ├── Contracts/                         ITicketSource, ITicketAsyncEnumerable, IPlayerLoopItem
    ├── Common/                            Ticket, TicketVoid, TicketCompletionSource, TicketRuntime,
    │                                      PlayerLoopTiming, AsyncUnit, Progress, etc.
    ├── Components/                        TicketScheduler, TicketSynchronizationContext, TaskPool,
    │                                      AsyncLazy, AsyncReactiveProperty, Channel
    ├── Extensions/                        TicketExtensions, CancellationTokenExtensions, etc.
    ├── Internal/                          pools, ContinuationQueue, diagnostics, TaskTracker
    ├── CompilerServices/                  AsyncTicketMethodBuilder, state machine runners
    └── Linq/                              ~70 AsyncEnumerable LINQ operators

com.clabs.adapter.unity.tickets            ← Unity bindings
└── Runtime/
    ├── CLabs.Tickets.Unity.asmdef         allowUnsafeCode: true
    ├── TicketUnityPackage.cs              UseTicketUnityPackage() + [RuntimeInitializeOnLoadMethod]
    ├── Components/                        PlayerLoopHelper, PlayerLoopTimer
    ├── Extensions/                        EnumeratorAsyncExtensions, UnityAsyncExtensions + partials,
    │                                      TicketUnityActions, UnityBindingExtensions, UnityAwaitableExtensions
    ├── Exceptions/                        UnityWebRequestException
    ├── MonoBehaviours/Triggers/           ~80 partial AsyncTriggerHandler fragments
    └── External/
        ├── Addressables/                  com.unity.addressables integration (version-define gated)
        ├── DOTween/                       DOTween integration (version-define gated)
        └── TextMeshPro/                   TextMeshPro integration (version-define gated)
```

Core dispatches to the engine via static function pointers registered by the adapter at `[RuntimeInitializeOnLoadMethod(BeforeSceneLoad)]`. Every `Ticket.Delay`, `Ticket.WaitUntil` and `await` scheduling call therefore goes through a direct `call` instruction rather than an interface vtable lookup. Matches upstream UniTask's hot-path codegen exactly.

## Installation

Core (required):

```
https://github.com/Crumpet-Labs/CLabs.Core.git?path=packages/com.clabs.tickets
```

Unity adapter (required for Unity projects):

```
https://github.com/Crumpet-Labs/CLabs.Core.git?path=adapters/unity/com.clabs.adapter.unity.tickets
```

The Unity adapter depends on the core, so installing the adapter pulls in the core automatically.

## Buttr Integration

```csharp
using Buttr.Core;
using CLabs.Tickets;

public static class Program
{
    public static ApplicationContainer Main() => Main(CMDArgs.Read());

    private static ApplicationContainer Main(IDictionary<string, string> args)
    {
        var builder = new ApplicationBuilder();

        builder.UseTicketUnityPackage();     // Unity adapter (bootstraps core too)

        // ... other CLabs packages ...

        return builder.Build();
    }
}
```

The Unity adapter also self-registers via `[RuntimeInitializeOnLoadMethod]` so `await Ticket.Delay(...)` works in code that runs before `ApplicationBuilder.Build()` (static constructors, early MonoBehaviour lifecycle, etc.). The explicit `UseTicketUnityPackage()` call is optional: it registers with Buttr's DI graph, but the PlayerLoop bindings are established regardless.

## Common patterns

### Frame-aware delays

```csharp
using CLabs.Tickets;

await Ticket.Delay(TimeSpan.FromSeconds(1));                // scaled delta time, respects Time.timeScale
await Ticket.Delay(1000, DelayType.UnscaledDeltaTime);       // ignores Time.timeScale
await Ticket.DelayFrame(60);                                 // 60 frames
await Ticket.NextFrame();                                    // one frame
await TicketUnityDelay.WaitForEndOfFrame();                  // post-render (Unity adapter only)
```

### Waiting on conditions

```csharp
await Ticket.WaitUntil(() => isReady);
await Ticket.WaitWhile(() => isLoading);
await Ticket.WaitUntilValueChanged(player, p => p.Health);   // zero-alloc on Unity targets
```

`WaitUntilValueChanged` and `EveryValueChanged` detect whether the target is a Unity object (`UnityEngine.Object`) and pick a zero-allocation engine-object path that uses Unity's destroyed-object check. Non-engine targets fall back to a `WeakReference` wrapper.

### Concurrency

```csharp
var (config, save, bundles) = await Ticket.WhenAll(
    LoadConfigAsync(),
    LoadSaveAsync(),
    LoadAddressablesAsync());

var (winnerIndex, result) = await Ticket.WhenAny(taskA, taskB);
```

### Threading

```csharp
await Ticket.SwitchToThreadPool();
ParseLargeFile();
await Ticket.SwitchToMainThread();
ApplyToScene();
```

### Cancellation

```csharp
var cts = new CancellationTokenSource();
cts.CancelAfterSlim(TimeSpan.FromSeconds(5));
try
{
    await SomeWorkAsync(cts.Token);
}
catch (OperationCanceledException)
{
    // expected
}
```

### MonoBehaviour cancellation token

```csharp
public partial class Player : MonoBehaviour
{
    private async TicketVoid Start()
    {
        var token = this.GetCancellationTokenOnDestroy();
        await PollServerAsync(token);
    }
}
```

The `GetCancellationTokenOnDestroy()` extension is provided by the Unity adapter (`CLabs.Tickets.Unity`).

### UnityAction adapters

```csharp
using CLabs.Tickets;

public partial class Menu : MonoBehaviour
{
    [Inject] private ISaveService saveService;

    private void OnEnable()
    {
        // Wrap an async method as a UnityAction for event bindings:
        saveButton.onClick.AddListener(TicketUnityActions.UnityAction(SaveAsync));
    }

    private async TicketVoid SaveAsync()
    {
        await saveService.SaveAsync();
    }
}
```

`UniTask` originally exposed these overloads as `UniTask.UnityAction(...)` via `partial struct UniTask`. In Tickets they live on a separate `TicketUnityActions` static class in the Unity adapter, because C# partial types cannot span assemblies (the engine-agnostic core can't reference `UnityEngine.Events.UnityAction`).

The same constraint applies to the end-of-frame helpers. `UniTask.WaitForEndOfFrame(MonoBehaviour)` and the `Awaitable.EndOfFrameAsync` overload are exposed by the adapter as `TicketUnityDelay.WaitForEndOfFrame(...)`:

```csharp
// Await Unity's native end-of-frame signal (preferred when no MonoBehaviour runner is handy)
await TicketUnityDelay.WaitForEndOfFrame(cancellationToken);

// Strict end-of-frame via coroutine runner (after all rendering, before present)
await TicketUnityDelay.WaitForEndOfFrame(this); // `this` is a MonoBehaviour
```

`Ticket.WaitForFixedUpdate()` stays in core. It is `Ticket.Yield(PlayerLoopTiming.LastFixedUpdate)` underneath and touches no Unity type.

## Perf characteristics

- **Struct-based `Ticket` type**: zero heap allocation on the fast path (same as upstream UniTask's `UniTask`)
- **Pooled promise objects**: `WaitUntil`, `Delay`, `WhenAll`, and every operator that needs an `ITicketSource` pulls from a per-type `TaskPool<T>` and returns on completion
- **Function-pointer hot path**: every `AddAction` / `AddContinuation` on the core side compiles to a direct `call` instruction via `TicketRuntime`'s static `delegate*<...>` fields, populated by the Unity adapter at `[RuntimeInitializeOnLoadMethod]` time. No interface dispatch, no boxing, no closures.
- **Zero-alloc engine-object detection**: `WaitUntilValueChanged(unityObj, ...)` and `EveryValueChanged(unityObj, ...)` hold the target by strong reference and check liveness through `TicketRuntime.IsEngineObjectAlive` (a function pointer to the adapter's `Object == null` check). No `WeakReference<T>` allocation per operation for Unity targets.
- **Unity math types**: `Vector2/3/4`, `Color`, `Rect`, `Bounds`, `Quaternion`, `Vector2Int`, `Vector3Int` are registered as "contains no managed references" during adapter startup, so `TaskPool<T>` and `Channel<T>` can take the faster no-reference-tracking path when used with these types.

In aggregate this matches upstream UniTask's perf profile. The engine split does not cost anything on the hot path.

## What changed from UniTask

If you're porting from UniTask to Tickets, the mental model is identical but the names changed:

| UniTask | Tickets |
|---|---|
| namespace `Cysharp.Threading.Tasks` | `CLabs.Tickets` |
| namespace `Cysharp.Threading.Tasks.Internal` | `CLabs.Tickets.Internal` |
| namespace `Cysharp.Threading.Tasks.Linq` | `CLabs.Tickets.Linq` |
| namespace `Cysharp.Threading.Tasks.CompilerServices` | `CLabs.Tickets.CompilerServices` |
| `struct UniTask` / `UniTask<T>` | `Ticket` / `Ticket<T>` |
| `struct UniTaskVoid` | `TicketVoid` |
| `IUniTaskSource` / `IUniTaskSource<T>` | `ITicketSource` / `ITicketSource<T>` |
| `UniTaskCompletionSource` / `UniTaskCompletionSourceCore<T>` | `TicketCompletionSource` / `TicketCompletionSourceCore<T>` |
| `IUniTaskAsyncEnumerable<T>` | `ITicketAsyncEnumerable<T>` |
| `UniTaskAsyncEnumerable` | `TicketAsyncEnumerable` |
| `AsyncUniTaskMethodBuilder` | `AsyncTicketMethodBuilder` |
| `UniTaskScheduler` | `TicketScheduler` |
| `UniTaskExtensions` | `TicketExtensions` |
| `UniTaskStatus` | `TicketStatus` |
| `UniTask.UnityAction(...)` | `TicketUnityActions.UnityAction(...)` (moved to Unity adapter) |
| `com.cysharp.unitask` | `com.clabs.tickets` (+ `com.clabs.adapter.unity.tickets`) |
| `UniTask.Analyzer` Roslyn analyzer | Not shipped; call `Ticket.Forget()` manually on fire-and-forget `TicketVoid` methods |

Note the namespace is **plural** (`CLabs.Tickets`) while the primary type is **singular** (`Ticket`). This mirrors .NET's `System.Threading.Tasks` / `Task` convention and avoids a namespace-type name collision.

## Attribution and license

Originally derived from [Cysharp UniTask](https://github.com/Cysharp/UniTask) 2.5.10, © 2019 Yoshifumi Kawai / Cysharp, Inc. (MIT). Cysharp's full copyright notice is preserved in [`LICENSE.md`](../LICENSE.md). Modifications by Crumpet Labs are also released under the MIT License.
