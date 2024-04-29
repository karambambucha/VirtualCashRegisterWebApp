namespace VirtualCashRegisterWebApp.TransactionContracts
{
    public class CardDataContract
    {
        public string? CardType { get; set; }
        public string? EntryType { get; set; }
        public string? Last4 { get; set; }
        public string? First4 { get; set; }
        public string? BIN { get; set; }
        public string? Name { get; set; }
    }
}
