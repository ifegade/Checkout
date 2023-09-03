using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Models;

namespace Checkout.PaymentGateway.Infrastructure;

public interface IMerchantService
{
    Task<bool> IsMerchantValid(string merchantId);
}

public class MerchantService : IMerchantService
{
    private IList<MerchantModel> _merchants;

    public MerchantService()
    {
        _merchants = new List<MerchantModel>()
        {
            new("1", "Amazon"),
            new("2", "Apple")
        };
    }

    /* A valid Merchant is a merchant that exist on the system
     * and has the right authorization & permission to make receive transaction
     * Here only the ID is being checked
     */
    public async Task<bool> IsMerchantValid(string merchantId)
    {
        return _merchants.Any(s => s.Id == merchantId);
    }
}