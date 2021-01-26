using Microsoft.EntityFrameworkCore;
using Filed.PaymentProcess.API.Model;

namespace Filed.PaymentProcess.API.Data.EFCore
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options)
            : base(options)
        {
        }

        public DbSet<PayDetails> PayDetails { get; set; }
        public DbSet<PaymentStatus> PaymentStatus { get; set; }
    }
}