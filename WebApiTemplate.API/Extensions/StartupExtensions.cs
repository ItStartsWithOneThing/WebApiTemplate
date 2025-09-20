using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebApiTemplate.API.APIs;
using WebApiTemplate.API.Mappers;
using WebApiTemplate.Application.Iterfaces;
using WebApiTemplate.Application.Models;
using WebApiTemplate.Application.Services;
using WebApiTemplate.Domain.Data.Interfaces;
using WebApiTemplate.Infrastructure.Repositories;
using WebApiTemplate.Infrastructure.Repositories.Base;

namespace WebApiTemplate.API.Extensions;

public static class StartupExtensions
{
    public static WebApplicationBuilder ConfigureAppServices(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection(nameof(AuthOptions)));
        builder.AddAuthentication();
        builder.Services.AddSwagger();
        builder.Services.AddOpenApi();
        builder.Services.AddAutoMapperConfig();
        builder.Services.AddServices();
        builder.Services.AddInfrastructureServices();

        return builder;
    }

    public static async Task ConfigureAppPipelineAsync(this WebApplication app)
    {
        app
            .MapAuthAPI()
            .MapAdminAPI();

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapOpenApi();

        app.UseSwagger();
        app.UseSwaggerUI();
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

        builder.Services.AddAuthorization();

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

    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(opt =>
        {
            opt.DescribeAllParametersInCamelCase();

            opt.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "API V1",
                Version = "v1"
            });

            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Enter token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });

            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
        });

        return services;
    }

    public static IServiceCollection AddAutoMapperConfig(this IServiceCollection services)
    {
        var profiles = new Type[]
        {
            typeof(MappingProfile)
        };

        services.AddAutoMapper(cfg => { }, profiles);
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAccountService, AccountService>();

        return services;
    }
}