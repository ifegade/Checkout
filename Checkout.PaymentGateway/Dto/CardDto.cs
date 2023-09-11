namespace Checkout.PaymentGateway.Dto;

public class CardDto : CardBaseDto
{
    public int Year { get; set; }
    public string Cvv { get; set; }
    public int Month { get; set; }
}

public class CardBaseDto
{
    public string CardNumber { get; set; }
    public string CardName { get; set; }
}