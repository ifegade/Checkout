namespace Checkout.PaymentGateway.Dto.Messaging;

public class EventMessage
{
    public EventType EventType { get; set; }
    public string Data { get; set; }
}

public enum EventType
{
    PaymentNotification
}