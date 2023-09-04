using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Dto;
using Checkout.PaymentGateway.Infrastructure;
using Checkout.PaymentGateway.Middleware;
using Microsoft.AspNetCore.Mvc;

namespace Checkout.PaymentGateway.Controllers
{
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
        public async Task<IActionResult> MakePayment([FromBody] PaymentDto content)
        {
            return Ok(await _paymentService.MakePayments(content));
        }

        [HttpGet("GetTransaction")]
        public async Task<IActionResult> GetTransaction(string merchantRef, string tRef)
        {
            return Ok(_paymentService.GetTransaction(merchantRef, tRef));
        }

        [HttpGet("GetCurrency")]
        public async Task<IActionResult> GetCurrency(int? currencyId)
        {
            return Ok(_paymentService.GetCurrency(currencyId));
        }
    }
}

