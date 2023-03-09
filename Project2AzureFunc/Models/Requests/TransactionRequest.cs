namespace Project2AzureFunc.Models.Requests
{
    public enum TransactionDirection
    {
        CREDIT,
        DEBIT
    }

    public record TransactionRequest
    {
        public float Amount { get; set; }
        public TransactionDirection Direction { get; set; }
        public int AccountID { get; set; }
    }
}