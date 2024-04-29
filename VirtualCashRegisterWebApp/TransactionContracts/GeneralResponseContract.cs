using System.Text.Json.Serialization;
using VirtualCashRegisterWebApp.Enums;

namespace VirtualCashRegisterWebApp.TransactionContracts
{
    public class GeneralResponseContract
    {
        /// <summary>
        /// General result code [0 - Success result, 1 - Error on terminal, 2 - Error on SPIn proxy side] 
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ResultCode? ResultCode { get; set; }

        /// <summary>
        /// Indicates 4-digit response code for specific situation.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public StatusCode? StatusCode { get; set; }

        /// <summary>
        /// Text message that describes response.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// More detailed message that describes response.
        /// </summary>
        public string DetailedMessage { get; set; }

        /// <summary>
        /// Max delay before next request, if terminal is busy
        /// </summary>
        public double? DelayBeforeNextRequest { get; set; }
    }
}