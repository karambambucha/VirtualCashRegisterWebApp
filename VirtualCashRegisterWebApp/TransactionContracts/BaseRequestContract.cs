using System.ComponentModel.DataAnnotations;

namespace VirtualCashRegisterWebApp.TransactionContracts
{
    public class BaseRequestContract
    {
        /// <summary>
        /// Terminal profile number.
        /// Can be used to identify the terminal in SPIn Proxy environment. Required if no RegisterId.
        /// </summary>
        [StringLength(12, MinimumLength = 10)]
        public string Tpn { get; set; }

        /// <summary>
        /// Terminal identifier for register. [Obsolete]
        /// Can be used to identify the terminal instead of Tpn in SPIn Proxy environment. Required if no Tpn.
        /// </summary>
        [StringLength(50, MinimumLength = 2)]
        public string? RegisterId { get; set; }

        /// <summary>
        /// Merchant's authorization password. Required if no SPInToken.
        /// </summary>
        [StringLength(10, MinimumLength = 10)]
        public string Authkey { get; set; }

        /// <summary>
        /// Timeout for processing transaction with SPIn proxy. If null, the default timeout will be used. 
        /// </summary>
        [Range(1, 720, ErrorMessage = "Proxy timout must be set between 1 and 720 seconds")]
        public virtual int? SPInProxyTimeout { get; set; }

        /// <summary>
        /// A collection of custom fields in key-value format. 
        /// Numeric fields must contain a decimal point. For example: 
        ///"CustomFee": 1.0 or
        ///"CustomFee": 1.00 or
        ///"CustomFee": "1.00"
        /// </summary>
        public virtual Dictionary<string, string>? CustomFields { get; set; }
    }
}