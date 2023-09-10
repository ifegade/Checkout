namespace Checkout.PaymentGateway.Dto;

public class CurrencyDto
{
    public CurrencyDto(int id, string character)
    {
        Id = id;
        Character = character;
    }

    public int Id { get; set; }
    public string Character { get; set; }
}