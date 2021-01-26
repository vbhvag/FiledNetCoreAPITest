using Filed.PaymentProcess.API.Data;
using System.Threading.Tasks;

namespace Filed.PaymentProcess.API.Model.BLL
{
    public class PaymentHandler : IPaymentHandler
    {
        private readonly IRepository<PayDetails> _payRepository;
        private readonly IRepository<PaymentStatus> _payStatusRepository;
        private readonly ICheapPaymentGateway _cheapPaymentGateway;
        private readonly IExpensivePaymentGateway _expensivePaymentGateway;
        private readonly IPremiumPaymentGateway _premiumPaymentGateway;
        public PaymentHandler(IRepository<PayDetails> payRepository, IRepository<PaymentStatus> payStatusRepository
            , ICheapPaymentGateway cheapPaymentGateway, IExpensivePaymentGateway expensivePaymentGateway, IPremiumPaymentGateway premiumPaymentGateway)
        {
            _payRepository = payRepository;
            _payStatusRepository = payStatusRepository;
            _cheapPaymentGateway = cheapPaymentGateway;
            _expensivePaymentGateway = expensivePaymentGateway;
            _premiumPaymentGateway = premiumPaymentGateway;
        }
        public async Task<string> DoPayment(PayDetails payDetails)
        {
            var payStatus = PaymentStatusType.Pending;
            if (payDetails.Amount <= 20)
            {
                payStatus =  await CheapPayment(payDetails);
            }
            else if (payDetails.Amount > 20 && payDetails.Amount <= 500)
            {
                payStatus =  await ExpensivePayment(payDetails);
            }
            else
            {
                payStatus =  await PremiumPayment(payDetails);
            }

            var payment = await _payRepository.Add(payDetails);
            var paymentStatus = new PaymentStatus {
                PayDetailId = payment.Id,
                Status = payStatus.ToString()
            };
            var status = await _payStatusRepository.Add(paymentStatus);
            return status.Status;
        }

        private async Task<PaymentStatusType> CheapPayment(PayDetails payDetails)
        {
            return await _cheapPaymentGateway.MakePayment(payDetails);
        }

        private async Task<PaymentStatusType> ExpensivePayment(PayDetails payDetails)
        {
            var payStatus = await _expensivePaymentGateway.MakePayment(payDetails);
            if (payStatus == PaymentStatusType.Failed)
                payStatus = await _cheapPaymentGateway.MakePayment(payDetails);

            return payStatus;
        }

        private async Task<PaymentStatusType> PremiumPayment(PayDetails payDetails)
        {
            int retry = 0;
            var payStatus = PaymentStatusType.Pending;
            while (retry < 3)
            {
                payStatus = await _premiumPaymentGateway.MakePayment(payDetails);
                if (payStatus == PaymentStatusType.Processed)
                    break;

                retry++;
            }

            return payStatus;
        }
    }
}