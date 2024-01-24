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
            await this.emailService.SendByZimbraAsync(
                "mint.rosetta2001@gmail.com", 
                "ขอตรวจสอบยอดเงินลูกค้า [TICKET ID: 10]", 
                "ขอตรวจสอบยอดเงินของลูกค้าชื่อ นาย ภัทรพงค์ ชุมวงศ์จันทร์ เป็นจำนวนเงิน 700 บาท ถูกต้องหรือไม่ ?");

            return Ok(new ResponseDto<string>()
            {
                Success = true,
                Message = "Successful",
                Data = null
            });
        }
    }
}