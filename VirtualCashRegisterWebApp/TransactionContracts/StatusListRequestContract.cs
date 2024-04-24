using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using VirtualCashRegisterWebApp.Enums;

namespace VirtualCashRegisterWebApp.TransactionContracts
{
    public class StatusListRequestContract : BaseRequestContract
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PaymentType? PaymentType { get; set; }
        [Required]
        [Range(1, 5000, ErrorMessage = "Index must be between 1 and 5000")]
        public int TransactionFromIndex { get; set; }
        [Required]
        [Range(1, 5000)]
        public int TransactionToIndex { get; set; }
    }
}