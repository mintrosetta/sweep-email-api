using Microsoft.AspNetCore.Mvc;

namespace sweep_email_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthCheckController : ControllerBase
    {
        [HttpGet()]
        public ActionResult HealthCheck() => Ok();
    }
}