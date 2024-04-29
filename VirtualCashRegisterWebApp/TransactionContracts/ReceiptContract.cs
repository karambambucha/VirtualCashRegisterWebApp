namespace VirtualCashRegisterWebApp.TransactionContracts
{
    public class ReceiptContract
    {
        /// <summary>
        /// Merchant receipt copy.
        /// </summary>
        public string Customer { get; set; }

        /// <summary>
        /// Customer receipt copy.
        /// </summary>
        public string Merchant { get; set; }
    }
}