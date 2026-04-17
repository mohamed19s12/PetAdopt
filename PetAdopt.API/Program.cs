using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PetAdopt.API.Middlewares.PetAdopt.API.Middlewares;
using PetAdopt.Application.DependencyInjection;
using PetAdopt.Domain.Entities;
using PetAdopt.Infrastructure.DependencyInjection;
using PetAdopt.Infrastructure.Hubs;
using PetAdopt.Persistence;
using PetAdopt.Persistence.DependencyInjection;
using PetAdopt.Persistence.Seeders;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

//Adding Services For Other Layers
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructure();

// Add Services
builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
builder.Services.AddEndpointsApiExplorer();

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
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User?.Identity?.Name ?? context.Request.Headers.Host.ToString(),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,        // 100 request
                Window = TimeSpan.FromMinutes(1)  
            }));

    // Auth policy => 5 request per 15 min for each user
    options.AddFixedWindowLimiter("Auth-Limit", opt =>
    {
        opt.PermitLimit = 50;             // 5 requests
        opt.Window = TimeSpan.FromMinutes(15);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst; // Process requests in the order they were received
        opt.QueueLimit = 0; // NO wait
    });

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        await context.HttpContext.Response.WriteAsync(
            "Too many requests. Please try again later.", token);
    };
});


var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRateLimiter();

app.UseExceptionHandler();

app.UseHttpsRedirection();

// SignalR Hub Mapping

app.UseStaticFiles();

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