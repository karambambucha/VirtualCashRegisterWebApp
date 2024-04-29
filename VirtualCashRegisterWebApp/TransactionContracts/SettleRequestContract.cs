using System.Text.Json.Serialization;
using VirtualCashRegisterWebApp.Enums;

namespace VirtualCashRegisterWebApp.TransactionContracts
{
    public class SettleRequestContract : BaseRequestContract
    {
        public string ReferenceId { get; set; }
        public bool? GetReceipt { get; set; }

        /// <summary>
        /// The field indicating the type of settlement processing. Standard close, remove all batch data, or settle without validation.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SettlementType? SettlementType { get; set; }
    }
}