using CLabs.Tickets;

namespace CLabs.Belfry {
    public delegate Ticket TollMessage<T>(T message) where T : struct;
}
