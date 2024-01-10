using MailKit.Net.Imap;
using Microsoft.Extensions.Options;
using sweep_email_api.Data.Configurations;
using sweep_email_api.Models;
using sweep_email_api.Services.Interfaces;

namespace sweep_email_api.Services
{
    public class SweepService : ISweepService
    {
        private readonly ImapGmail _imapGmail;

        public SweepService(IOptions<ImapGmail> imapGmail)
        {
            this._imapGmail = imapGmail.Value;
        }

        public Task<IEnumerable<TrackReply>> FetchRepliesAsync()
        {
            using (var client = new ImapClient())
            {
                Console.WriteLine(this._imapGmail.Email);
                Console.WriteLine(this._imapGmail.Password);

                return null;
            }
        }
    }
}