using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Filed.PaymentProcess.API.Data;
using Filed.PaymentProcess.API.Model;
using Filed.PaymentProcess.API.Model.BLL;
using Moq;
using NUnit.Framework;

namespace Filed.PaymentProcess.API.Tests.Model.BLL
{
    public class PaymentHandlerTests
    {
        private Mock<IRepository<PayDetails>> _payRepository;
        private Mock<IRepository<PaymentStatus>> _payStatusRepository;
        private Mock<ICheapPaymentGateway> _cheapPaymentGateway;
        private Mock<IExpensivePaymentGateway> _expensivePaymentGateway;
        private Mock<IPremiumPaymentGateway> _premiumPaymentGateway;
        private PaymentHandler _handler;
        
        [SetUp]
        public void Setup()
        {
            _payRepository = new Mock<IRepository<PayDetails>>();
            _payStatusRepository = new Mock<IRepository<PaymentStatus>>();
            _cheapPaymentGateway = new Mock<ICheapPaymentGateway>();
            _expensivePaymentGateway = new Mock<IExpensivePaymentGateway>();
            _premiumPaymentGateway = new Mock<IPremiumPaymentGateway>();
            _handler = new PaymentHandler(_payRepository.Object, _payStatusRepository.Object, _cheapPaymentGateway.Object, _expensivePaymentGateway.Object, _premiumPaymentGateway.Object);
        }

        [Test]
        public async Task PaymentDoneByCheapPaymentGateway()
        {
            var cheapPayDetails = new PayDetails { CreditCardNumber = "1234567891234567", CardHolder = "Test", ExpirationDate = DateTime.Now.AddMonths(1), SecurityCode = 100, Amount = 10 };
            _cheapPaymentGateway.Setup(s => s.MakePayment(cheapPayDetails)).ReturnsAsync(PaymentStatusType.Processed);
            _payRepository.Setup(p => p.Add(cheapPayDetails)).ReturnsAsync(new PayDetails { Id = 1 });
            _payStatusRepository.Setup(p => p.Add(It.IsAny<PaymentStatus>())).ReturnsAsync(new PaymentStatus { Id = 1, Status = PaymentStatusType.Processed.ToString(), PayDetailId = 1 }); 

            var result = await _handler.DoPayment(cheapPayDetails);

            Assert.AreEqual(PaymentStatusType.Processed.ToString(), result);
            _expensivePaymentGateway.Verify(s => s.MakePayment(cheapPayDetails), Times.Never);
            _premiumPaymentGateway.Verify(s => s.MakePayment(cheapPayDetails), Times.Never);
        }

        [Test]
        public async Task PaymentDoneByExpensivePaymentGateway()
        {
            var expensivePayDetails = new PayDetails { CreditCardNumber = "1234567891234567", CardHolder = "Test", ExpirationDate = DateTime.Now.AddMonths(1), SecurityCode = 100, Amount = 50 };
            _expensivePaymentGateway.Setup(s => s.MakePayment(expensivePayDetails)).ReturnsAsync(PaymentStatusType.Processed);
            _payRepository.Setup(p => p.Add(expensivePayDetails)).ReturnsAsync(new PayDetails { Id = 1 });
            _payStatusRepository.Setup(p => p.Add(It.IsAny<PaymentStatus>())).ReturnsAsync(new PaymentStatus { Id = 1, Status = PaymentStatusType.Processed.ToString(), PayDetailId = 1 });

            var result = await _handler.DoPayment(expensivePayDetails);

            Assert.AreEqual(PaymentStatusType.Processed.ToString(), result);
            _cheapPaymentGateway.Verify(s => s.MakePayment(expensivePayDetails), Times.Never);
            _premiumPaymentGateway.Verify(s => s.MakePayment(expensivePayDetails), Times.Never);
        }

        [Test]
        public async Task PaymentDoneByPremiumPaymentGateway()
        {
            var premiumPayDetails = new PayDetails { CreditCardNumber = "1234567891234567", CardHolder = "Test", ExpirationDate = DateTime.Now.AddMonths(1), SecurityCode = 100, Amount = 550 };
            _premiumPaymentGateway.Setup(s => s.MakePayment(premiumPayDetails)).ReturnsAsync(PaymentStatusType.Processed);
            _payRepository.Setup(p => p.Add(premiumPayDetails)).ReturnsAsync(new PayDetails { Id = 1 });
            _payStatusRepository.Setup(p => p.Add(It.IsAny<PaymentStatus>())).ReturnsAsync(new PaymentStatus { Id = 1, Status = PaymentStatusType.Processed.ToString(), PayDetailId = 1 });

            var result = await _handler.DoPayment(premiumPayDetails);

            Assert.AreEqual(PaymentStatusType.Processed.ToString(), result);
            _cheapPaymentGateway.Verify(s => s.MakePayment(premiumPayDetails), Times.Never);
            _expensivePaymentGateway.Verify(s => s.MakePayment(premiumPayDetails), Times.Never);
        }

        [Test]
        public async Task PaymentDoneByCheapGatewayWhenExpensivePaymentGatewayFailed()
        {
            var _payDetails = new PayDetails { CreditCardNumber = "1234567891234567", CardHolder = "Test", ExpirationDate = DateTime.Now.AddMonths(1), SecurityCode = 100, Amount = 50 };
            _expensivePaymentGateway.Setup(s => s.MakePayment(_payDetails)).ReturnsAsync(PaymentStatusType.Failed);
            _cheapPaymentGateway.Setup(s => s.MakePayment(_payDetails)).ReturnsAsync(PaymentStatusType.Processed);
            _payRepository.Setup(p => p.Add(_payDetails)).ReturnsAsync(new PayDetails { Id = 1 });
            _payStatusRepository.Setup(p => p.Add(It.IsAny<PaymentStatus>())).ReturnsAsync(new PaymentStatus { Id = 1, Status = PaymentStatusType.Processed.ToString(), PayDetailId = 1 });

            var result = await _handler.DoPayment(_payDetails);

            Assert.AreEqual(PaymentStatusType.Processed.ToString(), result);
            _cheapPaymentGateway.Verify(s => s.MakePayment(_payDetails), Times.Once);
            _premiumPaymentGateway.Verify(s => s.MakePayment(_payDetails), Times.Never);
        }

        [Test]
        public async Task PremiumPaymentGatewayFailedForAllRetries()
        {
            var _payDetails = new PayDetails { CreditCardNumber = "1234567891234567", CardHolder = "Test", ExpirationDate = DateTime.Now.AddMonths(1), SecurityCode = 100, Amount = 550 };
            _premiumPaymentGateway.Setup(s => s.MakePayment(_payDetails)).ReturnsAsync(PaymentStatusType.Failed);
            _payRepository.Setup(p => p.Add(_payDetails)).ReturnsAsync(new PayDetails { Id = 1 });
            _payStatusRepository.Setup(p => p.Add(It.IsAny<PaymentStatus>())).ReturnsAsync(new PaymentStatus { Id = 1, Status = PaymentStatusType.Failed.ToString(), PayDetailId = 1 });

            var result = await _handler.DoPayment(_payDetails);

            Assert.AreEqual(PaymentStatusType.Failed.ToString(), result);
            _premiumPaymentGateway.Verify(s => s.MakePayment(_payDetails), Times.Exactly(3));
            _cheapPaymentGateway.Verify(s => s.MakePayment(_payDetails), Times.Never);
            _expensivePaymentGateway.Verify(s => s.MakePayment(_payDetails), Times.Never);
        }
    }
}
