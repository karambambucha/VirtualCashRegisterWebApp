using System.Text.Json.Serialization;

namespace VirtualCashRegisterWebApp.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TransactionType
    {
        Sale,
        Redeem,
        Void,
        Return,
        TipAdjust,
        Auth,
        Capture,
        Status,
        Get,
        Settle,
        Printer,
        UserChoice,
        UserInput,
        Select,
        Pay,
        Report,
        AbortTransaction,
        Activate,
        Deactivate,
        Inquire,
        Refund,
        Reissue,
        Reload
    }


}