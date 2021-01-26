using System.Threading.Tasks;

namespace Filed.PaymentProcess.API.Model.BLL
{
    public class CheapPaymentGateway : ICheapPaymentGateway
    {
        public async Task<PaymentStatusType> MakePayment(PayDetails payDetails)
        {
            return PaymentStatusType.Processed;
        }
    }
}