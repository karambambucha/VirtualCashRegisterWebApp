namespace VirtualCashRegisterWebApp.TransactionContracts
{
    public class TransactionSummaryReportContract
    {
        public int? TransactionsCount { get; set; }
        public decimal? SaleAmount { get; set; }
        public decimal? ReturnAmount { get; set; }
        public decimal? VoidAmount { get; set; }
        public decimal? AuthAmount { get; set; }
        public decimal? TotalAmount { get; set; }
    }
}