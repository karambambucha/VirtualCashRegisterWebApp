namespace VirtualCashRegisterWebApp.TransactionContracts
{
    public class SettleResponseContract : BaseResponseContract
    {
        public List<SettleDetailsContract> SettleDetails { get; set; }
    }

    public class SettleTransactionReportContract : TransactionSummaryReportContract
    {
    }

}