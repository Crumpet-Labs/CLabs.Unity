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

            while (m_Config.Strategy.Count > 0 && false == ct.IsCancellationRequested) {
                if (m_Config.Strategy.TryDequeue(out var action)) {
                    try {
                        await action(ct);
                        await Ticket.Yield(PlayerLoopTiming.LastPostLateUpdate, ct);
                    } catch (OperationCanceledException) { /* todo: I should probably log something here */ }
                }
            }

            m_IsProcessing = false;
        }

        public int Count => m_Config.Strategy.Count;

        public void Dispose() {
            m_LifetimeCts?.Cancel();
            m_LifetimeCts?.Dispose();
            m_Config.Strategy.Clear();
        }
    }

    // EXAMPLE
    // public class ValidationMediator
    // {
    //     private readonly IEventBuffer m_EventBuffer;
    //     private readonly IValidationService m_ValidationService;
    //     private readonly IDatabaseService m_DatabaseService;
    //
    //     public ValidationMediator(IEventBuffer eventBuffer, IValidationService validationService, IDatabaseService databaseService)
    //     {
    //         m_EventBuffer = eventBuffer;
    //         m_ValidationService = validationService;
    //         m_DatabaseService = databaseService;
    //     }
    //
    //     public void OnUserEditedNode()
    //     {
    //         // Mediator coordinates: validate, then save
    //         m_EventBuffer.Enqueue(async ct =>
    //         {
    //             await m_ValidationService.ValidateAsync(ct);
    //             await m_DatabaseService.SaveAsync(ct);
    //         }, priority: 5);
    //     }
    //     
    //     public void OnCriticalShutdown()
    //     {
    //         // Critical priority - executes immediately
    //         m_EventBuffer.Enqueue(async ct =>
    //         {
    //             await m_DatabaseService.FlushAsync(ct);
    //         }, priority: 100);
    //     }
    // }
}