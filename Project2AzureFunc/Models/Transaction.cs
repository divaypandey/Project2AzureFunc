using Project2AzureFunc.Models.Requests;
using System;

namespace Project2AzureFunc.Models
{
    public record Transaction
    {
        public Guid TransactionID { get; set; }
        public float Amount { get; set; }
        public TransactionDirection Direction { get; set; }

        //in UTC
        public DateTime TransactionOn { get; set; }

        public int AccountID { get; set; }
    }
}
