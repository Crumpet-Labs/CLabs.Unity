# CLabs.Belfry

A type-safe, key-scoped pub/sub messaging system built on the **Tower / Rope / Ring** metaphor. Decouples publishers from subscribers through a mediator with struct-based messages, disposable subscriptions, and priority-aware async ringing.

## What it provides

| Type | Purpose |
|------|---------|
| `IBellTower` | Top-level entry point. `Rope(key)` returns a `BellRope` for a given scope key. |
| `BellRope` | Per-key façade exposing `RingBell<T>(in T msg)`, `RingToll<T>(in T msg, priority)`, and `OnBell<T>(handler, priority)`. |
| `IBelfry` | Low-level subscription store backing the tower. Most code goes through `BellRope`. |
| `IPeal` / `Peal` / `IPealConfig` / `PealConfig` | Async queue + config for `RingToll`. Backed by an `IRingOrder`. |
| `IRingOrder` + `FairRoundRobinRingOrder` / `StrictPriorityRingOrder` | Pluggable dequeue strategies — fairness vs strict priority. |
| `BellMessage<T>` | `delegate void BellMessage<T>(in T message)` — the handler signature. |
| `UseBelfry()` | `ApplicationBuilder` extension registering `IBelfry`, `IPealFactory`, `IBellTower` as singletons. |

## Installation

### .NET projects

Clone the repo (or add as a submodule) and reference the project from your `.csproj`:

```xml
<ItemGroup>
  <ProjectReference Include="path/to/CLabs.Belfry/CLabs.Belfry.csproj" />
</ItemGroup>
```

Or, once published, via NuGet:

```bash
dotnet add package CLabs.Belfry
```

### Dependencies

You must install these alongside Belfry **in this order**:

1. **Buttr.Core** — the DI / application-builder framework. Belfry registers its services on an `ApplicationBuilder` from Buttr. → [Buttr.Core](https://github.com/Crumpet-Labs/Buttr.Core)
2. **CLabs.Tickets** — the async/await primitive `RingToll` returns. → [CLabs.Tickets](https://github.com/Crumpet-Labs/CLabs.Tickets)
3. **CLabs.Belfry** itself.

## Using it

### Register Belfry with Buttr

```csharp
using Buttr.Core;
using CLabs.Belfry;

var builder = new ApplicationBuilder();

builder.UseBelfry();   // registers IBelfry, IPealFactory, IBellTower

var app = builder.Build();
```

### Define a message

Messages are plain `readonly struct`s passed by `in` reference, so there's no allocation on the hot path.

```csharp
public readonly struct EnemyDefeated {
    public readonly int EntityId;
    public readonly int XPReward;

    public EnemyDefeated(int entityId, int xpReward) {
        EntityId = entityId;
        XPReward = xpReward;
    }
}
```

### Ring a bell (publish)

```csharp
using Buttr.Injection;
using CLabs.Belfry;

public sealed class CombatService {
    [Inject] private readonly IBellTower i_Tower;

    public void DefeatEnemy(int entityId, int xp) {
        i_Tower
            .Rope("combat")
            .RingBell(new EnemyDefeated(entityId, xp));
    }
}
```

### Hook a listener (subscribe)

`OnBell<T>(...)` returns an `IDisposable`. Hook on activation, dispose on teardown.

```csharp
using System;
using Buttr.Injection;
using CLabs.Belfry;

public sealed class XPListener : IDisposable {
    [Inject] private readonly IBellTower i_Tower;
    private IDisposable m_Subscription;

    public void Start() {
        m_Subscription = i_Tower
            .Rope("combat")
            .OnBell<EnemyDefeated>(OnEnemyDefeated);
    }

    public void Dispose() => m_Subscription?.Dispose();

    private void OnEnemyDefeated(in EnemyDefeated msg)
        => Console.WriteLine($"Entity {msg.EntityId} defeated -- +{msg.XPReward} XP");
}
```

### Async ringing through a Peal

`RingToll` queues each invocation through an `IPeal` backed by an `IRingOrder` (fairness or strict-priority). Critical priorities bypass the queue and run inline.

```csharp
using CLabs.Belfry;

var config = new PealConfig(
    strategy: new StrictPriorityRingOrder(),
    criticalPriorities: new[] { 100 });

await i_Tower
    .Rope("combat", config)
    .RingToll(new EnemyDefeated(42, 10), priority: 50);
```

## Unity users

If you're building a Unity project, install the [CLabs.Unity](https://github.com/Crumpet-Labs/CLabs.Unity) UPM umbrella — Belfry ships inside it together with its Unity adapter (`com.clabs.adapter.unity.belfry`). This repo is for plain .NET consumers.

## License

MIT.
