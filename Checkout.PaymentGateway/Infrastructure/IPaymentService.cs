using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Checkout.PaymentGateway.Dto;
using Checkout.PaymentGateway.Dto.Messaging;
using Checkout.PaymentGateway.Models;
using Checkout.PaymentGateway.Repository;
using Checkout.PaymentGateway.Simulator;
using Checkout.PaymentGateway.Utils;
using Newtonsoft.Json;

namespace Checkout.PaymentGateway.Infrastructure;

public interface IPaymentService
{
    Task<ResponseDto<IEnumerable<CurrencyDto>>> GetCurrency(int? id);
    Task<bool> CurrencyExist(int id);
    Task<ResponseDto<IEnumerable<TransactionDto>>> GetTransaction(string merchantRef, string tRef);
    Task<ResponseDto<TransactionDto>> MakePayments(PaymentDto content);
}

public class PaymentService : IPaymentService
{
    private readonly ICkoSimulator _ckoSimulator;

    //this is a mock currency repository
    private readonly IEnumerable<CurrencyDto> _currencies = new List<CurrencyDto>
    {
        new(1, "£"),
        new(2, "$"),
        new(3, "€")
    };

    private readonly IEncryptionServices _encryptionService;

    private readonly IMapper _mapper;
    private readonly IMessagingService _messagingService;
    private readonly ITransactionRepo _transactionRepo;

    public PaymentService(
        ICkoSimulator cko,
        IMessagingService messagingService,
        IEncryptionServices encryptionService,
        IMapper mapper,
        ITransactionRepo transactionRepo)
    {
        _ckoSimulator = cko ??
                        throw new ArgumentNullException($"{nameof(cko)} cannot be null");
        _messagingService = messagingService ??
                            throw new ArgumentNullException($"{nameof(messagingService)} cannot be null");
        _encryptionService = encryptionService ??
                             throw new ArgumentNullException($"{nameof(encryptionService)} cannot be null");
        _mapper = mapper ??
                  throw new ArgumentNullException($"{nameof(mapper)} cannot be null");
        _transactionRepo = transactionRepo ??
                           throw new ArgumentNullException($"{nameof(transactionRepo)} cannot be null");
    }

    public async Task<ResponseDto<IEnumerable<CurrencyDto>>> GetCurrency(int? id)
    {
        var output = id.HasValue ? _currencies.Where(s => s.Id == id) : _currencies;

        return new ResponseDto<IEnumerable<CurrencyDto>>(output);
    }

    public async Task<bool> CurrencyExist(int id)
    {
        return _currencies.Any(s => s.Id == id);
    }

    public async Task<ResponseDto<IEnumerable<TransactionDto>>> GetTransaction(string merchantRef, string tRef)
    {
        var transactions = await _transactionRepo.GetTransaction(merchantRef, tRef);

        if (!string.IsNullOrEmpty(tRef))
            transactions = transactions.Where(s => s.Id == Guid.Parse(tRef))
                .AsQueryable();
        if (!string.IsNullOrEmpty(merchantRef))
            transactions = transactions.Where(s => s.MerchantId == merchantRef)
                .AsQueryable();

        return new ResponseDto<IEnumerable<TransactionDto>>(
            _mapper.Map<IEnumerable<TransactionDto>>(transactions));
    }

    public async Task<ResponseDto<TransactionDto>> MakePayments(PaymentDto content)
    {
        var result = await _ckoSimulator
            .ProcessPayment(content.Card, content.Amount, content.CurrencyId, content.Merchant.Id);

        TransactionModel tModel = new()
        {
            Id = Guid.NewGuid(),
            Amount = content.Amount,
            CurrencyId = content.CurrencyId,
            Currency = _currencies.Where(s => s.Id == content.CurrencyId).Select(r => new CurrencyModel
            {
                Character = r.Character
            }).FirstOrDefault(),
            MerchantId = content.Merchant.Id,
            //defining merchant is not required here since it was validated at a controller level.
            //specifying it here is for the purpose of having a detailed response.
            Merchant = new MerchantModel
            {
                Name = content.Merchant.Name,
                Id = content.Merchant.Id
            },
            Status = result.status ? TransactionStatusEnum.Successful : TransactionStatusEnum.Failed,
            Card = new CardModel
            {
                CardName = _encryptionService.EncryptString(content.Card.CardName),
                CardNumber = _encryptionService.EncryptString(content.Card.CardNumber),
                CVV = _encryptionService.EncryptString(content.Card.Cvv),
                Year = content.Card.Year,
                Month = content.Card.Month
            }
        };

        /***
         *This is meant to be an asynchronous call to execute a Store Procedure
         *written in SQL. The reason for using SQL is because the data is structured
         */
        await _transactionRepo.Add(tModel);

        /***
         * send event message to a messaging bus for the following reasons
         *---- Notification to Merchant API
         *---- Logging
         *---- Analytics/Measurement/Experiement etc
         */
        _messagingService.SendEventMessage(new EventMessage
        {
            EventType = EventType.PaymentNotification,
            Data = _encryptionService.EncryptString(JsonConvert.SerializeObject(tModel))
        });

        if (result.status)
            return new ResponseDto<TransactionDto>(_mapper.Map<TransactionDto>(tModel));
        return new ResponseDto<TransactionDto>(ResponseCode.Unsuccessful, result.message);
    }
}