using System.Collections.Generic;

namespace Checkout.PaymentGateway.Dto;

public class IpDto
{
    public string Name { get; set; }
    public List<string> Ips { get; set; }
}