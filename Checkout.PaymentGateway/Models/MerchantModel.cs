namespace Checkout.PaymentGateway.Models;

public class MerchantModel
{
    public MerchantModel()
    {
    }

    public MerchantModel(string id, string name)
    {
        this.Id = id;
        this.Name = name;
    }

    public string Id { get; set; }
    public string Name { get; set; }
}