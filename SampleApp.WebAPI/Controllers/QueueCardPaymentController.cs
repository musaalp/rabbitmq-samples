using Microsoft.AspNetCore.Mvc;
using SampleApp.WebAPI.Models;

namespace SampleApp.WebAPI.Controllers
{
    public class QueueCardPaymentController : Controller
    {
        [HttpPost]
        public IActionResult MakePayment([FromBody] CardPayment cardPayment)
        {
            return Ok(cardPayment);
        }
    }
}
