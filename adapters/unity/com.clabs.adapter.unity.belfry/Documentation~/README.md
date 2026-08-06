# CLabs Adapter: Belfry (Unity)

Unity bindings for `com.clabs.belfry`. The core Belfry package is engine-agnostic (pub/sub over `IBellTower`); this adapter provides the `ScriptableObject` ring-order wrappers and the application loader that plugs Belfry into the Unity player loop.

## What this provides

| Type | Purpose |
|---|---|
| `BellLoader` | `UnityApplicationLoaderBase` that initialises the Belfry tower on play |
| `PealConfigSo` | SO wrapper for `IPealConfig`; pairs a ring order with priority-critical rules |
| `RingOrderSo` | Abstract SO base for ring orders |
| `FairRoundRobinRingOrderSo` | Fair round-robin delivery across ropes |
| `StrictPriorityRingOrderSo` | Strict priority-ordered delivery |
| `Example/Publisher` + `Example/Subscriber` | Scene-ready MonoBehaviours demonstrating the API |

## Setup

1. Create a ring-order asset: `Create > CLabs > Belfry > ...`
2. Create a `PealConfigSo` and assign the strategy.
3. Register the loader in your Buttr application bootstrap.

## Dependencies

- `com.clabs.belfry`: core tower + contracts
- `com.clabs.tickets`: `Ticket` async primitive used by strategy signatures
- `com.clabs.utility`, `com.crumpetlabs.buttr`, `com.crumpetlabs.buttr.unity`

## See also

- [../Code-Index.md](../Code-Index.md)
- The `com.clabs.belfry` package's `Documentation~/README.md`
