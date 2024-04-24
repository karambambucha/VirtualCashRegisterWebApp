using System.ComponentModel;

namespace VirtualCashRegisterWebApp.Enums
{
    public enum UserInputTypes
    {
        [Description("n")]
        Number,
        [Description("a")]
        Letters,
        [Description("an")]
        NumberAndLetters,
        [Description("$n")]
        Currency,
        [Description("i")]
        InfoOnly
    }
}