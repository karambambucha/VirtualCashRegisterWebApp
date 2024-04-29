using System.Text.Json.Serialization;
using VirtualCashRegisterWebApp.Enums;

namespace VirtualCashRegisterWebApp.TransactionContracts
{
    public class BasePaymentResponseContract : BaseResponseContract
    {
        /// <summary>
        /// General response for payment request
        /// </summary>
        public GeneralPaymentResponseContract GeneralResponse { get; set; }

        /// <summary>
        /// Indicates electronic data capture  ['Credit', 'Debit', 'EBT', 'Card', 'Cash', 'Check', 'Gift'].
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PaymentType? PaymentType { get; set; }

        /// <summary>
        /// Indicates Transaction Type (Sale, Void, Auth, etc).
        /// </summary>
        public string TransactionType { get; set; }

        public PaymentAmountsContract Amounts { get; set; }

        /// <summary>
        /// Authorization code provided by payment processor.
        /// </summary>
        public string AuthCode { get; set; }

        /// <summary>
        /// Alphanumeric unique SPin transaction identifier.
        /// </summary>
        public string ReferenceId { get; set; }

        /// <summary>
        /// Unique invoice number.
        /// </summary>
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// Device Serial Number.
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// Current batch number.
        /// </summary>
        public string BatchNumber { get; set; }

        /// <summary>
        /// Transaction number within batch.
        /// </summary>
        public string TransactionNumber { get; set; }

        /// <summary>
        /// Indicates whether transaction was voided or not.
        /// </summary>
        public bool? Voided { get; set; }

        /// <summary>
        /// Indicates customer signature if it was captured.
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// Using the token obtained in sale response POS can perform new authorisation using "Transact"”" api of iPosPays. This sale request will not require any card interaction at the payment device.
        /// </summary>
        public string IPosToken { get; set; }

        /// <summary>
        /// This token could be required for future transactions using the same card
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Reference Retrieval Number provided by iPOSPay Gateway. Uses for iPos gateway Api.
        /// </summary>
        public string RRN { get; set; }

        public Dictionary<string, Dictionary<string, string>> ExtendedDataByApplication { get; set; }

        public CardDataContract CardData { get; set; }

        public ReceiptContract Receipts { get; set; }
    }
}