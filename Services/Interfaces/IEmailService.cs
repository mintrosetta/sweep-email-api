namespace sweep_email_api.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendByZimbraAsync(string receive, string subject, string body);
    }
}