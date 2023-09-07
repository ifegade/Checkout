using Checkout.PaymentGateway.Infrastructure;
using NSubstitute;

namespace Checkout.UnitTest.Mock;

public static class Given
{
    public static IPaymentService APaymentService => Substitute.For<IPaymentService>();
    public static IMerchantService AMerchantService => Substitute.For<IMerchantService>();
}