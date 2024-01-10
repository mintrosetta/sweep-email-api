using sweep_email_api.Data.Configurations;
using sweep_email_api.Services;
using sweep_email_api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Load configuration data in appsetting.json
builder.Configuration.AddJsonFile($"{Environment.CurrentDirectory}\\appsettings.json");

// Add data frin MailSetting -> Gmail -> Imap to ImapGmail Model
builder.Services.Configure<ImapGmail>(builder.Configuration.GetSection("MailSetting:Gmail:Imap"));

// Add services to the container.
builder.Services.AddScoped<ISweepService, SweepService>();

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

app.Run();
