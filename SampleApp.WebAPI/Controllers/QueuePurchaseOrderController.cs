using Microsoft.AspNetCore.Mvc;
using SampleApp.WebAPI.Models;

namespace SampleApp.WebAPI.Controllers
{
    public class QueuePurchaseOrderController : Controller
    {
        [HttpPost]
        public IActionResult MakePayment([FromBody] PurchaseOrder purchaseOrder)
        {
            return Ok(cardPayment);
        }
    }
}
