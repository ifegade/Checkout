using System;
using System.Text.Json.Serialization;

namespace Checkout.PaymentGateway.Dto;

public class CardDto : CardBaseDTO
{
    public int Year { get; set; }
    public string CVV { get; set; }
    public int Month { get; set; }
}

public class CardBaseDTO
{
    public string CardNumber { get; set; }
    public string CardName { get; set; }
}