using System;
using Microsoft.Extensions.Hosting;

namespace Checkout.PaymentGateway.Utils;

public class Utility
{
    public static string GetEnvironment()
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Development)
            return "Development";
        return "Production";
    }

    public static string MaskCardNumber(string cardNumber)
    {
        int i = 0;
        var number = cardNumber.ToCharArray();
        while (i < cardNumber.Length)
        {
            if (i > 3 && i < cardNumber.Length - 3)
                number[i] = '*';

            i += 1;
        }

        return new string(number);
    }
}