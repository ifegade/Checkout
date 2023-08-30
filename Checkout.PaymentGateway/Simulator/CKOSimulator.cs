using System;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Dto;
using Checkout.PaymentGateway.Utils;

namespace Checkout.PaymentGateway.Simulator;

public  class CKOSimulator
{
    readonly Random rnd;

    public CKOSimulator()
    {
        rnd = new Random();
    }

    private async Task<bool> ValidCardDetails(CardDto cardDetails)
    {
        return rnd.Next(1, 10) % 2 == 0;
    }

    private async Task<bool> CanTransactionBeMade(CardDto cardDto, decimal amount)
    {
        //Does the user have the right amount
        //Is the user within the limit for the day
        //Is the Merchant Platform authorised by the user e.t.c
        return rnd.Next(1, 10) % 2 == 0;
    }

    public async  Task<(bool status, string message)> ProcessPayment(CardDto cardDetails, decimal amount, int currency, string merchant)
    {
        var isCardValid = await ValidCardDetails(cardDetails);
        if (!isCardValid)
            return (false, MessageStrings.InvalidCardDetails);
        
        var isBalanceSufficient = await CanTransactionBeMade(cardDetails, amount);
        if (!isBalanceSufficient)
            return (false, MessageStrings.InsufficientFunds);
        
        return (false, MessageStrings.UnTransactionSuccessful);
    }
}