# CLabs Adapter: Unity Tickets

Unity bindings for [`com.clabs.tickets`](../../../../packages/com.clabs.tickets/Documentation~/README.md) — the engine-agnostic async/await primitive hard-forked from Cysharp UniTask 2.5.10 and heavily modified. Without this adapter, Tickets core cannot tick, measure frames, or detect destroyed Unity objects.

## What this provides

- **PlayerLoop integration** — `PlayerLoopHelper` + `PlayerLoopTimer` drive Tickets' scheduling from Unity's 14-slot PlayerLoop
- **MonoBehaviour message triggers** — `AsyncTriggerHandler<T>` for every Unity message (`Update`, `FixedUpdate`, `OnEnable`, `OnDisable`, collision/trigger events, pointer events, etc.) — ~80 handlers total
- **AsyncOperation / Coroutine / UnityWebRequest awaiters** — `await` any `AsyncOperation`, `IEnumerator` coroutine, or `UnityWebRequest`
- **`TicketUnityActions`** — adapter for wrapping async methods as `UnityAction` callbacks (moved here from core because `UnityEngine.Events.UnityAction` is Unity-only)
- **Engine-object detection** — zero-allocation destroyed-object checks for `WaitUntilValueChanged` and `EveryValueChanged` when the target is a `UnityEngine.Object`
- **Unity math type registration** — `Vector2/3/4`, `Color`, `Rect`, `Bounds`, `Quaternion`, `Vector2Int`, `Vector3Int` registered as "contains no managed references" so pools/channels can skip reference tracking for them
- **Optional integrations** (version-define gated):
  - `com.unity.addressables` → `CLabs.Tickets.Addressables`
  - DOTween → `CLabs.Tickets.DOTween`
  - TextMeshPro → `CLabs.Tickets.TextMeshPro`
- **Ticket Tracker window** — `Window > Ticket Tracker` shows active `Ticket`s for debugging (same tool UniTask shipped, renamed)

## How it works

The adapter registers static function pointers with core's `TicketRuntime` at `[RuntimeInitializeOnLoadMethod(BeforeSceneLoad)]`. Every `Ticket.Delay`, `Ticket.WaitUntil`, `await` scheduling call on the core side compiles to a direct `call` instruction through `TicketRuntime.AddAction` / `AddContinuation`, which forwards to `PlayerLoopHelper` in this adapter. No interface dispatch, no boxing, no per-call allocation — matches upstream UniTask's hot-path codegen exactly.

## Setup

1. Add package dependency: `com.clabs.adapter.unity.tickets` (pulls in `com.clabs.tickets` automatically)
2. Register in Buttr: `builder.UseUnityTicketsPackage()` (optional — the adapter also self-registers via `[RuntimeInitializeOnLoadMethod]`, so `await Ticket.Delay(...)` works even before `ApplicationBuilder.Build()` runs)
3. Use `Ticket` / `Ticket<T>` anywhere you'd have used `UniTask` / `UniTask<T>`

No Unity-side configuration is required — the PlayerLoop injection happens automatically when the adapter assembly loads.

## Dependencies

- `com.clabs.tickets` — engine-agnostic Tickets core
- `com.clabs.utility` — Registry / Disposable helpers
- `com.crumpetlabs.buttr` — DI framework

## Attribution

Forked from [Cysharp UniTask](https://github.com/Cysharp/UniTask) 2.5.10 (MIT). Cysharp's original copyright is preserved in [`../../packages/com.clabs.tickets/LICENSE.md`](../../../../packages/com.clabs.tickets/LICENSE.md).
