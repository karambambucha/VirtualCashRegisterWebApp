namespace VirtualCashRegisterWebApp.TransactionContracts
{
    public class SettleDetailsContract
    {
        public string Application { get; set; }
        public string DetailedMessage { get; set; }
        public string HostStatus { get; set; }
        public SettleTransactionReportContract TransactionsReports { get; set; }
        public string Receipt { get; set; }
    }
}