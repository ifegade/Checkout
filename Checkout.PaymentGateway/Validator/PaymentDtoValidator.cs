using System;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Dto;
using Checkout.PaymentGateway.Infrastructure;
using FluentValidation;

namespace Checkout.PaymentGateway.Validator;

public class PaymentDtoValidator
    : AbstractValidator<PaymentDto>
{
    public PaymentDtoValidator(IPaymentService paymentService, IMerchantService merchantService)
    {
        RuleFor(s => s.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than 0.");


        RuleFor(s => s)
            .MustAsync(async (request, context) =>
                {
                    var isMerchantValid = merchantService.IsMerchantValid(request.Merchant.Id);
                    var isCurrencyValid = paymentService.CurrencyExist(request.CurrencyId);

                    Task.WhenAll(isCurrencyValid, isMerchantValid);

                    return isMerchantValid.Result && isCurrencyValid.Result;
                }
            ) 
            .WithMessage("Merchant/Currency does not exist.");

        RuleFor(s => s.Card.CardName)
            .NotNull()
            .NotEmpty()
            .WithMessage("Card Name cannot be empty");

        RuleFor(s => s.Card.CardNumber)
            .NotNull()
            .NotEmpty()
            .CreditCard()
            .WithMessage("Credit card number is not valid");

        RuleFor(s => s.Card.Month)
            .GreaterThan(0)
            .LessThanOrEqualTo(12)
            .WithMessage("Card Month is Invalid");

        RuleFor(s => s.Card.CVV)
            .NotNull()
            .NotEmpty()
            .Length(3)
            .WithMessage("Card CVV is Invalid");

        RuleFor(s => s.Card.Year)
            .LessThanOrEqualTo(DateTime.Now.Year)
            .LessThanOrEqualTo(DateTime.Now.Year + 5)
            .WithMessage("Card here is invalid");
    }
}