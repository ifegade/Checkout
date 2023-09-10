using System;

namespace Checkout.PaymentGateway.Models;

public class TransactionModel : BaseModel
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public int CurrencyId { get; set; }

    public CurrencyModel Currency { get; set; }
    public string MerchantId { get; set; }
    public MerchantModel Merchant { get; set; }
    public CardModel Card { get; set; }
    public int CardId { get; set; }
    public TransactionStatusEnum Status { get; set; }
}