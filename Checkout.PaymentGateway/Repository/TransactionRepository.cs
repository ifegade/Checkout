using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Models;

namespace Checkout.PaymentGateway.Repository;

public interface ITransactionRepo
{
    Task<IQueryable<TransactionModel>> GetTransaction(string merchantRef, string tRef);
    Task<TransactionModel> Add(TransactionModel tModel);
}

public class TransactionRepository : ITransactionRepo
{
    private readonly IList<TransactionModel> _transactions;

    public TransactionRepository()
    {
        _transactions = new List<TransactionModel>();
    }

    public async Task<IQueryable<TransactionModel>> GetTransaction(string merchantRef, string tRef)
    {
        return _transactions.AsQueryable();
    }

    public async Task<TransactionModel> Add(TransactionModel tModel)
    {
        await Task.Delay(500);
        _transactions.Add(tModel);
        return tModel;
    }
}