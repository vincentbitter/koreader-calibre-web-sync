using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using Sync.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("user", new OpenApiSecurityScheme
    {
        Description = "Username@url",
        In = ParameterLocation.Header,
        Name = "x-auth-user",
        Type = SecuritySchemeType.ApiKey
    });
    options.AddSecurityDefinition("pass", new OpenApiSecurityScheme
    {
        Description = "Password",
        In = ParameterLocation.Header,
        Name = "x-auth-key",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<AuthOperationsFilter>();
    options.SchemaFilter<SnakeCaseSchemaFilter>();
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance;
});

builder.Services.AddAuthentication("custom")
    .AddScheme<AuthenticationSchemeOptions, CustomAuthenticationHandler>("custom", null);
builder.Services.AddAuthorization();
builder.Services.AddSingleton<Sync.Configuration.AuthenticationService>();

Sync.Infrastructure.Setup.AddServices(builder.Services);
Sync.Application.Setup.AddServices(builder.Services);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

Sync.Application.Setup.AddRoutes(app);

app.Run();
