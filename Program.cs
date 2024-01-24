using Hangfire;
using Hangfire.Storage.SQLite;
using sweep_email_api.Data.Configurations;
using sweep_email_api.Services;
using sweep_email_api.Services.HangfireJobs;
using sweep_email_api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Load configuration data in appsetting.json
builder.Configuration.AddJsonFile($"{Environment.CurrentDirectory}\\appsettings.json");

// Add data frin MailSetting -> Gmail -> Imap to ImapGmail Model
builder.Services.Configure<ImapGmail>(builder.Configuration.GetSection("MailSetting:Gmail:Imap"));
builder.Services.Configure<SmtpZimbra>(builder.Configuration.GetSection("MailSetting:Zimbra:Smtp"));
builder.Services.Configure<ImapZimbra>(builder.Configuration.GetSection("MailSetting:Zimbra:Imap"));

// Add services to the container.
builder.Services.AddScoped<ISweepService, SweepService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<SweepRepliesJob>();

// initial hangfire configuration
builder.Services.AddHangfire(config => config.UseSimpleAssemblyNameTypeSerializer()
                                             .UseRecommendedSerializerSettings()
                                             .UseSQLiteStorage($"Data Source=hangfire.db;"));
builder.Services.AddHangfireServer();


// tell .NET use Controller
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// configuration hangfire dashboard
app.UseHangfireDashboard();
app.MapHangfireDashboard();

RecurringJob.AddOrUpdate<SweepRepliesJob>("SweepJob", x => x.Run(), Cron.Minutely());

app.Run();
