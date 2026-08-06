# CLabs.Tickets

> **Forked from [Cysharp UniTask](https://github.com/Cysharp/UniTask) 2.5.10 (MIT) and heavily modified.** Tickets is a hard fork. The upstream code has been split into an engine-agnostic pure-C# core plus engine-specific adapters, rebranded from the `Cysharp.Threading.Tasks` / `UniTask` surface to `CLabs.Tickets` / `Ticket`, reorganised into Buttr's folder conventions, reworked to dispatch through static function pointers instead of direct `PlayerLoopHelper` calls, and is no longer tracking upstream. Cysharp's original copyright is preserved in [`LICENSE.md`](LICENSE.md) and all modifications are also MIT-licensed.

A cross-engine async/await task primitive, engine-decoupled into a pure-C# core with engine-specific adapters. Built so the CLabs ecosystem can use a single async primitive across .NET / Unity / Godot without depending on an externally-owned package, and so async work integrates cleanly with Buttr's dependency injection.

## Features

- **Full UniTask 2.5.10 surface**: `Ticket`, `Ticket<T>`, `TicketCompletionSource`, `ITicketAsyncEnumerable`, all ~70 LINQ operators, `WhenAll` / `WhenAny` generated overloads, threading switches, async reactive properties, channels.
- **Engine-agnostic core**: `com.clabs.tickets` has `noEngineReferences: true`, runs anywhere .NET runs.
- **Zero-allocation hot path**: function-pointer dispatch from core to engine; upstream UniTask perf parity.
- **Unity bindings via adapter**: `com.clabs.adapter.unity.tickets` provides PlayerLoop integration, MonoBehaviour message triggers, `AsyncOperation` / `Coroutine` / `UnityWebRequest` awaiters, and optional Addressables / DOTween / TextMeshPro integrations gated by version defines.
- **Buttr-aware**: Unity adapter auto-registers via `[RuntimeInitializeOnLoadMethod]`; in standalone .NET, call `builder.UseTicketPackage()` on `ApplicationBuilder`.

## Installation

### .NET projects

Clone the repo (or add as a submodule) and reference the project from your `.csproj`:

```xml
<ItemGroup>
  <ProjectReference Include="path/to/CLabs.Tickets/CLabs.Tickets.csproj" />
</ItemGroup>
```

Or, once published, via NuGet:

```bash
dotnet add package CLabs.Tickets
```

### Dependencies

- **CLabs**: none. The core is fully standalone.
- **External**: none for the pure-C# core. The Unity adapter (separate package, distributed via [CLabs.Unity](https://github.com/Crumpet-Labs/CLabs.Unity)) pulls in `Buttr.Core` + `com.clabs.utility`.

## Using it

```csharp
using CLabs.Tickets;

public class GameLoop {
    public async TicketVoid RunAsync() {
        await Ticket.Delay(1000);
        Console.WriteLine("One second later");

        await Ticket.WhenAll(
            Ticket.Delay(500),
            Ticket.Delay(750),
            Ticket.Delay(1000));

        Console.WriteLine("Three concurrent delays complete");
    }
}
```

> **Note on naming**: the namespace is `CLabs.Tickets` (plural), the primary type is `Ticket` (singular). This mirrors .NET's `System.Threading.Tasks` / `Task` convention and avoids a namespace–type collision.

## Architecture

```
CLabs.Tickets                              <- engine-agnostic core
  +- CLabs.Tickets                         primary Ticket struct + public API
  +- CLabs.Tickets.Linq                    ~70 AsyncEnumerable LINQ operators
  +- CLabs.Tickets.Internal                pools, continuation queues, helpers
  +- CLabs.Tickets.CompilerServices        AsyncTicketMethodBuilder, state-machine runner

CLabs.Tickets.Unity (Unity adapter)        <- ships in CLabs.Unity
  +- CLabs.Tickets.Unity                   PlayerLoopHelper, Triggers, UnityAsyncExtensions
  +- CLabs.Tickets.Unity.Editor            Ticket tracker window
  +- CLabs.Tickets.Addressables            Addressables integration (version-define gated)
  +- CLabs.Tickets.DOTween                 DOTween integration (version-define gated)
  +- CLabs.Tickets.TextMeshPro             TextMeshPro integration (version-define gated)
```

Core dispatches to the engine via static function pointers registered by the adapter. There is no interface dispatch on the hot path, and the codegen matches upstream UniTask's direct `PlayerLoopHelper.X` calls.

## Unity users

If you're building a Unity project, install the [CLabs.Unity](https://github.com/Crumpet-Labs/CLabs.Unity) UPM umbrella, which ships Tickets together with its Unity adapter. This repo is for plain .NET consumers.

## Attribution

This package is derived from [Cysharp UniTask](https://github.com/Cysharp/UniTask) 2.5.10. UniTask is © 2019 Yoshifumi Kawai / Cysharp, Inc. and is distributed under the MIT License. The full original notice is preserved in [`LICENSE.md`](LICENSE.md). Modifications by Crumpet Labs are also released under the MIT License.
