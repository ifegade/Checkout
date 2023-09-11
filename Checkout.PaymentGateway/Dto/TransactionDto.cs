using System;
using Checkout.PaymentGateway.Models;

namespace Checkout.PaymentGateway.Dto;

public class TransactionDto
{
    public Guid Id { get; set; }
    public DateTime DateCreated { get; set; }
    public decimal Amount { get; set; }
    public CurrencyDto Currency { get; set; }
    public CardBaseDto Card { get; set; }
    public MerchantDto Merchant { get; set; }
    public TransactionStatusEnum Status { get; set; }
}