namespace VirtualCashRegisterWebApp.TransactionContracts
{
    public class PaymentAmountsContract
    {
        /// <summary>
        /// Amount with fee and tip.
        /// </summary>
        public decimal? TotalAmount { get; set; }

        /// <summary>
        /// Amount with tip.
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// Tip amount of the transaction.
        /// </summary>
        public decimal? TipAmount { get; set; }

        /// <summary>
        /// Fee amount of the transaction.
        /// </summary>
        public decimal? FeeAmount { get; set; }

        /// <summary>
        /// Tax amount of the transaction.
        /// </summary>
        public decimal? TaxAmount { get; set; }
    }
}