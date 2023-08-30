using System;

namespace Checkout.PaymentGateway.Models;

public class TransactionModel
{
    public Guid Id { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DataUpdated { get; set; }
    public decimal Amount { get; set; }
}