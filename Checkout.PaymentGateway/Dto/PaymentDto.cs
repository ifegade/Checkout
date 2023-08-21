namespace Checkout.PaymentGateway.Dto;

public class PaymentDto
{
    public CardDto Card { get; set; }
    public MerchantDto Merchant { get; set; }
    public decimal Amount { get; set; }
    public int CurrencyId { get; set; }
}

public class CardDto
{
    public string CardNumber { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public string CVV { get; set; }
    public string CardName { get; set; }
}

public class MerchantDto
{
    public string Id { get; set; }
    public string Name { get; set; }
}