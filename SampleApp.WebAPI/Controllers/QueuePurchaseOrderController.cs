using Microsoft.AspNetCore.Mvc;
using SampleApp.WebAPI.Models;
using SampleApp.WebAPI.RabbitMQ;
using System;
using System.Net;

namespace SampleApp.WebAPI.Controllers
{
    public class QueuePurchaseOrderController : BaseController
    {
        [HttpPost]
        [Route("MakePayment")]
        public IActionResult MakePayment([FromBody] PurchaseOrder purchaseOrder)
        {
            try
            {
                var client = new RabbitMQClient();
                client.SendPurchaseOrder(purchaseOrder);
                client.Close();
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.BadRequest);
            }

            return Ok(purchaseOrder);
        }
    }
}
