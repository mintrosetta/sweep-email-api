using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

                return Ok(new
                {
                    Success = true,
                    Message = "Successful",
                    Data = replies
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message!
                });
            }
        }
    }
}