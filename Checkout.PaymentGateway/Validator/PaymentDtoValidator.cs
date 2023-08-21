using Checkout.PaymentGateway.Dto;
using Checkout.PaymentGateway.Infrastructure;
using FluentValidation;

namespace Checkout.PaymentGateway.Validator;

public class PaymentDtoValidator
    : AbstractValidator<PaymentDto>
{
    public PaymentDtoValidator(IPaymentService service)
    {
        
    }
}