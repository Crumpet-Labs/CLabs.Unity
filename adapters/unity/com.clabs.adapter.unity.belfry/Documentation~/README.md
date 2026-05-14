# CLabs Adapter: Belfry (Unity)

Unity bindings for [`com.clabs.belfry`](../../../../packages/com.clabs.belfry). The core Belfry package is engine-agnostic (pub/sub over `IBellTower`); this adapter provides the `ScriptableObject` queue-strategy wrappers and the application loader that plugs Belfry into the Unity player loop.

## What this provides

| Type | Purpose |
|---|---|
| `BellLoader` | `UnityApplicationLoaderBase` that initialises the Belfry tower on play |
| `EventBufferConfigSO` | SO wrapper for `IEventBufferConfig` — pairs a queue strategy with priority-critical rules |
| `EventQueueStrategySO` | Abstract SO base for queue strategies |
| `FairRoundRobinStrategySO` | Fair round-robin delivery across ropes |
| `StrictPriorityStrategySO` | Strict priority-ordered delivery |
| `Example/Publisher` + `Example/Subscriber` | Scene-ready MonoBehaviours demonstrating the API |

## Setup

1. Create a strategy asset: `Create > CLabs > Belfry > ...`
2. Create an `EventBufferConfigSO` and assign the strategy.
3. Register the loader in your Buttr application bootstrap.

## Dependencies

- `com.clabs.belfry` — core tower + contracts
- `com.clabs.tickets` — `Ticket` async primitive used by strategy signatures
- `com.clabs.utility`, `com.crumpetlabs.buttr`, `com.crumpetlabs.buttr.unity`

## See also

- [../Code-Index.md](../Code-Index.md)
- [com.clabs.belfry/Documentation~/README.md](../../../../packages/com.clabs.belfry/Documentation~/README.md)
