namespace VirtualCashRegisterWebApp.Data
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public double Cost { get; set; }
        public List<SaleResponse>? SaleResponses { get; set; }
    }
}
