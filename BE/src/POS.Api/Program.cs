using System.Security.Claims;
using System.Text.Json;
using POS.Application;
using POS.Infrastructure;
using POS.Infrastructure.Persistence.Seed;
using POS.Api.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddControllers();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Staff";
    options.DefaultChallengeScheme = "Staff";
})
    .AddJwtBearer("Staff", options =>
    {
        options.Authority = "http://localhost:8080/realms/pos-staff";
        options.Audience = "account";
        // Keycloak runs over plain HTTP in local dev - never do this in production.
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                if (context.Principal?.Identity is not ClaimsIdentity identity)
                    return Task.CompletedTask;

                var realmAccessClaim = context.Principal.FindFirst("realm_access");
                if (realmAccessClaim is null)
                    return Task.CompletedTask;

                using var realmAccess = JsonDocument.Parse(realmAccessClaim.Value);
                if (!realmAccess.RootElement.TryGetProperty("roles", out var roles))
                    return Task.CompletedTask;

                foreach (var role in roles.EnumerateArray())
                {
                    var roleName = role.GetString();
                    if (!string.IsNullOrWhiteSpace(roleName))
                        identity.AddClaim(new Claim(ClaimTypes.Role, roleName));
                }

                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            }
        };
    })
    .AddJwtBearer("Customer", options =>
    {
        options.Authority = "http://localhost:8080/realms/pos-customers";
        options.Audience = "account";
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                if (context.Principal?.Identity is not ClaimsIdentity identity)
                    return Task.CompletedTask;

                var realmAccessClaim = context.Principal.FindFirst("realm_access");
                if (realmAccessClaim is null)
                    return Task.CompletedTask;

                using var realmAccess = JsonDocument.Parse(realmAccessClaim.Value);
                if (!realmAccess.RootElement.TryGetProperty("roles", out var roles))
                    return Task.CompletedTask;

                foreach (var role in roles.EnumerateArray())
                {
                    var roleName = role.GetString();
                    if (!string.IsNullOrWhiteSpace(roleName))
                        identity.AddClaim(new Claim(ClaimTypes.Role, roleName));
                }

                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
                          policy =>
                          {
                              policy.AllowAnyOrigin()
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                          });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "POS API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type=Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});


builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder("Staff", "Customer")
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddAntiforgery(options => options.HeaderName = "XSRF-TOKEN");
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Populates an empty dev database with test data (users/customers/products/orders/payments).
    // No-ops if data already exists - see DatabaseSeeder.SeedAsync.
    await DatabaseSeeder.SeedAsync(app.Services);
}

app.UseExceptionHandler();



app.UseHttpsRedirection();


app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

