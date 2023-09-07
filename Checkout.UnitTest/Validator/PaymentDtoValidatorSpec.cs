using Checkout.PaymentGateway.Dto;
using Checkout.PaymentGateway.Infrastructure;
using Checkout.PaymentGateway.Validator;
using Checkout.UnitTest.Mock;
using FluentValidation.TestHelper;
using NSubstitute;
using Shouldly;
using NUnit.Framework;

namespace Checkout.UnitTest.Validator;

[TestFixture]
public class PaymentDtoValidatorSpec
{
    private PaymentDtoValidator _validator;
    private IPaymentService _paymentService;
    private IMerchantService _merchantService;

    [SetUp]
    public void SetUp()
    {
        _paymentService = Given.APaymentService;
        _merchantService = Given.AMerchantService;
        _validator = new PaymentDtoValidator(_paymentService, _merchantService);
    }

    [TearDown]
    public void Dispose()
    {
        _paymentService = null;
        _merchantService = null;
        _validator = null;
    }

    [Test]
    public void Amount_ShouldHaveValidationError_WhenAmountIsZero()
    {
        var paymentDto = new PaymentDto { Amount = 0 };
        var result = _validator.TestValidate(paymentDto);
        result.ShouldHaveValidationErrorFor(x => x.Amount)
              .WithErrorMessage("Amount must be greater than 0.");
    }

    [Test]
    public async Task MerchantAndCurrencyExist_ShouldNotHaveValidationError()
    {
        _merchantService.IsMerchantValid(Arg.Any<string>()).Returns(true);
        _paymentService.CurrencyExist(Arg.Any<int>()).Returns(true);

        var paymentDto = new PaymentDto
        {
            Amount = 100, Merchant = new MerchantDto { Id = "a32db69f-3794-4674-b58e-36fcb6319924" }, CurrencyId = 1,
            Card = new CardDto()
            {
                CardNumber = "378282246310005",
                CardName = "Test 01",
                CVV = "111",
                Year = 2023,
                Month = 1
            }
        };
        var result = await _validator.ValidateAsync(paymentDto);

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public async Task MerchantOrCurrencyDoesNotExist_ShouldHaveValidationError()
    {
        _merchantService.IsMerchantValid(Arg.Any<string>()).Returns(false);
        _paymentService.CurrencyExist(Arg.Any<int>()).Returns(true);

        var paymentDto = new PaymentDto { Merchant = new MerchantDto { Id = "a32db69f-3794-4674-b58e-36fcb6319924" }, CurrencyId = 1 };
        var result = await _validator.ValidateAsync(paymentDto);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorMessage == "Merchant/Currency does not exist.");
    }
    
    [Test]
    public async Task MerchantOrCurrencyDoesNotExist_2_ShouldHaveValidationError()
    {
        _merchantService.IsMerchantValid(Arg.Any<string>()).Returns(true);
        _paymentService.CurrencyExist(Arg.Any<int>()).Returns(false);

        var paymentDto = new PaymentDto { Merchant = new MerchantDto { Id = "a32db69f-3794-4674-b58e-36fcb6319924" }, CurrencyId = 1 };
        var result = await _validator.ValidateAsync(paymentDto);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorMessage == "Merchant/Currency does not exist.");
    }
    
    [Test]
    public async Task MerchantOrCurrencyDoesNotExist_3_ShouldHaveValidationError()
    {
        _merchantService.IsMerchantValid(Arg.Any<string>()).Returns(false);
        _paymentService.CurrencyExist(Arg.Any<int>()).Returns(false);

        var paymentDto = new PaymentDto { Merchant = new MerchantDto { Id = "a32db69f-3794-4674-b58e-36fcb6319924" }, CurrencyId = 1 };
        var result = await _validator.ValidateAsync(paymentDto);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorMessage == "Merchant/Currency does not exist.");
    }

    // Add more test methods for other validation rules...

    // Example tests for other rules
    [Test]
    public void CardName_ShouldHaveValidationError_WhenCardNameIsNull()
    {
        var paymentDto = new PaymentDto { Card = new CardDto { CardName = null } };
        var result = _validator.TestValidate(paymentDto);
        result.ShouldHaveValidationErrorFor(x => x.Card.CardName)
              .WithErrorMessage("Card Name cannot be empty");
    }

    [Test]
    public void CardNumber_ShouldHaveValidationError_WhenCardNumberIsInvalid()
    {
        var paymentDto = new PaymentDto { Card = new CardDto { CardNumber = "1234" } };
        var result = _validator.TestValidate(paymentDto);
        result.ShouldHaveValidationErrorFor(x => x.Card.CardNumber)
              .WithErrorMessage("Credit card number is not valid");
    }

    // Add more tests for other card-related validation rules...

    [Test]
    public void Year_ShouldHaveValidationError_WhenYearIsGreaterThanCurrentYearPlus5()
    {
        var paymentDto = new PaymentDto { Card = new CardDto { Year = DateTime.Now.Year + 6 } };
        var result = _validator.TestValidate(paymentDto);
        result.ShouldHaveValidationErrorFor(x => x.Card.Year)
              .WithErrorMessage("Card here is invalid");
    }
}