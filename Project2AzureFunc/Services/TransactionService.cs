using Project2AzureFunc.Data;
using Project2AzureFunc.Models;
using Project2AzureFunc.Models.Requests;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Project2AzureFunc.Services
{
    interface ITransactionService
    {
        Transaction CreateTransaction(TransactionRequest request);
        Task<Transaction> GetTransactionByID(Guid transactionID);
    }
    internal class TransactionService : ITransactionService
    {
        private readonly IDataProvider dataProvider = new DataAccessHelper();
        private static Transaction DataSetToTransaction(DataSet result)
        {
            if (result is null) return null;

            DataRow row = result.Tables[0].Rows[0];

            Transaction transaction = new()
            {
                TransactionID = Guid.Parse(row["TransactionID"].ToString()),
                Amount = float.Parse(row["Amount"].ToString()),
                TransactionOn = DateTime.Parse(row["TransactionOn"].ToString()),
                Direction = Enum.Parse<TransactionDirection>(row["Direction"].ToString(), true),
                AccountID = int.Parse(row["AccountID"].ToString())
            };

            return transaction;
        }

        public Transaction CreateTransaction(TransactionRequest request)
        {
            /*
             The DEBIT is also wrapped into its DB-level consistency check
             */
            if (request.Direction.Equals(TransactionDirection.CREDIT))
            {
                DataSet result = dataProvider.HandleSQL(@$"
BEGIN
UPDATE WalletTable SET Balance = (Balance + {request.Amount}) WHERE AccountID = {request.AccountID};
INSERT INTO TransactionTable (TransactionID, Amount, Direction, TransactionOn, AccountID) OUTPUT INSERTED.* VALUES (NEWID(), {request.Amount}, '{request.Direction}', '{DateTime.UtcNow}', {request.AccountID})
END");
                return DataSetToTransaction(result);
            }
            else if (request.Direction.Equals(TransactionDirection.DEBIT))
            {
                DataSet result = dataProvider.HandleSQL(@$"
DECLARE @BAL FLOAT;
SET @BAL = (SELECT Balance FROM WalletTable WHERE AccountID = {request.AccountID});
IF (@BAL - {request.Amount} >= 0)
BEGIN
UPDATE WalletTable SET Balance = (Balance - {request.Amount}) WHERE AccountID = {request.AccountID};
INSERT INTO TransactionTable (TransactionID, Amount, Direction, TransactionOn, AccountID) OUTPUT INSERTED.* VALUES (NEWID(), {request.Amount}, '{request.Direction}', '{DateTime.UtcNow}', {request.AccountID})
END");
                return DataSetToTransaction(result);
            }
            else return null;
        }

        public async Task<Transaction> GetTransactionByID(Guid transactionID)
        {
            return DataSetToTransaction(await dataProvider.HandleSQLAsync($"SELECT * FROM TransactionTable WHERE TransactionID = '{transactionID}'"));
        }
    }
}
