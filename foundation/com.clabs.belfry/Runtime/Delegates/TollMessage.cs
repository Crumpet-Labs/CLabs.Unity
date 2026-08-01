using CLabs.Tickets;

namespace CLabs.Belfry {
    public delegate Ticket TollMessage<T>(in T message) where T : struct;
}
