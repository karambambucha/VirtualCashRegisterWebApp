namespace VirtualCashRegisterWebApp.TransactionContracts
{
    public class GeneralPaymentResponseContract : GeneralResponseContract
    {
        /// <summary>
        /// This response code comes from the payment processor as is. It is usually referring to ISO 8583-1987.
        /// </summary>
        public string HostResponseCode { get; set; }

        /// <summary>
        /// Meanings for host response code that comes from the payment processor as is.  It is usually referring to ISO 8583-1987.
        /// </summary>
        public string HostResponseMessage { get; set; }

        public void AddRequiredFieldsIfNeeded()
        {
            if (StatusCode != null
                && ResultCode != null
                && !string.IsNullOrWhiteSpace(Message)
                && !string.IsNullOrWhiteSpace(DetailedMessage))
                return;

            if (string.IsNullOrEmpty(DetailedMessage))
                DetailedMessage = Message;
            var messageForSearch = DetailedMessage?.ToLower().TrimEnd('.');

            if (StatusCode == null && ResultCode == Enums.ResultCode.TerminalError)
                StatusCode = Enums.StatusCode.UnknownTerminalResponse;

            if (ResultCode == null)
            {
                ResultCode = StatusCode == Enums.StatusCode.Approved ? Enums.ResultCode.Ok : Enums.ResultCode.TerminalError;
            }
        }
    }
}
