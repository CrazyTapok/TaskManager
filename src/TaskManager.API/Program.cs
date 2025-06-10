using DotNetEnv;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;
using TaskManager.API.Contracts.Extensions;
using TaskManager.API.Contracts.HealthChecks;
using TaskManager.Core.Infrastructure;
using TaskManager.Core.Infrastructure.Configuration;
using TaskManager.Core.Interfaces.Services.Scheduling;
using TaskManager.Infrastructure.EF;

var builder = WebApplication.CreateBuilder(args);


// Load environment variables from .env file
Env.Load("../.env");
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});

// Registration of FluentValidation via the extension method
builder.Services.AddValidationServices();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContextInfrastructure(builder.Configuration);

builder.Services.AddServiceModule();

// Set culture to English (United States)
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");


//var notificationSettings = builder.Configuration.GetSection(NotificationSettings.SectionName).Get<NotificationSettings>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection(SmtpSettings.SectionName));
builder.Services.Configure<NotificationSettings>(builder.Configuration.GetSection(NotificationSettings.SectionName));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JWT").Get<JwtSettings>();

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings?.Issuer,
        ValidAudience = jwtSettings?.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Key ?? string.Empty))
    };
});

builder.Services.AddHangfire(config => config
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"),
        new SqlServerStorageOptions
        {
            PrepareSchemaIfNecessary = true,
            QueuePollInterval = TimeSpan.FromSeconds(15)
        }));

builder.Services.AddHangfireServer();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var jobScheduler = scope.ServiceProvider.GetRequiredService<IDailyNewsletterSchedulerService>();
    jobScheduler.ConfigureJobs();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseHangfireDashboard("/hangfire");

app.UseHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new HealthCheckReponse
        {
            Status = report.Status.ToString(),
            HealthChecks = report.Entries.Select(entry => new IndividualHealthCheckResponse
            {
                Component = entry.Key,
                Status = entry.Value.Status.ToString(),
                Description = entry.Value.Description
            }),
            HealthCheckDuration = report.TotalDuration
        };
        await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
    }
});

app.Run();