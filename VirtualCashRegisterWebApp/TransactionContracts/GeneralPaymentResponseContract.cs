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
    }
}
