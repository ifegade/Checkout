using Checkout.PaymentGateway.Dto.Messaging;
using Serilog;

namespace Checkout.PaymentGateway.Infrastructure;

public interface IMessagingService
{
    void SendEventMessage(EventMessage message);
}

public class MessagingService : IMessagingService
{
    public void SendEventMessage(EventMessage message)
    {
        //send an event to a EventBus (Kafka/RabbitMQ)
        //this message will be sent to a queue that will be picked up by a consumer service that calls
        //the endpoint of the merchant to provide transaction notifications to the Merchant.

        Log.Information("Event Message {@details}", message);
    }
}