using System.ComponentModel;

namespace VirtualCashRegisterWebApp.Enums
{
    public enum EntryType
    {
        None,

        [Description("chip")]
        Chip,

        [Description("swipe")]
        Swipe,

        [Description("chipcontactless")]
        ChipContactless,

        [Description("contactless")]
        Contactless,

        [Description("manual")]
        Manual,

        Unknown
    }
}