namespace VirtualCashRegisterWebApp.TransactionContracts
{
    public class StatusListResponseContract : BaseResponseContract
    {
        public List<BasePaymentResponseContract> Transactions { get; set; }
    }
}