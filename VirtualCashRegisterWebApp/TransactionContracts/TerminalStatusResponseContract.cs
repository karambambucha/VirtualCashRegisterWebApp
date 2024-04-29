namespace VirtualCashRegisterWebApp.TransactionContracts
{
    public class TerminalStatusResponseContract
    {
        /// <summary>
        /// Terminal status - Online, Offline, or Not Found
        /// </summary>
        public string TerminalStatus { get; set; }

        /// <summary>
        /// Terminal profile number.
        /// </summary>
        public string Tpn { get; set; }

        /// <summary>
        /// Description of validations error for invalid request 
        /// </summary>
        public string ErrorDescription { get; set; }
    }
}