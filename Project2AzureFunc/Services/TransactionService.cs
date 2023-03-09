using Project2AzureFunc.Data;
using Project2AzureFunc.Models;
using Project2AzureFunc.Models.Requests;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Project2AzureFunc.Services
{
    interface ITransactionService
    {
        Transaction CreateTransaction(TransactionRequest request);
        Task<Transaction> GetTransactionByID(Guid transactionID);
        IEnumerable<Transaction> GetTransactionsForAccount(int accountID);
    }
    internal class TransactionService : ITransactionService
    {
        private readonly IDataProvider dataProvider = new DataAccessHelper();
        private Transaction DataSetToTransaction(DataSet result)
        {
            if (result is null) return null;

            DataRow row = result.Tables[0].Rows[0];

            Transaction transaction = new()
            {
                TransactionID = Guid.Parse(row["TransactionID"].ToString()),
                Amount = float.Parse(row["Amount"].ToString()),
                TransactionOn = DateTime.Parse(row["TransactionOn"].ToString()),
                Direction = Enum.Parse<TransactionDirection>(row["Direction"].ToString(), true)
            };

            return transaction;
        }

        public Transaction CreateTransaction(TransactionRequest request)
        {
            DataSet result = dataProvider.HandleSQL($"INSERT INTO TransactionTable (TransactionID, Amount, Direction, TransactionOn) OUTPUT INSERTED.* VALUES ((NEWID(), {request.Amount}, '{request.Direction}', '{DateTime.UtcNow}')");
            return DataSetToTransaction(result);
        }

        public IEnumerable<Transaction> GetTransactionsForAccount(int accountID)
        {
            throw new NotImplementedException();
        }

        public async Task<Transaction> GetTransactionByID(Guid transactionID)
        {
            return DataSetToTransaction(await dataProvider.HandleSQLAsync($"SELECT * FROM TransactionTable WHERE TransactionID = '{transactionID}'"));
        }
    }
}
