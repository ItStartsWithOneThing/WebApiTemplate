using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApiTemplate.API.APIs;
using WebApiTemplate.Application.Models;
using WebApiTemplate.Domain.Data.Interfaces;
using WebApiTemplate.Infrastructure.Repositories;
using WebApiTemplate.Infrastructure.Repositories.Base;

namespace WebApiTemplate.API.Extensions;

public static class StartupExtensions
{
    public static WebApplicationBuilder ConfigureAppServices(this WebApplicationBuilder builder)
    {
        builder.AddAuthentication();
        builder.Services.AddInfrastructureServices();

        return builder;
    }

    public static async Task ConfigureAppPipelineAsync(this WebApplication app)
    {
        app.MapAuthAPI();
    }

    public static WebApplicationBuilder AddAuthentication(this WebApplicationBuilder builder)
    {
        var serviceProvider = builder.Services.BuildServiceProvider();
        var authOptions = serviceProvider.GetRequiredService<IOptions<AuthOptions>>().Value;

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = authOptions.Issuer,
                    ValidAudience = authOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.Key))
                };

                // For SignalR auth
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = ctx =>
                    {
                        var path = ctx.HttpContext.Request.Path;
                        var hubUrls = new HashSet<string>(authOptions.Hubs ?? []);

                        // Если запрос идет к одному из хабов
                        if (hubUrls.Any(url => path.StartsWithSegments(url)))
                        {
                            // SignalR клиент сам положит токен в заголовок Authorization
                            var accessToken = ctx.Request.Query["access_token"];
                            if (!string.IsNullOrEmpty(accessToken))
                            {
                                ctx.Token = accessToken;
                            }
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        return builder;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Base repository support for DI
        services.AddScoped(typeof(BaseRepository<>));

        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();

        return services;
    }
}