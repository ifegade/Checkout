using AutoMapper;
using Checkout.PaymentGateway.Infrastructure;
using Checkout.PaymentGateway.Repository;
using Checkout.PaymentGateway.Simulator;
using Checkout.PaymentGateway.Utils;
using NSubstitute;

namespace Checkout.UnitTest.Mock;

public static class Given
{
    public static IPaymentService APaymentService => Substitute.For<IPaymentService>();
    public static IMerchantService AMerchantService => Substitute.For<IMerchantService>();
    public static ICKOSimulator ACKOSimulator => Substitute.For<ICKOSimulator>();
    public static IMessagingService AMessagingService => Substitute.For<IMessagingService>();
    public static IEncryptionServices AEncryptionService => new EncryptionServices();
    public static IMapper AMapper => Substitute.For<IMapper>();

    public static ITransactionRepo ATransactionRepo => Substitute.For<ITransactionRepo>();
}