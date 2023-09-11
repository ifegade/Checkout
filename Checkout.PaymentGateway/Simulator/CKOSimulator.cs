using System;
using System.Net.Http;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Dto;
using Checkout.PaymentGateway.Utils;

namespace Checkout.PaymentGateway.Simulator;

public interface ICkoSimulator
{
    Task<(bool status, string message)> ProcessPayment(CardDto cardDetails, decimal amount, int currency,
        string merchant);
}

public class CKOSimulator : ICkoSimulator
{
    private readonly HttpClient _client;
    private readonly Random rnd;

    public CKOSimulator()
    {
    }

    public CKOSimulator(HttpClient client)
    {
        rnd = new Random();
        _client = client;
    }

    public async Task<(bool status, string message)> ProcessPayment(CardDto cardDetails, decimal amount, int currency,
        string merchant)
    {
        var isCardValid = ValidCardDetails(cardDetails);
        var isBalanceSufficient = CanTransactionBeMade(cardDetails, amount);

        await Task.WhenAll(isCardValid, isBalanceSufficient);

        if (!isCardValid.Result)
            return (false, MessageStrings.InvalidCardDetails);

        if (!isBalanceSufficient.Result)
            return (false, MessageStrings.InsufficientFunds);

        var paymentResult = await DeductPayment(cardDetails, amount, currency);

        if (paymentResult)
            return (paymentResult, MessageStrings.Successful);

        return (false, MessageStrings.UnTransactionSuccessful);
    }

    private async Task<bool> ValidCardDetails(CardDto cardDetails)
    {
        await Task.Delay(500);
        return true; //rnd.Next(1, 10) % 2 == 0;
    }

    private async Task<bool> CanTransactionBeMade(CardDto cardDto, decimal amount)
    {
        //Does the user have the right amount
        //Is the user within the limit for the day
        //Is the Merchant Platform authorised by the user e.t.c
        //All these validations are API calls within the banking system to validate a payment
        await Task.Delay(500);
        return true; //rnd.Next(1, 10) % 2 == 0;
    }

    private async Task<bool> DeductPayment(CardDto cardDetails, decimal amount, int currency)
    {
        await Task.Delay(500);
        return true; //rnd.Next(1, 10) % 2 == 0;
    }
}