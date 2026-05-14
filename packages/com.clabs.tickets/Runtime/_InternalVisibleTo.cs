using System.Runtime.CompilerServices;

// Core Ticket assembly exposes its Internal/ types to the Linq operator
// assembly and to every Unity-adapter assembly so that PlayerLoopHelper,
// PlayerLoopRunner, and the External integrations can reach internal
// primitives like ContinuationQueue, MinimumQueue, and PooledDelegate
// without inflating the public API surface.
[assembly: InternalsVisibleTo("CLabs.Tickets.Linq")]
[assembly: InternalsVisibleTo("CLabs.Tickets.Unity")]
[assembly: InternalsVisibleTo("CLabs.Tickets.Unity.Editor")]
[assembly: InternalsVisibleTo("CLabs.Tickets.Addressables")]
[assembly: InternalsVisibleTo("CLabs.Tickets.DOTween")]
[assembly: InternalsVisibleTo("CLabs.Tickets.TextMeshPro")]
