# CLabs Belfry

A type-safe, key-scoped pub/sub messaging system for Unity 6+. Decouples publishers from subscribers through a mediator pattern with struct-based messages, disposable subscriptions, and priority-aware async ringing.

## Features

- **Tower / Rope / Ring API** — inject `IBellTower`, grab a `BellRope` for a key, ring messages or hook listeners
- **Key-scoped channels** — every subscription is keyed by `(scope, messageType)`, so the same struct can travel multiple ropes without collision
- **Struct messages** — all Belfry messages are `readonly struct` passed by `in` reference for zero-allocation publishing
- **Disposable subscriptions** — `On<T>(...)` returns `IDisposable`. Hook in `OnEnable`, dispose in `OnDisable`
- **Async ringing via Peals** — `RingAsync` queues through an `IPeal` with priority-aware ordering
- **Two built-in ring orders** — `FairRoundRobinRingOrder` (no starvation) and `StrictPriorityRingOrder` (highest first)
- **Critical priority bypass** — entries flagged critical in the `IPealConfig` execute immediately, skipping the queue
- **Custom ring orders** — implement `IRingOrder` for bespoke dequeue logic

## Installation

Add to your `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.clabs.belfry": "https://github.com/Crumpet-Labs/Belfry.git"
  }
}
```

### Dependencies

- `com.crumpetlabs.buttr` — DI / architecture container
- `com.clabs.tickets` — async primitive used by the `Peal` machinery

## Quick Start

### Define a message

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

### Ring (publish)

```csharp
using Buttr.Injection;
using CLabs.Belfry;
using UnityEngine;

public sealed partial class CombatSystem : MonoBehaviour {
    [Inject] private IBellTower i_Tower;

    public void DefeatEnemy(int entityId, int xp) {
        var message = new EnemyDefeatedMessage(entityId, xp);
        i_Tower.Rope(k.Towers.CombatSystem).Ring();
    }
}
```

### Hook (subscribe)

```csharp
using System;
using Buttr.Injection;
using CLabs.Belfry;
using UnityEngine;

public sealed partial class XPListener : MonoBehaviour {
    [Inject] private IBellTower i_Tower;
    private IDisposable m_Subscription;

    private void OnEnable() {
        m_Subscription = i_Tower
            .Rope(k.Towers.CombatSystem)
            .On<EnemyDefeatedMessage>(OnEnemyDefeated);
    }

    private void OnDisable() {
        m_Subscription?.Dispose();
    }

    private void OnEnemyDefeated(in EnemyDefeatedMessage msg) {
        Debug.Log($"Enemy {msg.EntityId} defeated — +{msg.XPReward} XP");
    }
}
```

## Buttr Integration

```csharp
using Buttr.Core;
using CLabs.Belfry;

namespace YourProject {
    public sealed class Program {
        public static ApplicationContainer Main() => Main(CMDArgs.Get());
        
        private static ApplicationContainer Main(Dictionary<string, string> args) {
            var builder = new ApplicationBuilder();
            
            builder.UseBelfry();
            
            return builder.Build();
        }
    }
}
```

`UseBelfry()` registers `IBelfry`, `IPealFactory`, and `IBellTower` as singletons. `IPealConfig` and `IRingOrder` are user-provided when constructing async ropes.

## Documentation

- [Guide](Guide.md) — full usage guide covering ringing, hooking, peals, ring orders, and the bridge pattern
