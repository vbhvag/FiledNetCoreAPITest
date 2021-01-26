using Filed.PaymentProcess.API.Model;

namespace Filed.PaymentProcess.API.Data.EFCore
{
    public class PayDetailRepository : EFCoreRepository<PayDetails, PaymentDbContext>
    {
        public PayDetailRepository(PaymentDbContext context) : base(context)
        {

        }
    }
}