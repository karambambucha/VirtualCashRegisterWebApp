namespace VirtualCashRegisterWebApp.TransactionContracts
{
    public class EMVDataContract
    {
        /// <summary>
        /// Application name.
        /// </summary>

        public string? ApplicationName { get; set; }

        /// <summary>
        /// Application Identifier. A data label that differentiates payment systems and products.
        /// </summary>
        public string? AID { get; set; }

        /// <summary>
        /// Terminal Verification Results. The result of the checks performed by the terminal during the transaction.
        /// </summary>
        public string? TVR { get; set; }

        /// <summary>
        /// Transaction Status Information.
        /// </summary>
        public string? TSI { get; set; }

        /// <summary>
        /// Issuer Application Data.
        /// </summary>
        public string? IAD { get; set; }

        /// <summary>
        /// Authorization Response Code.
        /// </summary>
        public string? ARC { get; set; }
    }
}