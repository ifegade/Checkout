using AutoMapper;
using Checkout.PaymentGateway.Dto;
using Checkout.PaymentGateway.Infrastructure;
using Checkout.PaymentGateway.Models;
using Checkout.PaymentGateway.Repository;
using Checkout.PaymentGateway.Simulator;
using Checkout.PaymentGateway.Utils;
using Checkout.UnitTest.Mock;
using NSubstitute;
using Shouldly;

namespace Checkout.UnitTest.Services;

[TestFixture]
public class PaymentServiceSpec
{
    [SetUp]
    public void Setup()
    {
        _mockCKOSimulator = Given.ACKOSimulator;
        _mockMessagingService = Given.AMessagingService;
        _mockEncryptionService = Given.AEncryptionService;
        _mockMapper = Given.AMapper;
        _mockTransactionRepo = Given.ATransactionRepo;
        _paymentService = new PaymentService(
            _mockCKOSimulator,
            _mockMessagingService,
            _mockEncryptionService,
            _mockMapper,
            _mockTransactionRepo);
    }


    [TearDown]
    public void TearDown()
    {
        _paymentService = null;
        _mockCKOSimulator = null;
        _mockMessagingService = null;
        _mockEncryptionService = null;
        _mockMapper = null;
    }

    private PaymentService _paymentService;
    private ICKOSimulator _mockCKOSimulator;
    private IMessagingService _mockMessagingService;
    private IEncryptionServices _mockEncryptionService;
    private IMapper _mockMapper;
    private ITransactionRepo _mockTransactionRepo;

    [Test]
    public async Task GetCurrency_WithValidId_ReturnsCorrectCurrency()
    {
        var validCurrencyId = 1;

        var result = await _paymentService.GetCurrency(validCurrencyId);

        result.ShouldNotBeNull();
        result.isSuccessful.ShouldBeTrue();
        result.content.Single().Id.ShouldBe(validCurrencyId);
    }

    [Test]
    public async Task GetCurrency_WithInvalidId_ReturnsEmptyList()
    {
        var invalidCurrencyId = 99;

        var result = await _paymentService.GetCurrency(invalidCurrencyId);

        result.ShouldNotBeNull();
        result.isSuccessful.ShouldBeTrue();
        result.content.Count().ShouldBe(0);
    }

    [Test]
    public async Task CurrencyExist_WhenCurrencyExists_ReturnsTrue()
    {
        var validCurrencyId = 1; // Assuming this ID exists in your test data

        var result = await _paymentService.CurrencyExist(validCurrencyId);

        result.ShouldBeTrue();
    }

    [Test]
    public async Task CurrencyExist_WhenCurrencyDoesNotExist_ReturnsFalse()
    {
        var invalidCurrencyId = 99; // Assuming this ID does not exist in your test data

        var result = await _paymentService.CurrencyExist(invalidCurrencyId);

        result.ShouldBeFalse();
    }

    [Test]
    public async Task GetTransaction_WithMerchantRef_ReturnsFilteredTransactions()
    {
        var merchantRef = "Merchant123";
        string tRef = null; // Set tRef to null to ignore it
        var transactions = new List<TransactionModel>
        {
            new() { Id = Guid.NewGuid(), MerchantId = merchantRef },
            new() { Id = Guid.NewGuid(), MerchantId = "Merchant124" },
            new() { Id = Guid.NewGuid(), MerchantId = merchantRef }
        }.AsQueryable();

        _mockTransactionRepo.GetTransaction(merchantRef, tRef)
            .Returns(transactions);

        _mockMapper.Map<IEnumerable<TransactionDto>>(Arg.Any<IEnumerable<TransactionModel>>())
            .Returns(new List<TransactionDto> { new(), new() });

        var result = await _paymentService.GetTransaction(merchantRef, tRef);

        Assert.IsNotNull(result);
        Assert.IsTrue(result.isSuccessful);
        var transactionDtos = result.content.ToList();
        Assert.AreEqual(2, transactionDtos.Count); // Expecting 2 transactions with the given merchantRef
    }

    [Test]
    public async Task GetTransaction_WithTRef_ReturnsFilteredTransactions()
    {
        string merchantRef = null;
        var tRef = Guid.NewGuid().ToString();
        var transactions = new List<TransactionModel>
        {
            new() { Id = Guid.NewGuid(), MerchantId = "Merchant123" },
            new() { Id = Guid.Parse(tRef), MerchantId = "Merchant124" },
            new() { Id = Guid.NewGuid(), MerchantId = "Merchant123" }
        }.AsQueryable();
        _mockTransactionRepo.GetTransaction(Arg.Any<string>(), Arg.Any<string>()).Returns(transactions);

        _mockMapper.Map<IEnumerable<TransactionDto>>(Arg.Any<IEnumerable<TransactionModel>>())
            .Returns(new List<TransactionDto>
            {
                new()
                {
                    Id = Guid.Parse(tRef)
                }
            });

        var result = await _paymentService.GetTransaction(merchantRef, tRef);

        result.ShouldNotBeNull();
        result.isSuccessful.ShouldBeTrue();
        var transactionDtos = result.content.ToList();
        result.content.Count().ShouldBe(transactionDtos.Count());
        transactionDtos[0].Id.ShouldBe(Guid.Parse(tRef));
    }

    [Test]
    public async Task MakePayments_WithSuccessfulPayment_ReturnsSuccessfulResponse()
    {
        // Arrange
        var content = new PaymentDto
        {
            Card = new CardDto(),
            Amount = 100,
            CurrencyId = 1,
            Merchant = new MerchantDto()
        };

        var simulatedResult = (true, "Payment successful");

        _mockCKOSimulator.ProcessPayment(Arg.Any<CardDto>(), Arg.Any<decimal>(), Arg.Any<int>(), Arg.Any<string>())
            .Returns(simulatedResult);

        _mockMapper.Map<TransactionDto>(Arg.Any<TransactionModel>())
            .Returns<TransactionDto>(input => new TransactionDto());

        var result = await _paymentService.MakePayments(content);

        result.ShouldNotBeNull();
        result.isSuccessful.ShouldBeTrue();
        result.response_message.ShouldBe("Success");
    }

    [Test]
    public async Task MakePayments_WithFailedPayment_ReturnsUnsuccessfulResponse()
    {
        var content = new PaymentDto
        {
            Card = new CardDto(),
            Amount = 100,
            CurrencyId = 1,
            Merchant = new MerchantDto()
        };

        var simulatedResult = (false, "Payment failed");

        _mockCKOSimulator.ProcessPayment(Arg.Any<CardDto>(), Arg.Any<decimal>(), Arg.Any<int>(), Arg.Any<string>())
            .Returns(simulatedResult);

        var result = await _paymentService.MakePayments(content);

        result.ShouldNotBeNull();
        result.isSuccessful.ShouldBeFalse();
        result.response_message.ShouldBe("Payment failed");
    }
}