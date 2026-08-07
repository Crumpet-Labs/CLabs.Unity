# CLabs Belfry: Guide

> Historic note: renamed from `com.clabs.eda` (Event-Driven Architecture) in the kitchen-theme sweep. The public API moved from `IEventFactory.CreatePublisher/CreateSubscriber` to the Tower/Rope/Ring shape: `IBellTower.Rope(key).RingBell(msg)` to publish, `IBellTower.Rope(key).OnBell<T>(handler)` to subscribe. Internal types were also renamed: `EventService → Belfry`, `EventBuffer → Peal`, `IEventQueueStrategy → IRingOrder`, `FairRoundRobinStrategy → FairRoundRobinRingOrder`, `StrictPriorityStrategy → StrictPriorityRingOrder`.

## Overview

Belfry is the pub/sub messaging backbone for CLabs packages. It decouples publishers from subscribers through a mediator pattern: ring struct messages on keyed ropes, hook receivers for those ropes. Neither side knows the other exists.

Packages don't depend on Belfry directly. Each package uses local `Action<>` events internally, and Belfry *bridges* translate those local events into Belfry messages for cross-package communication.

## Architecture

```
IBellTower (singleton)
└── Rope(key, pealConfig?) → BellRope (readonly struct)
    ├── RingBell<T>(in T message)        (sync)
    │   └── Belfry.PublishBell(new BellChannel(key, typeof(T)), in message)
    │       └── Dictionary<BellChannel, List<Delegate>>
    │           └── foreach listener → BellMessage<T>(in message)
    ├── RingToll<T>(in T message, priority) → Ticket   (async)
    │   └── Peal.Enqueue → IRingOrder dispatch
    ├── OnBell<T>(BellMessage<T> handler) → IDisposable (sync subscribe)
    └── OnToll<T>(TollMessage<T> handler) → IDisposable (async subscribe)

IPealFactory (singleton)
└── CreatePeal(IPealConfig) → IPeal
    └── Peal
        ├── Enqueue(action, priority)
        │   ├── critical? → execute immediately
        │   └── normal → Strategy.Enqueue → ProcessQueueAsync
        └── IRingOrder
            ├── FairRoundRobinRingOrder
            └── StrictPriorityRingOrder
```

## Messages

All Belfry messages must be `struct` types. They're passed by `in` reference to avoid allocations.

```csharp
public readonly struct PlayerDiedMessage {
    public readonly int Owner;
    public readonly string CauseOfDeath;

    public PlayerDiedMessage(int owner, string causeOfDeath) {
        Owner = owner;
        CauseOfDeath = causeOfDeath;
    }
}
```

The handler delegate is:

```csharp
public delegate void BellMessage<T>(in T message) where T : struct;
```

## Ringing (publishing)

Inject `IBellTower` from Buttr, grab a `BellRope` for your channel key, and call `RingBell`:

```csharp
public sealed partial class GameManager : MonoBehaviour {
    [Inject] private IBellTower i_Tower;

    public void EndRound() {
        var message = new RoundEnded(roundNumber: 3, winner: "Blue");
        i_Tower.Rope(k.Towers.GameManager).RingBell(in message);
    }
}
```

The channel key can be anything, but typically it's good practice to keep a static class with constant values in it inside a shared Messages/ directory

I've found the best approach tto be something like
```csharp
public static class k {
    public static class Towers {
        public const int GameManager = 1;
    }
}
```

this means anywhere `k.Towers.GameManager` you can access it. 

Having said this it can be any object: a `string`, an enum, a `ScriptableObject`, or a `Type`.
The rope combines the key with `typeof(T)` to form a `BellChannel(scope, messageType)` lookup, so the same rope can carry multiple message types and listeners can pick out specific ones.

### Async ringing

`RingToll` enqueues the message through a peal so listeners are invoked off the calling frame, returning a `Ticket` that completes after delivery:

```csharp

var message = new StateFlushedMessage(slotId: "slot1");
await i_Tower.Rope(k.Towers.SaveSystem).RingToll(in message, priority: 10);
```

Async ringing requires the rope to have been built with a `IPealConfig` (see *Peals* below).

## Hooking (subscribing)

Call `Rope(key).OnBell<T>(handler)` and hold the returned `IDisposable`:

```csharp
public sealed partial class ScoreTracker : MonoBehaviour {
    [Inject] private IBellTower i_Tower;
    
    private IDisposable m_RoundEnded;
    private IDisposable m_PlayerDied;

    private void OnEnable() {
        var rope = i_Tower.Rope(k.Towers.GameManager);
        
        m_RoundEnded = rope.OnBell<RoundEndedMessage>(OnRoundEnded);
        m_PlayerDied = rope.OnBell<PlayerDiedMessage>(OnPlayerDied);
    }

    private void OnDisable() {
        m_RoundEnded?.Dispose();
        m_PlayerDied?.Dispose();
    }

    private void OnRoundEnded(in RoundEndedMessage msg) {
        Debug.Log($"Round {msg.RoundNumber} won by {msg.Winner}");
    }

    private void OnPlayerDied(in PlayerDiedMessage msg) {
        Debug.Log($"{msg.Owner} died: {msg.CauseOfDeath}");
    }
}
```

### Subscription lifecycle

`OnBell` returns an `IDisposable`. Disposing it removes the listener. The standard pattern:

- `OnEnable`: hook
- `OnDisable`: dispose

This keeps subscriptions tied to Unity's enable/disable lifecycle and avoids dangling handlers.

### Multiple listeners in one hook

`OnBell(params IBellListener[])` registers many at once; dispose the returned `IDisposable` to tear them all down. Wrap typed handlers in `BellListener<T>`:

```csharp
m_Subscription = rope.OnBell(
    new BellListener<RoundEndedMessage>(OnRoundEnded),
    new BellListener<PlayerDiedMessage>(OnPlayerDied)
);
```

## Channels and keys

The routing pair is `BellChannel(object scope, Type messageType)`:

- **scope**: the publisher identity, typically a `Type` like `k.Towers.CombatSystem`
- **messageType**: the message struct type, derived from the generic parameter on `RingBell<T>` / `OnBell<T>`

So:

- Two publishers using different scopes can publish the same message type without collision
- A subscriber must know the publisher's scope to receive its messages
- One rope (one scope) can carry multiple message types

`BellChannel` is a `readonly struct` implementing `IEquatable<BellChannel>`, with equality based on both members.

## Peals (async ringing)

A `Peal` is a priority-aware async queue for ringing operations that shouldn't execute simultaneously. They're separate from the sync ring path and are typically used by bridge mediators to serialise async work.

### Creating a peal-enabled rope

Instantiate a `PealConfig` with an `IRingOrder` strategy and pass it when grabbing the rope:

```csharp
public sealed class SaveMediator {
    private readonly BellRope m_SaveRope;

    public SaveMediator(IBellTower tower) {
        var pealConfig = new PealConfig(
            strategy: new FairRoundRobinRingOrder(),
            criticalPriorities: new[] { 100 }
        );
        
        m_SaveRope = tower.Rope(k.Towers.SaveMediator, pealConfig);
    }

    public void OnStateChanged() {
        var message = new SaveRequestedMessage(reason: "auto");
        m_SaveRope.RingToll(in message, priority: 5);
    }

    public void OnCriticalShutdown() {
        // Priority 100 is in the critical set: fires immediately, skipping the queue.
        var message = new SaveRequestedMessage(reason: "shutdown");
        m_SaveRope.RingToll(in message, priority: 100);
    }
}
```

Calling `Rope(key)` without a `PealConfig` produces a sync-only rope, so `RingToll` on that rope has no peal to enqueue through.

### IPealConfig

| Member | Type | Purpose |
|---|---|---|
| `Strategy` | `IRingOrder` | Controls dequeue order |
| `IsCritical(priority)` | `bool` | Priority values that bypass the queue entirely |

### Ring orders

Two built-in `IRingOrder` implementations are provided.

#### FairRoundRobinRingOrder

Round-robins across priority levels. Higher-priority levels are visited first in each rotation, but lower-priority work is never starved.

| Behaviour | Detail |
|---|---|
| Ordering | Priorities sorted descending (highest first) |
| Fairness | Each priority level gets one dequeue per rotation cycle |
| Starvation | Prevented; all levels participate in every cycle |

#### StrictPriorityRingOrder

Always dequeues from the highest-priority level first. Lower-priority actions only run when no higher-priority work remains.

| Behaviour | Detail |
|---|---|
| Ordering | Priorities sorted ascending; dequeue from highest |
| Fairness | None; highest priority drains completely first |
| Starvation | Possible; continuous high-priority work blocks lower levels |

### Custom ring orders

Implement `IRingOrder` directly for bespoke dequeue logic:

```csharp
public sealed class WeightedRingOrder : IRingOrder {
    public void Enqueue(Func<CancellationToken, Ticket> action, int priority) {
        // Your enqueue logic
    }

    public bool TryDequeue(out Func<CancellationToken, Ticket> action) {
        // Your dequeue logic
        action = default;
        return false;
    }

    public int Count => /* queued count */;
    public void Clear() { /* reset state */ }
}
```

## Delegates

| Delegate | Signature | Purpose |
|---|---|---|
| `BellMessage<T>` | `void(in T message) where T : struct` | The handler delegate for all Belfry subscriptions |

## Bridge pattern

Belfry depends on Tickets to function correctly. The intended ecosystem has each domain package exposing local `Action<>` events, and a small bridge assembly that translates those into Belfry messages. This keeps every domain package independent, since none of them references `CLabs.Belfry` in its own asmdef.

Each bridge follows the same shape:

1. Subscribe to the package's local `Action<>` events
2. Translate the event data into a message `struct`
3. Ring the struct on a dedicated rope via `IBellTower`
4. Implement `IDisposable` for cleanup

Intended bridge ecosystem (not all shipped yet):

| Bridge | Source package | What it rings |
|---|---|---|
| `dough-belfry` | Dough | XP gained, level up, milestone reached |
| `glaze-belfry` | Glaze | Effect applied, removed, ticked |
| `equipment-belfry` | Equipment | Item equipped, unequipped |
| `inventory-belfry` | Inventory | Item added, removed, moved, stack changed |
| `mint-belfry` | Mint | Balance changed, transaction completed |
| `spice-belfry` | Spice | Ability activated, cooldown started/ended |
| `temper-belfry` | Temper | State entered, exited, transition occurred |
| `stats-belfry` | Stats | Stat changed, modifier added/removed |
| `oven-belfry` | Oven | Day / month / season / phase / year transitions |
| `zest-belfry` | Zest | Reputation value + tier changes |
| `sprig-belfry` | Sprig | Talent unlock / refund / grant / revoke / respec |

## Buttr registration

`UseBelfry()` registers three singletons:

| Service | Implementation | Purpose |
|---|---|---|
| `IBelfry` | `Belfry` | The subscription store and dispatch core |
| `IPealFactory` | `PealFactory` | Builds `IPeal` instances from `IPealConfig` |
| `IBellTower` | `BellTower` | Entry point; produces `BellRope`s by key |

`IPealConfig` and `IRingOrder` are *not* registered globally. They are constructed at the call site when async ropes are needed. This keeps async ordering a per-rope choice rather than an application-wide one.
