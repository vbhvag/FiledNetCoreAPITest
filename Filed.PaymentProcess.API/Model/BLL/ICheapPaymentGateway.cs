using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Filed.PaymentProcess.API.Model.BLL
{
    public interface ICheapPaymentGateway
    {
        Task<PaymentStatusType> MakePayment(PayDetails payDetails);
    }
}