using System;
using System.Threading.Tasks;
using Filed.PaymentProcess.API.Controllers;
using Filed.PaymentProcess.API.Model;
using Filed.PaymentProcess.API.Model.BLL;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Filed.PaymentProcess.API.Tests.Controller
{
    public class PaymentProcessorControllerTests
    {
        private PaymentProcessorController _controller; 
        private Mock<IPaymentHandler> _paymentHandler;
        private static string ProcessedStatus = PaymentStatusType.Processed.ToString();
        [SetUp]
        public void Setup()
        {
            _paymentHandler = new Mock<IPaymentHandler>();
            _controller = new PaymentProcessorController(_paymentHandler.Object);
        }

        [TestCase(20, "Processed")]
        [TestCase(450, "Processed")]
        [TestCase(550, "Processed")]
        public async Task PaymentProcessedSuccessfully(decimal amount, string status)
        {
            var request = new PayDetails { CreditCardNumber = "1234567891234567", CardHolder = "Test", ExpirationDate = DateTime.Now.AddMonths(1), SecurityCode = 100, Amount = amount };
            _paymentHandler.Setup(a => a.DoPayment(request)).ReturnsAsync(status);
            var response = await _controller.ProcessPayment(request);
            var okResult = response as OkObjectResult;

            var apiResponse = okResult.Value as APIResponse;

            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(true, apiResponse.IsSuccess);
            Assert.AreEqual($"Payment Status: {status}", apiResponse.Message);
        }

        [Test]
        public async Task ExceptionHandledWhilePamentProcess()
        { 
            var request = new PayDetails { CreditCardNumber = "1234567891234567", CardHolder = "Test", ExpirationDate = DateTime.Now.AddMonths(1), SecurityCode = 100, Amount = -100 };
            
            _paymentHandler.Setup(a => a.DoPayment(request)).Throws(new Exception("Internal Server Error"));
            var response = await _controller.ProcessPayment(request);

            var errorResult = response as StatusCodeResult;
            Assert.AreEqual(500, errorResult.StatusCode);
        }
    }
}