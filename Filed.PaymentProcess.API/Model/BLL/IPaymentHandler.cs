using System.Threading.Tasks;

namespace Filed.PaymentProcess.API.Model.BLL
{
    public interface IPaymentHandler
    {
        Task<string> DoPayment(PayDetails payDetails);
    }
}