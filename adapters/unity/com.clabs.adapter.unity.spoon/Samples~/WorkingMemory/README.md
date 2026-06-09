# The Lab's Working Memory — Spoon sample

The lab's live numbers — day, coins, Old Hob's temperature — live in **one reactive store**.
Spoon is Redux-style: you never mutate state directly, you **dispatch an action**, a **pure reducer**
returns a fresh snapshot, and **subscribers** are notified.

## Run it
1. Import this sample.
2. Add `WorkingMemoryBehaviour` to a GameObject and enter Play mode.
3. Press **S** (serve, +5 coin), **H** (heat oven, +40°C), **N** (new day). Watch the Console.

## The loop to learn
`Dispatch(action)` → `KitchenReducer.Reduce(state, action)` → new `KitchenState` → your `Subscribe`
observer fires. `IsSafeToServe` is a **selector** — derived from state, never stored.

## See it live (optional)
To inspect the store in **Window → Crumpet Labs → Spoon Stores DevTools**, register it with a Buttr
container at build time (the DevTools window reads Buttr's registry, populated by `builder.Build()`).
See the `com.clabs.adapter.unity.spoon` adapter README for the registration snippet.

## Save/load hook
`Store.Restore(state)` rehydrates a store from a saved snapshot — the bridge to Fork/Knife (later episodes).
