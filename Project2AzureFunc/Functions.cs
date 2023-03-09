using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Project2AzureFunc.Models.Requests;
using Project2AzureFunc.Models;
using Project2AzureFunc.Services;
using System;

namespace Project2AzureFunc
{
    public static class Functions
    {
        private static readonly IAccountService accountService = new AccountService();
        private static readonly ITransactionService transactionService = new TransactionService();

        [FunctionName("Transaction")]
        public static async Task<IActionResult> RunTransaction(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function for new transaction hit");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            
            try
            {
                TransactionRequest request = JsonConvert.DeserializeObject<TransactionRequest>(requestBody);

                if (request.Amount < 0) return new BadRequestObjectResult($"Amount cannot be negative, you entered '{request.Amount}'");

                WalletAccount account = await accountService.GetAccountByID(request.AccountID);
                if (account == null) return new NotFoundObjectResult($"Account with ID: '{request.AccountID}' not found!");
                else
                {
                    if ((request.Direction.Equals(TransactionDirection.DEBIT) && (account.Balance - request.Amount >= 0)) || request.Direction.Equals(TransactionDirection.CREDIT))
                    {
                        Transaction transaction = transactionService.CreateTransaction(request);
                        if (transaction is not null)
                        {
                            accountService.UpdateLastTransactionDateToNow(request.AccountID);
                            return new OkObjectResult(transaction);
                        }
                        else return new BadRequestObjectResult("Something went wrong while trying to process this request :(");
                    }
                    else return new BadRequestObjectResult($"Account with ID: '{account.AccountID}' does not have sufficient funds to debit '{request.Amount}'");
                }
            }
            catch
            {
                return new BadRequestObjectResult("Something went wrong while trying to process this request :(");
            }
        }

        [FunctionName("Account")]
        public static async Task<IActionResult> RunAccount(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function for Account hit");

            string query = req.Query["accountID"];
            try
            {
                int accountID = int.Parse(query);
                WalletAccount account = await accountService.GetAccountByID(accountID);
                if (account is not null) return new OkObjectResult($"Account already exists with ID: '{account.AccountID}'");
                else
                {
                    account = await accountService.CreateAccount(accountID);
                    if (account is not null) return new OkObjectResult($"Account created with ID: '{account.AccountID}'");
                    else return new BadRequestObjectResult("Something went wrong while trying to process this request :(");
                }
            }
            catch(Exception ex)
            {
                if (ex is FormatException) return new BadRequestObjectResult($"Please enter a valid account, '{query}' isnt a valid Account format.");
                else return new BadRequestObjectResult("Something went wrong while trying to process this request :(");
            }
        }
    }
}
