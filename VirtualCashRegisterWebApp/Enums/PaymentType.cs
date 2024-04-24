using System.Text.Json.Serialization;

namespace VirtualCashRegisterWebApp.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PaymentType
    {
        Credit,
        Debit,
        EBT_Food,
        EBT_Cash,
        Card,
        Cash,
        Check,
        Gift
    }
}