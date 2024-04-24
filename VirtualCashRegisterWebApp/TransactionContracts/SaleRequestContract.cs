using System.ComponentModel.DataAnnotations;

namespace VirtualCashRegisterWebApp.TransactionContracts
{
    public class SaleRequestContract : BasePaymentRequestContract
    {
        /// <summary>
        /// Total amount of the transaction.
        /// </summary>
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "The Amount field is required and must be a positive value greater than zero.")]
        public decimal? Amount { get; set; }

        /// <summary>
        /// Tip amount of the transaction.
        /// </summary>
        [Range(0.00, double.MaxValue, ErrorMessage = "The TipAmount field must be a non-negative value greater or equal than zero.")]
        public decimal? TipAmount { get; set; }

        /// <summary>
        /// Text in receipt format that terminal prints as a part of terminal receipt
        /// </summary>
        public string? ExternalReceipt { get; set; }
        public List<ProductContract>? Products { get; set; }
    }
}