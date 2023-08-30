using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Dto;
using Checkout.PaymentGateway.Models;
using Checkout.PaymentGateway.Simulator;
using Checkout.PaymentGateway.Utils;

namespace Checkout.PaymentGateway.Infrastructure;

public interface IPaymentService
{
    IEnumerable<CurrencyDto> GetCurrency(int? id);
    Task<IEnumerable<TransactionDto>> GetTransaction(string merchantRef, string tRef);
    Task<ResponseDto<string>> MakePayments(PaymentDto content);
}

public class PaymentService : IPaymentService
{
    private readonly IEnumerable<CurrencyDto> _currencies = new List<CurrencyDto>()
    {
        new(1, "£"),
        new(2, "$"),
        new(3, "€"),
    };

    private IList<TransactionModel> _mockTransactionsDB = new List<TransactionModel>();
    private readonly CKOSimulator _ckoSimulator;
    private readonly IMessagingService _messagingService;
    
    public PaymentService(CKOSimulator cko, IMessagingService messagingService)
    {
        _ckoSimulator = cko ?? throw new ArgumentNullException($"{nameof(cko)} cannot be null");
        _messagingService = messagingService ??
                            throw new ArgumentNullException($"{nameof(messagingService)} cannot be null");
    }

    public IEnumerable<CurrencyDto> GetCurrency(int? id)
    {
        return id.HasValue ? 
            _currencies.Where(s =>  s.Id == id ) :
            _currencies;
    }

    public async Task<IEnumerable<TransactionDto>> GetTransaction(string merchantRef, string tRef)
    {
        //T
        return null;
    }

    public async Task<ResponseDto<string>> MakePayments(PaymentDto content)
    {
        var result = await _ckoSimulator
            .ProcessPayment(content.Card, content.Amount, content.CurrencyId, content.Merchant.Id);
        
        TransactionModel tModel = new()
        {
            Id = Guid.NewGuid(),
            Amount = content.Amount,
        };
        
        _mockTransactionsDB.Add(tModel);
        
        _messagingService.SendEventMessage(new()
        {
            Amount = tModel.Amount,
            Merchant = content.Merchant,
            PaymentDate = DateTime.UtcNow,
            TransactionReference = tModel.Id,
            CardDetails = new CardDto()
            {
                CardNumber = Utility.MaskCardNumber(content.Card.CardNumber),
                CardName = content.Card.CardName
            }
        });

        return new ResponseDto<string>(result.status ? MessageStrings.Successful : result.message);
    }
}