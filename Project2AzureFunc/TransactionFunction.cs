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

namespace Project2AzureFunc
{
    public static class TransactionFunction
    {
        private static IAccountService accountService = new AccountService();
        private static ITransactionService transactionService = new TransactionService();

        [FunctionName("Transaction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            TransactionRequest request = JsonConvert.DeserializeObject<TransactionRequest>(requestBody);
            WalletAccount account = await accountService.GetAccountByID(request.Account);

            if (account == null) return new NotFoundObjectResult($"Account with ID: '{request.Account}' not found!");
            else
            {
                if ((request.Direction.Equals(TransactionDirection.DEBIT) && (account.Balance - request.Amount > 0)) || request.Direction.Equals(TransactionDirection.CREDIT))
                {
                    Transaction transaction = transactionService.CreateTransaction(request);
                    if (transaction is not null) return new OkObjectResult(transaction);
                    else return new BadRequestObjectResult("Something went wrong while trying to process this request :(");
                }
                else return new BadRequestObjectResult($"Account with ID: '{account.AccountID}' does not have sufficient funds to debit '{request.Amount}'");
            }
        }

        /*
         wallet table:
        acc id
        acc balance
        other rev details

        trans table:
        tr id
        amount
        direction
        acc id
        timestamp

         
         */
    }
}
