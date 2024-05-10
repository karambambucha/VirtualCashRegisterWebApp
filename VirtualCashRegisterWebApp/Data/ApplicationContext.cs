using Microsoft.EntityFrameworkCore;
using VirtualCashRegisterWebApp.TransactionContracts;
namespace VirtualCashRegisterWebApp.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<SaleResponse> SaleResponses { get; set; } = null!;
        public DbSet<ProductSaleResponse> ProductSaleResponse { get; set; } = null!;
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SaleResponse>()
                 .HasMany(e => e.Products)
                 .WithMany(e => e.SaleResponses)
                 .UsingEntity<ProductSaleResponse>();
        }
        public async void AddSaleResponse(SaleResponseContract saleResponseContract, List<ProductContract> productContract)
        {
            var products = new List<Product>();
            foreach (ProductContract product in productContract)
            {
                var prod = (from p in Products
                            where p.Id == product.Id
                            select p).ToList();
                products.Add(prod[0]);
            }
            var saleResponse = new SaleResponse()
            {
                ReferenceId = saleResponseContract.ReferenceId,
                TotalAmount = saleResponseContract.Amounts.TotalAmount,
                Amount = saleResponseContract.Amounts.Amount,
                TipAmount = saleResponseContract.Amounts.TipAmount,
                FeeAmount = saleResponseContract.Amounts.FeeAmount,
                PaymentType = saleResponseContract.PaymentType,
                AuthCode = saleResponseContract.AuthCode,
                CardType = saleResponseContract.CardData.CardType,
                EntryType = saleResponseContract.CardData.EntryType,
                Last4 = saleResponseContract.CardData.Last4,
                First4 = saleResponseContract.CardData.First4,
                BIN = saleResponseContract.CardData.BIN,
                CardName = saleResponseContract.CardData.Name,
                CustomerReceipt = saleResponseContract.Receipts.Customer,
                MerchantReceipt = saleResponseContract.Receipts.Merchant,
                Products = products
            };
            await SaleResponses.AddAsync(saleResponse);
            SaveChanges();
            foreach (Product product in products.Distinct())
            {
                var latestSaleResponse = SaleResponses.OrderBy(item => item.Id).Last();
                var productSaleResponse = ProductSaleResponse.Where(x => x.ProductsId == product.Id && x.SaleResponsesId == latestSaleResponse.Id).FirstOrDefault();
                productSaleResponse.ProductCount = products.Where(x => x == product).Count();
            }
            SaveChanges();
        }
        public async void AddSaleResponse(SaleResponseContract saleResponseContract)
        {
            var saleResponse = new SaleResponse()
            {
                ReferenceId = saleResponseContract.ReferenceId,
                TotalAmount = saleResponseContract.Amounts.TotalAmount,
                Amount = saleResponseContract.Amounts.Amount,
                TipAmount = saleResponseContract.Amounts.TipAmount,
                FeeAmount = saleResponseContract.Amounts.FeeAmount,
                PaymentType = saleResponseContract.PaymentType,
                AuthCode = saleResponseContract.AuthCode,
                CardType = saleResponseContract.CardData.CardType,
                EntryType = saleResponseContract.CardData.EntryType,
                Last4 = saleResponseContract.CardData.Last4,
                First4 = saleResponseContract.CardData.First4,
                BIN = saleResponseContract.CardData.BIN,
                CardName = saleResponseContract.CardData.Name,
                CustomerReceipt = saleResponseContract.Receipts.Customer,
                MerchantReceipt = saleResponseContract.Receipts.Merchant,
            };
            await SaleResponses.AddAsync(saleResponse);
            SaveChanges();
        }

        public async Task<Object> GetSaleResponse(int id)
        {
            var saleResponse = await SaleResponses
            .Include(c => c.Products).Select(x => new
            {
                id = x.Id,
                referenceId = x.ReferenceId,
                amount = x.Amount,
                authCode = x.AuthCode,
                bin = x.BIN,
                cardName = x.CardName,
                cardType = x.CardType,
                customerReceipt = x.CustomerReceipt,
                entryType = x.EntryType,
                feeAmount = x.FeeAmount,
                first4 = x.First4,
                last4 = x.Last4,
                merchantReceipt = x.MerchantReceipt,
                paymentType = x.PaymentType,
                taxAmount = x.TaxAmount,
                tipAmount = x.TipAmount,
                totalAmount = x.TotalAmount,
                products = x.Products.Join(ProductSaleResponse, product => product.Id, productSaleResponse => productSaleResponse.ProductsId, (product, productSaleResponse) => new { product.Name, product.Cost, productSaleResponse.ProductCount, productSaleResponse.SaleResponsesId }).Where(productSaleResponse => productSaleResponse.SaleResponsesId == id)
            })
            .FirstOrDefaultAsync(u => u.id == id);
            return saleResponse;
        }
    }

}
