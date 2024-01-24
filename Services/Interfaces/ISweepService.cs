using sweep_email_api.Models;

namespace sweep_email_api.Services.Interfaces
{
    public interface ISweepService
    {
        Task<IEnumerable<TrackReply>> FetchRepliesAsync();
        Task<IEnumerable<TrackReply>> ZimbraFetchRepliesAsync();
    }
}