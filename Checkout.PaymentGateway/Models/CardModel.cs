namespace Checkout.PaymentGateway.Models;

public class CardModel : BaseModel
{
    public string CardNumber { get; set; }
    public string CardName { get; set; }
    public int Year { get; set; }
    public string CVV { get; set; }
    public int Month { get; set; }
}