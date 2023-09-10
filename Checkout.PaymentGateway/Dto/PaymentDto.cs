namespace Checkout.PaymentGateway.Dto;

public class PaymentDto
{
    public PaymentDto()
    {
        Card = new CardDto();
        Merchant = new MerchantDto();
    }

    public CardDto Card { get; set; }
    public MerchantDto Merchant { get; set; }
    public decimal Amount { get; set; }
    public int CurrencyId { get; set; }
}