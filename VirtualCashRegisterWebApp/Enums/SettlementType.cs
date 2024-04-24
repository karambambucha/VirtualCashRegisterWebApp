using System.Text.Json.Serialization;

namespace VirtualCashRegisterWebApp.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SettlementType
    {
        Close,
        Clear,
        Force
    }
}