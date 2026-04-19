using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PetAdopt.API.Middlewares;
using PetAdopt.Application.DependencyInjection;
using PetAdopt.Domain.Entities;
using PetAdopt.Domain.Enums;
using PetAdopt.Infrastructure.DependencyInjection;
using PetAdopt.Infrastructure.Hubs;
using PetAdopt.Persistence;
using PetAdopt.Persistence.DependencyInjection;
using PetAdopt.Persistence.Seeders;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using System.Data;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Diagnostics", LogEventLevel.Fatal)
    .MinimumLevel.Override("Microsoft.AspNetCore.Server", LogEventLevel.Fatal)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .WriteTo.Console()
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(
        new Uri(builder.Configuration["Serilog:ElasticSearch:Uri"]))
    {
        IndexFormat = string.Format(
            builder.Configuration["Serilog:ElasticSearch:IndexFormat"],
            DateTime.UtcNow),
        AutoRegisterTemplate = true,
        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv8,
        NumberOfReplicas = 1,
        NumberOfShards = 2
    })
    .CreateLogger();

builder.Host.UseSerilog();

//Adding Services For Other Layers
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

// Add Services
builder.Services.AddControllers()
        // To serialize enums as strings in JSON responses
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
builder.Services.AddEndpointsApiExplorer();

//Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
    options.InstanceName = "PetAdopt_";
});

// SignalR UserIdProvider
builder.Services.AddSingleton<IUserIdProvider, NameIdentifierUserIdProvider>();
//SignalR
builder.Services.AddSignalR();

builder.Services.AddSwaggerGen();

//JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var key = Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]);

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };

    //Read from cookie
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Check if the request is for the SignalR hub
            var accessToken = context.Request.Query["access_token"];
            // read from the cookie
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken))
            {
                context.Token = accessToken;
            }
            else
            {
                // Read token from cookie
                var token = context.HttpContext.Request.Cookies["jwt"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
            }   
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

//Rates Limiting
builder.Services.AddRateLimiter(options =>
{
    // Global Limiter => 100 request per min for each user
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var role = context.User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var ip = context.Connection.RemoteIpAddress?.ToString();

        // Admins have 1000 requests per minute NOT Like basic users
        if (context.User.IsInRole("Admin"))
        {
            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: userId ?? ip ?? "admin",
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 20, //1000 requests
                    Window = TimeSpan.FromMinutes(1)
                });
        }

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: userId ?? ip ?? "anonymous",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 10,             // 100 requests
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0 // NO wait
            });
    });
    // Auth policy => 5 request per 15 min for each user Like login, register
    options.AddPolicy("Auth-Limit", context =>
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var ip = context.Connection.RemoteIpAddress?.ToString();

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: userId ?? ip ?? "anonymous",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1), // 5 request per 15 min
                QueueLimit = 0
            });
    });

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        context.HttpContext.Response.Headers["Retry-After"] = "60";

        await context.HttpContext.Response.WriteAsync(
            "Too many requests. Please try again later.", token);
    };
});

//CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000"    // React
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // allow cookies and authentication headers to sent in cross-origin requests
    });
});


var app = builder.Build();

app.UseSerilogRequestLogging();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

// SignalR Hub Mapping

app.UseStaticFiles();

app.UseRateLimiter();

app.UseCors("AllowFrontend");

app.UseAuthentication(); 
app.UseAuthorization();


app.MapHub<NotificationHub>("/hubs/notifications");

app.MapControllers();

// Admin Seeding
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider
                        .GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider
                        .GetRequiredService<RoleManager<IdentityRole>>();

    await AdminSeeder.SeedAsync(userManager, roleManager);
}

app.Run();