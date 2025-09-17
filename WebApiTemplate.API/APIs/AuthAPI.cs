using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using WebApiTemplate.API.Models;
using WebApiTemplate.Application.Iterfaces;

namespace WebApiTemplate.API.APIs;

public static class AuthAPI
{
    public static IEndpointRouteBuilder MapAuthAPI(this IEndpointRouteBuilder app)
    {
        var v1 = app.MapGroup("/api/v1/auth")
            .WithMetadata(new ApiVersionAttribute(1));

        v1.MapPost("/login", Login)
            .WithName("Post Login")
            .WithSummary("This endpoint is used to authenticate a user based on the provided credentials.")
            .WithDescription("Returns: 200ok")
            .Produces(StatusCodes.Status200OK);

        return app;
    }

    private static async Task<IResult> Login(
        [FromBody] LoginRequest credentials,
        [FromServices] IAuthService authService)
    {
        var securityToken = await authService.Login(credentials.Username, credentials.Password);

        var refreshToken = await authService.CreateRefreshToken(credentials.Username);

        var response = new LoginResponse()
        {
            SecurityToken = new JwtSecurityTokenHandler().WriteToken(securityToken),
            RefreshToken = refreshToken.Token,
            ExpiresAt = securityToken.ValidTo
        };

        return Results.Ok(response);
    }
}