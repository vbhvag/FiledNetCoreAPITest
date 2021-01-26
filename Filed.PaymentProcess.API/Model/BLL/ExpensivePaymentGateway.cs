﻿using System.Threading.Tasks;

namespace Filed.PaymentProcess.API.Model.BLL
{
    public class ExpensivePaymentGateway : IExpensivePaymentGateway
    {
        public async Task<PaymentStatusType> MakePayment(PayDetails payDetails)
        {
            return PaymentStatusType.Processed;
        }
    }
}