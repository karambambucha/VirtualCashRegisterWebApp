using System.ComponentModel;

namespace VirtualCashRegisterWebApp.Enums
{
    public enum StatusCode
    {
        [Description("Approved")]
        Approved = 0000,

        [Description("Terminal Busy")]
        TerminalBusy = 1000,

        [Description("Not Found")]
        NotFound = 1001,

        [Description("Not Implemented")]
        NotImplemented = 1002,

        [Description("Not Supported")]
        NotSupported = 1003,

        [Description("Not Allowed")]
        NotAllowed = 1004,

        [Description("Low Battery")]
        LowBattery = 1005,

        [Description("Internal Error")]
        InternalError = 1006,

        [Description("Format Error")]
        FormatError = 1007,

        [Description("Wrong Payment Or Transaction Type")]
        WrongPaymentOrTransactionType = 1008,

        [Description("Authentication Failed")]
        AuthenticationFailed = 1009,

        [Description("Missing Reference ID")]
        MissingReferenceId = 1010,

        [Description("Duplicate Reference ID")]
        DuplicateReferenceId = 1011,

        [Description("Canceled")]
        Canceled = 1012,

        [Description("Bad Request")]
        BadRequest = 1013,

        [Description("Declined")]
        Declined = 1015,

        [Description("Payment Type Mismatch")]
        PaymentTypeMismatch = 1016,

        [Description("Incorrect Merchant ID")]
        IncorrectMerchantId = 1017,

        [Description("No Debit Keys Loaded")]
        NoDebitKeysLoaded = 1019,

        [Description("No Open Batch")]
        NoOpenBatch = 1020,

        [Description("KMS Failed")]
        KmsFailed = 1024,

        [Description("Terminal Was Disconnected")]
        TerminalWasDisconnected = 1030,

        [Description("Signature Not Captured")]
        SignatureNotCaptured = 1500,

        [Description("Unknown Terminal Response")]
        UnknownTerminalResponse = 1999,

        [Description("Terminal not connected to SPIn Proxy server")]
        TerminalIsNotConnected = 2001,

        [Description("Active authKey not found")]
        ActiveAuthKeyNotFound = 2002,

        [Description("Register not found")]
        RegisterNotFound = 2003,

        [Description("Route not found")]
        RouteNotFound = 2004,

        [Description("Active route not found")]
        ActiveRouteNotFound = 2005,

        [Description("Not pars request")]
        NotParsRequest = 2006,

        [Description("The operation has timed out")]
        Timeout = 2007,

        [Description("Terminal in use")]
        TerminalInUse = 2008,

        [Description("Transaction not found")]
        TransactionNotFound = 2009,

        [Description("Communication error. Send request one more time")]
        CommunicationError = 2010,

        [Description("Terminal is not available")]
        TerminalIsNotAvailable = 2011,

        [Description("Callback Url was not specified")]
        AsyncCallbackUrlNotSpecified = 2101,

        [Description("Invalid XML document")]
        AsyncInvalidXml = 2102,

        [Description("Internal exception")]
        AsyncInternalException = 2110,

        [Description("Invalid request data")]
        InvalidRequestData = 2201,

        [Description("There is no processing request now")]
        NoProcessingRequest = 2301,
    }
}
