#pragma warning disable IDE0058

using AuthServer.Contracts;
using AuthServer.Extensions;
using AuthServer.Middleware;
using AuthServer.Models;
using AuthServer.Services.Authentication;
using AuthServer.Services.Client;
using AuthServer.Services.Token;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(
    (context, services, configuration) =>
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
);

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowSpaOrigin",
        builder =>
        {
            builder
                .WithOrigins(
                    "http://localhost:5173", // Vite dev server default
                    "http://127.0.0.1:5173"
                )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        }
    );
});

// Add services to the container
builder
    .Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configure JSON serialization to handle both snake_case and PascalCase
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // Use property names as-is
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true; // Case-insensitive property matching
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        // Configure form binding to handle snake_case parameter names
        options.SuppressModelStateInvalidFilter = true;
    })
    .AddMvcOptions(options =>
    {
        // Configure form options to handle form data
        options.EnableEndpointRouting = true;
    });

// Configure form options
builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();

// Add authentication
builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Token:Issuer"],
            ValidAudience = builder.Configuration["Token:Audience"],
            NameClaimType = "name",
            RoleClaimType = "role",
        };
    });

builder
    .Services.AddSingleton<IUserRepository, InMemoryUserRepository>()
    .AddScoped<IAuthenticationService, AuthenticationService>()
    .AddSingleton<IKeyManagementService, KeyManagementService>()
    .AddSingleton<IRedirectUriValidator, RedirectUriValidator>()
    .AddSingleton<IRefreshTokenService, RefreshTokenService>()
    .AddSingleton<IAuthCodeRepository, InMemoryAuthCodeRepository>()
    .AddSingleton<IOAuthClientRepository, InMemoryOAuthClientRepository>()
    .RegisterConfiguration<TokenConfiguration>("Token", ServiceLifetime.Singleton)
    .AddDistributedMemoryCache()
    .AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(5);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

builder
    .Services.AddHealthChecks()
    .AddCheck("LivenessCheck", () => HealthCheckResult.Healthy(), ["liveness"])
    .AddCheck<KeyManagementHealthCheck>("KeyManagement", tags: ["readiness"]);

// Add background service to clean up expired refresh tokens
// This would likely be moved to an Azure Function or similar
builder.Services.AddHostedService<RefreshTokenCleanupService>();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
    app.Use(
        async (context, next) =>
        {
            // Prevent clickjacking
            context.Response.Headers.Append("X-Frame-Options", "DENY");
            // Prevent MIME type sniffing
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            // XSS protection
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
            // Referrer policy
            context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
            // Content Security Policy
            context.Response.Headers.Append(
                "Content-Security-Policy",
                "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'"
            );

            await next();
        }
    );
}

app.UseCors("AllowSpaOrigin");
app.UseSession();
app.ConfigureJwtValidation();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapControllers();
app.MapRazorPages();

app.MapHealthChecks(
    "/health/live",
    new HealthCheckOptions { Predicate = check => check.Tags.Contains("liveness") }
);

app.MapHealthChecks(
    "/health/ready",
    new HealthCheckOptions { Predicate = check => check.Tags.Contains("readiness") }
);

app.Run();

#pragma warning restore IDE0058
