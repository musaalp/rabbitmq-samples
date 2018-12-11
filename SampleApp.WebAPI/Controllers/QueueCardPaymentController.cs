using Microsoft.AspNetCore.Mvc;
using SampleApp.WebAPI.Models;
using SampleApp.WebAPI.RabbitMQ;
using System.Net;

namespace SampleApp.WebAPI.Controllers
{
    public class QueueCardPaymentController : BaseController
    {
        [HttpPost]
        [Route("MakePayment")]
        public IActionResult MakePayment([FromBody] CardPayment cardPayment)
        {
            try
            {
                var client = new RabbitMQClient();
                client.SendPayment(cardPayment);
                client.Close();
            }
            catch (System.Exception)
            {
                return StatusCode((int)HttpStatusCode.BadRequest);
            }

            return Ok(cardPayment);
        }
    }
}
