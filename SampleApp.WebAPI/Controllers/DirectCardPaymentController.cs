using Microsoft.AspNetCore.Mvc;
using SampleApp.WebAPI.Models;
using SampleApp.WebAPI.RabbitMQ;
using System.Net;

namespace SampleApp.WebAPI.Controllers
{
    public class DirectCardPaymentController : BaseController
    {
        [HttpPost]
        [Route("MakePayment")]
        public IActionResult MakePayment([FromBody] CardPayment cardPayment)
        {
            string reply;

            try
            {
                var client = new RabbitMQDirectClient();
                client.CreateConnection();
                reply = client.MakePayment(cardPayment);

                client.Close();
            }
            catch (System.Exception)
            {
                return StatusCode((int)HttpStatusCode.BadRequest);
            }

            return Ok(reply);
        }
    }
}
