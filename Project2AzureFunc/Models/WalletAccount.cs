using System;

namespace Project2AzureFunc.Models
{
    public record WalletAccount
    {
        public int AccountID { get; set; }
        public float Balance { get; set; }

        //in UTC
        public DateTime CreatedOn { get; set; }

        //in UTC
        public DateTime LastTransactionOn { get; set; }
    }
}
