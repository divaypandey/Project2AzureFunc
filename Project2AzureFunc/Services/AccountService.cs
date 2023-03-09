using Project2AzureFunc.Data;
using Project2AzureFunc.Models;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Project2AzureFunc.Services
{
    interface IAccountService
    {
        Task<WalletAccount> GetAccountByID(int accountID);
        Task<WalletAccount> CreateAccount(int accountID);
    }
    internal class AccountService : IAccountService
    {
        private readonly IDataProvider dataProvider = new DataAccessHelper();

        private WalletAccount DataSetToWalletAccount(DataSet result)
        {
            if (result is null) return null;

            DataRow row = result.Tables[0].Rows[0];

            WalletAccount account = new()
            {
                AccountID = int.Parse(row["AccountID"].ToString()),
                Balance = float.Parse(row["Balance"].ToString()),
                CreatedOn = DateTime.Parse(row["CreatedOn"].ToString())
            };
            if (!row.IsNull("LastTransactionOn")) account.LastTransactionOn = DateTime.Parse(row["LastTransactionOn"].ToString());

            return account;
        }

        public async Task<WalletAccount> GetAccountByID(int accountID)
        {
            return DataSetToWalletAccount(await dataProvider.HandleSQLAsync($"SELECT * FROM WalletTable WHERE AccountID = {accountID}"));
        }

        public async Task<WalletAccount> CreateAccount(int accountID)
        {
            WalletAccount account = await GetAccountByID(accountID);
            if (account is not null) return account;
            else
            {
                _ = dataProvider.HandleSQL($"INSERT INTO WalletTable (AccountID, CreatedOn) VALUES ({account}, '{DateTime.UtcNow}')");
                return await GetAccountByID(accountID);
            }
        }
    }
}
