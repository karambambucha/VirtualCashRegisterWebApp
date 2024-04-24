using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using VirtualCashRegisterWebApp.Enums;

namespace VirtualCashRegisterWebApp.TransactionContracts
{
    public class BasePaymentRequestContract : BaseRequestContract
    {
        /// <summary>
        /// Indicates electronic data capture (EDC) type.
        /// </summary>
        [Required(ErrorMessage = "Invalid payment type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PaymentType? PaymentType { get; set; }

        /// <summary>
        /// Alphanumeric SPIn transaction identifier. Has to be unique within one batch.
        /// </summary>
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string? ReferenceId { get; set; }

        /// <summary>
        /// Indicates if any of receipt copies should be printed after the transaction.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public virtual ReceiptType? PrintReceipt { get; set; }

        /// <summary>
        /// Indicates if any of receipt copies should be returned in response.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public virtual ReceiptType? GetReceipt { get; set; }

        /// <summary>
        /// Merchant number for multi-merchant environment. 
        /// If not present in multi-merchant environment, transaction will be cancelled.
        /// </summary>
        [Range(1, 5)]
        public int? MerchantNumber { get; set; }

        /// <summary>
        /// Unique alphanumeric invoice number.
        /// </summary>
        [StringLength(50)]
        public virtual string? InvoiceNumber { get; set; }

        /// <summary>
        /// Indicates whether customer signature should be captured or not in course of transaction.
        /// </summary>
        public virtual bool? CaptureSignature { get; set; }

        /// <summary>
        /// Indicates whether extended transaction data should be returned or not.
        /// </summary>
        public bool? GetExtendedData { get; set; }

        /// <summary>
        /// Information for callback with transaction result.
        /// </summary>
        public CallbackInfoContract? CallbackInfo { get; set; }
    }
}