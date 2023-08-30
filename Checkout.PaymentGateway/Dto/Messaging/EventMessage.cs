using System;

namespace Checkout.PaymentGateway.Dto.Messaging;

public class EventMessage
{
    public CardDto CardDetails { get; set; }
    public Guid TransactionReference { get; set; }
    public DateTime PaymentDate { get; set; }
    public MerchantDto Merchant { get; set; }
    public decimal Amount { get; set; }
}