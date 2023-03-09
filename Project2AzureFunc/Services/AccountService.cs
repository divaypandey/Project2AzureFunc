using Project2AzureFunc.Data;
using Project2AzureFunc.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2AzureFunc.Services
{
    interface IAccountService
    {
        Task<WalletAccount> GetAccountByID(int accountID);
    }
    internal class AccountService : IAccountService
    {
        private readonly IDataProvider dataProvider = new DataAccessHelper();

        private WalletAccount DataSetToWalletAccount(DataSet result)
        {
            throw new NotImplementedException();
        }

        public async Task<WalletAccount> GetAccountByID(int accountID)
        {
            var result = await dataProvider.HandleSQLAsync($"SELECT * FROM WalletTable WHERE AccountID = {accountID}");
            if(result == null && result.Tables.Count > 0)
            {

            }
            throw new NotImplementedException();
        }
    }
}
