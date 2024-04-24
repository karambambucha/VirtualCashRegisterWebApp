using System.Text.Json.Serialization;

namespace VirtualCashRegisterWebApp.TransactionContracts
{
    public class TerminalStatusRequestContract : BaseRequestContract
    {
        [JsonIgnore]
        public override int? SPInProxyTimeout { get; set; }

        [JsonIgnore]
        public override Dictionary<string, string> CustomFields { get; set; }
    }
}