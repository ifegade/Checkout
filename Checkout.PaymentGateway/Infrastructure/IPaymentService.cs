using System.Collections.Generic;
using System.Linq;
using Checkout.PaymentGateway.Dto;

namespace Checkout.PaymentGateway.Infrastructure;

public interface IPaymentService
{
    IEnumerable<CurrencyDto> GetCurrency(int? id);
}

public class PaymentService : IPaymentService
{
    private readonly IEnumerable<CurrencyDto> _currencies = new List<CurrencyDto>()
    {
        new CurrencyDto(1, "£"),
        new CurrencyDto(2, "$"),
        new CurrencyDto(3, "€"),
    };
    
    public PaymentService()
    {
        
    }

    public IEnumerable<CurrencyDto> GetCurrency(int? id)
    {
        return id.HasValue ? 
            _currencies.Where(s =>  s.Id == id ) :
            _currencies;
    }
}