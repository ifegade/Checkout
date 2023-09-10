namespace Checkout.PaymentGateway.Dto;

public class PaymentDto
{
    public PaymentDto()
    {
        Card = new();
        Merchant = new();
    }

    public CardDto Card { get; set; }
    public MerchantDto Merchant { get; set; }
    public decimal Amount { get; set; }
    public int CurrencyId { get; set; }
}