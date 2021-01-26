using Filed.PaymentProcess.API.Model;

namespace Filed.PaymentProcess.API.Data.EFCore
{
    public class PaymentStatusRepository : EFCoreRepository<PaymentStatus, PaymentDbContext>
    {
        public PaymentStatusRepository(PaymentDbContext context) : base(context)
        {

        }
    }
}