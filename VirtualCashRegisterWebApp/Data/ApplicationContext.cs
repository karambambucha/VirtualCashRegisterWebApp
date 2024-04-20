using Microsoft.EntityFrameworkCore;
namespace VirtualCashRegisterWebApp.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<SaleResponse> SaleResponses { get; set; } = null!;
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }

}
