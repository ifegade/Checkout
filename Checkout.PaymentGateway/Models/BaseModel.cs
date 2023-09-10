using System;
using System.ComponentModel.DataAnnotations;

namespace Checkout.PaymentGateway.Models;

public class BaseModel
{
    public BaseModel()
    {
        DateCreated = DateUpdated = DateTime.UtcNow;
    }

    [Key] public int Id { get; set; }
    public bool? MarkAsDeleted { get; set; }
    public DateTime DateCreated { get; set; }
    private DateTime _createdAt => DateCreated.ToLocalTime();
    public DateTime DateUpdated { get; set; }
    private DateTime _updatedAt => DateUpdated.ToLocalTime();
}