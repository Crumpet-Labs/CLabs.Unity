using System;
using System.Threading;
using CLabs.Tickets;

namespace CLabs.Belfry {
    public sealed class Peal : IPeal, IDisposable {
        private readonly IPealConfig m_Config;
        private readonly CancellationTokenSource m_LifetimeCts = new();
        private bool m_IsProcessing;

        public Peal(IPealConfig config) {
            m_Config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public void Enqueue(Func<CancellationToken, Ticket> action, int priority = 0) {
            if (m_Config.IsCritical(priority)) {
                action(m_LifetimeCts.Token).Forget();
            }
            else {
                m_Config.Strategy.Enqueue(action, priority);

                if (false == m_IsProcessing)
                    ProcessQueueAsync(m_LifetimeCts.Token).Forget();
            }
        }

        private async TicketVoid ProcessQueueAsync(CancellationToken ct) {
            m_IsProcessing = true;
            try {
                while (m_Config.Strategy.Count > 0 && false == ct.IsCancellationRequested) {
                    if (false == m_Config.Strategy.TryDequeue(out var action)) continue;
                    try {
                        await action(ct);
                        await Ticket.Yield(PlayerLoopTiming.LastPostLateUpdate, ct);
                    } catch (OperationCanceledException) {
                        return;
                    } catch {
                        // Caller already observed the exception via the action's TCS; keep draining.
                    }
                }
            } finally {
                m_IsProcessing = false;
            }
        }

        public int Count => m_Config.Strategy.Count;

        public void Dispose() {
            m_LifetimeCts?.Cancel();
            m_LifetimeCts?.Dispose();
            m_Config.Strategy.Clear();
        }
    }
}
