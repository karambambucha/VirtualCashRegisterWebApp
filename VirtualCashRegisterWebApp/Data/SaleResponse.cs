namespace VirtualCashRegisterWebApp.Data
{
    public class SaleResponse
    {
        public int Id { get; set; }
        public string ReferenceId { get; set; } = "";
        public double TotalAmount { get; set; }
        public double Amount { get; set; }
        public double TipAmount { get; set; }
        public double FeeAmount { get; set; }
        public double TaxAmount { get; set; }
        public string? PaymentType { get; set; }
        public string? AuthCode { get; set; }
        public string? CardType { get; set; }
        public string? EntryType { get; set; }
        public int Last4 { get; set; }
        public int First4 { get; set; }
        public int BIN { get; set; }
        public string? CardName { get; set; }
        public string? CustomerReceipt { get; set; }
        public string? MerchantReceipt { get; set; }
        public List<Product>? Products { get; set; }
    }
}
