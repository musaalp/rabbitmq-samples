using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace SampleApp.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController : Controller
    {
    }
}
