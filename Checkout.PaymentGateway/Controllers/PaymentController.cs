using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Dto;
using Checkout.PaymentGateway.Infrastructure;
using Checkout.PaymentGateway.Middleware;
using Microsoft.AspNetCore.Mvc;

namespace Checkout.PaymentGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
[ServiceFilter(typeof(IPValidator))]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService ??
                          throw new ArgumentNullException($"{nameof(paymentService)} cannot be empty");
    }

    // GET: /<controller>/
    [HttpPost("MakePayment")]
    public async Task<ResponseDto<TransactionDto>> MakePayment([FromBody] PaymentDto content)
    {
        /***
         * Request content is assumed to be encrypted for security response
         * But in the case, it is not
         */
        return await _paymentService.MakePayments(content);
    }

    [HttpGet("GetTransaction")]
    public async Task<ResponseDto<IEnumerable<TransactionDto>>> GetTransaction(string merchantRef, string tRef)
    {
        return await _paymentService.GetTransaction(merchantRef, tRef);
    }

    [HttpGet("GetCurrency")]
    public async Task<ResponseDto<IEnumerable<CurrencyDto>>> GetCurrency(int? currencyId)
    {
        return await _paymentService.GetCurrency(currencyId);
    }
}