using sweep_email_api.Services.Interfaces;

namespace sweep_email_api.Services.HangfireJobs
{
    public class SweepRepliesJob
    {
        private readonly ISweepService sweepService;

        public SweepRepliesJob(ISweepService sweepService)
        {
            this.sweepService = sweepService;
            Console.WriteLine("Start job");
        }

        public async Task Run()
        {
            var result = await this.sweepService.FetchRepliesAsync();
            Console.WriteLine($"Count replies is: {result.Count()}");
        }
    }
}
