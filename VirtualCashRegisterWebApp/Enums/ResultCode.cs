namespace VirtualCashRegisterWebApp.Enums
{
    /// <summary>
    /// General result code [0 - Success result, 1 - Error on terminal, 2 - Error on SPIn proxy side] 
    /// </summary>
    public enum ResultCode
    {
        Ok = 0,
        TerminalError = 1,
        ApiError = 2
    }
}
