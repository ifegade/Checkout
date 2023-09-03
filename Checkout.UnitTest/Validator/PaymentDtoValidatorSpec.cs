using Checkout.PaymentGateway.Dto;
using Checkout.PaymentGateway.Validator;

namespace Checkout.UnitTest.Validator;

[TestFixture]
public class PaymentDtoValidatorSpec
{
    [Test]
    public async Task ValidatePaymentDtoReturn_Success()
    {
        var request = new PaymentDto();

        //var validator = new PaymentDtoValidator();
        
    }
}