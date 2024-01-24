using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Microsoft.Extensions.Options;
using sweep_email_api.Data.Configurations;
using sweep_email_api.Models;
using sweep_email_api.Services.Interfaces;
using System.Text;

namespace sweep_email_api.Services
{
    public class SweepService : ISweepService
    {
        private readonly ImapGmail _imapGmail;
        private readonly ImapZimbra imapZimbra;

        public SweepService(IOptions<ImapGmail> imapGmail, IOptions<ImapZimbra> imapZimbra)
        {
            this._imapGmail = imapGmail.Value;
            this.imapZimbra = imapZimbra.Value;
        }

        public async Task<IEnumerable<TrackReply>> FetchRepliesAsync()
        {
            using (var client = new ImapClient())
            {
                // connect to gmail imap server
                await client.ConnectAsync(this._imapGmail.Host, this._imapGmail.Port, MailKit.Security.SecureSocketOptions.SslOnConnect);

                // authentication
                await client.AuthenticateAsync(this._imapGmail.Email, this._imapGmail.Password);

                // get inbox folder
                var inbox = client.Inbox;

                // open folder
                await inbox.OpenAsync(FolderAccess.ReadWrite);

                // search email has subject Re: || re: || �ͺ��Ѻ: && has "�͵�Ǩ�ͺ�ʹ�Թ�١���"
                var searchCondition = SearchQuery.SubjectContains("Re:")
                                                 .Or(SearchQuery.SubjectContains("re:"))
                                                 .Or(SearchQuery.SubjectContains("re:"))
                                                 .Or(SearchQuery.SubjectContains("�ͺ��Ѻ:"))
                                                 .And(SearchQuery.SubjectContains("�͵�Ǩ�ͺ�ʹ�Թ�١���"))
                                                 .And(SearchQuery.NotSeen);

                // search email
                var rawReplies = await inbox.SearchAsync(searchCondition);

                List<TrackReply> replies = new List<TrackReply>();
                foreach (var rawReply in rawReplies)
                {
                    // get id of message
                    UniqueId uid = rawReply;

                    // get email message by uid
                    var message = await inbox.GetMessageAsync(uid);

                    // get TextBody, remove all space from \r, splite TextBody by \n
                    // only accepts index 0 because at index 0 is the actual response message
                    string replyDesc = message.TextBody.Replace("\r", "").Split('\n')[0];

                    replies.Add(new TrackReply
                    {
                        TrackId = this.FilterTrackId(message.Subject),
                        Description = replyDesc,
                        Amount = this.FilterAmount(replyDesc)
                    });

                    // set email as read
                    await inbox.AddFlagsAsync(uid, MessageFlags.Seen, true);
                }

                return replies;
            }
        }

        public async Task<IEnumerable<TrackReply>> ZimbraFetchRepliesAsync()
        {
            using (var client = new ImapClient())
            {
                // connect to gmail imap server
                await client.ConnectAsync(this.imapZimbra.Host, this.imapZimbra.Port, MailKit.Security.SecureSocketOptions.SslOnConnect);

                // authentication
                await client.AuthenticateAsync(this.imapZimbra.Email, this.imapZimbra.Password);

                // get inbox folder
                var inbox = client.Inbox;

                // open folder
                await inbox.OpenAsync(FolderAccess.ReadWrite);

                // search email has subject Re: || re: || �ͺ��Ѻ: && has "�͵�Ǩ�ͺ�ʹ�Թ�١���"
                var searchCondition = SearchQuery.SubjectContains("Re:")
                                                 .Or(SearchQuery.SubjectContains("ตอบกลับ:"))
                                                 .And(SearchQuery.SubjectContains("ขอตรวจสอบยอดเงินลูกค้า"))
                                                 .And(SearchQuery.NotSeen);

                // search email
                var rawReplies = await inbox.SearchAsync(searchCondition);

                List<TrackReply> replies = new List<TrackReply>();
                foreach (var rawReply in rawReplies)
                {
                    // get id of message
                    UniqueId uid = rawReply;

                    // get email message by uid
                    var message = await inbox.GetMessageAsync(uid);

                    // get TextBody, remove all space from \r, splite TextBody by \n
                    // only accepts index 0 because at index 0 is the actual response message
                    string replyDesc = message.TextBody.Replace("\r", "").Split('\n')[0];

                    replies.Add(new TrackReply
                    {
                        TrackId = this.FilterTrackId(message.Subject),
                        Description = replyDesc,
                        Amount = this.FilterAmount(replyDesc)
                    });

                    // set email as read
                    await inbox.AddFlagsAsync(uid, MessageFlags.Seen, true);
                }

                return replies;
            }
        }

        private string FilterTrackId(string subject)
        {
            // we need find index of [ and ], because track id between [ and ]
            int? start = null; // inedx of [
            int? end = null; // index of ]

            for (int index = 0; index < subject.Length; index++)
            {
                if (subject[index].Equals('[')) start = index;
                if (subject[index].Equals(']')) end = index;

                // if start and end is not null, break loop because we have indexes for both
                if (start != null && end != null) break;
            }

            // if start or end is null, throw error because subject is invalid
            if (start == null && end == null) throw new Exception("subject is invalid.");

            // there may be many concat string, use string builder for less memory <3
            StringBuilder trackBuilder = new StringBuilder();
            for (int index = (int) start!; index <= end; index++)
            {
                // track id has only digit
                // if letter between 0 - 9, that letter is a digit
                bool digitCondition = (subject[index] >= '0' && subject[index] <= '9');
                if (digitCondition) trackBuilder.Append(subject[index]);
            }
             
            return trackBuilder.ToString();
        }

        private decimal FilterAmount(string message)
        {
            if (message.Length == 0) return 0;

            string amountText = string.Empty;
            for (int messageIndex = 0; messageIndex < message.Length; messageIndex++)
            {
                char character = message[messageIndex];

                // if letter between 0 - 9, that letter is a digit
                bool numberCondition = (character >= '0' && character <= '9');

                // if letter is '.'
                bool dotCondition = (
                    (character == '.' || character == ',')
                    && (message[messageIndex - 1] >= '0' && message[messageIndex - 1] <= '9') // position before '.' is a number
                    && messageIndex + 1 < message.Length // message next index should must less than length of message
                    && (message[messageIndex + 1] >= '0' && message[messageIndex + 1] <= '9') // next position must be a letter
                );

                bool nextIsNumberOrDotCondition = (
                    messageIndex + 1 < message.Length
                    && (
                            (message[messageIndex + 1] >= '0' && message[messageIndex + 1] <= '9')
                            || message[messageIndex + 1] == '.'
                            || message[messageIndex + 1] == ','
                        )
                );


                if (numberCondition) // if letter is number
                {
                    // if index of digit in array is equal last index, concat to amountText
                    if (messageIndex == message.Length - 1)
                    {
                        amountText += character;
                    }
                    else
                    {
                        // if letter is not '.', concat to amountText
                        if (message[messageIndex + 1] != '.')
                        {
                            amountText += character;
                        }
                        else
                        {
                            // if the next character after '.' is digit, concat to amountText 
                            if ((messageIndex + 2) <= message.Length && message[messageIndex + 2] >= '0' && message[messageIndex + 2] <= '9') amountText += character;
                        }
                    }

                    if (!nextIsNumberOrDotCondition) break;
                }
                else if (dotCondition) // if character is '.'
                {
                    amountText += character; // concat to amountText
                }
            }

            // if amountText is empty return 0, if amountText is not empty convert to decimal and return
            return (string.IsNullOrEmpty(amountText)) ? 0 : Convert.ToDecimal(amountText);
        }
    }
}