using Microsoft.AspNetCore.Mvc;
using sweep_email_api.Dto;
using sweep_email_api.Services.Interfaces;

namespace sweep_email_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SendMailController : ControllerBase
    {
        private readonly IEmailService emailService;

        public SendMailController(IEmailService emailService)
        {
            this.emailService = emailService;
        }

        [HttpPost("zimbra")]
        public async Task<ActionResult> SendByZimbra()
        {
            await this.emailService.SendByZimbraAsync("mint.rosetta2001@gmail.com", "Testing zimbra", "Testing zimbra");

            return Ok(new ResponseDto<string>()
            {
                Success = true,
                Message = "Successful",
                Data = null
            });
        }
    }
}