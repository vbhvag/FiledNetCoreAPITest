using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Filed.PaymentProcess.API.Model;
using Filed.PaymentProcess.API.Model.BLL;
using Filed.PaymentProcess.API.Filters;

namespace Filed.PaymentProcess.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentProcessorController : ControllerBase
    {
        private readonly IPaymentHandler _handler;
        public PaymentProcessorController(IPaymentHandler handler)
        {
            _handler = handler;
        }

        [HttpPost("ProcessPayment")]
        [ValidateModel]
        public async Task<IActionResult> ProcessPayment([FromBody] PayDetails payDetails)
        {
            try
            {
                var paymentStatus = await _handler.DoPayment(payDetails);
                var response = new APIResponse { IsSuccess = true, Message = $"Payment Status: {paymentStatus}" };
                return new OkObjectResult(response);
            }
            catch
            {
                return new StatusCodeResult(500);
            }
            
        }
    }
}