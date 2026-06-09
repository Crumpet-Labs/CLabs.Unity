# The Pass Bell — Belfry sample

When a batch of crumpets is ready, the line cook rings the **pass bell**. Front of house and
wash-up both react — with **zero references to each other or to the kitchen**. That decoupling is
the whole point of Belfry.

## Run it
1. Import this sample.
2. Create a loader asset: **Assets → Create → CLabs → Belfry Samples → Pass Bell Loader**, and add
   it to your application's loader set (same wiring as the Fork/Crumb application loaders — see the
   `com.clabs.adapter.unity.fork` adapter for the established pattern).
3. In a scene, add `PassBell`, `FrontOfHouse`, and `WashUp` to GameObjects.
4. Enter Play mode and press **Space** (or right-click `PassBell` → *Ring the pass bell*).

You'll see both stations log independently — that's one `RingBell` fanning out to two subscribers.

## What to look at
- `IBellTower.Rope("lab.service")` — names a channel; publisher and subscribers just agree on the key.
- `RingBell<CrumpetReady>(…)` — **fire-and-forget**. The publisher never waits.
- `OnBell<CrumpetReady>(…)` returns an `IDisposable` — dispose it in `OnDisable` (shown).

## Stretch: bells vs tolls
Belfry also has `RingToll<T>` / `OnToll<T>`, which return a `Ticket` you can **await** — use it when
the expo must wait for every station to acknowledge before clearing the rail. Try swapping `RingBell`
for `RingToll` and awaiting the result.
