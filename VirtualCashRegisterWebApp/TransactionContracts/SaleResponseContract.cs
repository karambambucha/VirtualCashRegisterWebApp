namespace VirtualCashRegisterWebApp.TransactionContracts
{
    public class SaleResponseContract : BaseResponseContract
    {
        /// <summary>
        /// Receipts  
        /// </summary>
        public ReceiptContract? Receipts { get; set; }
        /// <summary>
        /// Amounts of money 
        /// </summary>
        public AmountsContract? Amounts { get; set; }

        public string? PaymentType { get; set; }
        public string? TransactionType { get; set; }
        public string? AuthCode { get; set; }
        public string? ReferenceId { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? BatchNumber { get; set; }
        public CardDataContract? CardData { get; set; }

    }
}
