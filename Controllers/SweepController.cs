using Microsoft.AspNetCore.Mvc;
using sweep_email_api.Dto;
using sweep_email_api.Models;
using sweep_email_api.Services.Interfaces;

namespace sweep_email_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SweepController : ControllerBase
    {
        private readonly ISweepService _sweepService;

        public SweepController(ISweepService sweepService)
        {
            this._sweepService = sweepService;
        }

        [HttpGet]
        public async Task<ActionResult> FetchReplies()
        {
            try
            {
                var replies = await this._sweepService.FetchRepliesAsync();

                return Ok(new ResponseDto<IEnumerable<TrackReply>>()
                {
                    Success = true,
                    Message = "Successful",
                    Data = replies
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDto<object>()
                {
                    Message = ex.Message
                });
            }
        }

        [HttpGet("zimbra")]
        public async Task<ActionResult> FetchZimbraAsync()
        {
            try
            {
                var replies = await this._sweepService.ZimbraFetchRepliesAsync();

                return Ok(new ResponseDto<IEnumerable<TrackReply>>()
                {
                    Success = true,
                    Message = "Successful",
                    Data = replies
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDto<object>()
                {
                    Message = ex.Message
                });
            }
        }
    }
}