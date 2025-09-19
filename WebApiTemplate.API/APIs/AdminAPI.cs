using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using WebApiTemplate.Application.Iterfaces;
using WebApiTemplate.Domain.Data.TableModels;

namespace WebApiTemplate.API.APIs;

public static class AdminAPI
{
    public static IEndpointRouteBuilder MapAdminAPI(this IEndpointRouteBuilder app)
    {
        var v1 = app.MapGroup("/api/v1/admin")
            .WithMetadata(new ApiVersionAttribute(1));

        v1.MapPost("/all-accounts", GetAllAccounts)
            .WithName("Get all accounts")
            .WithSummary("This endpoint is used to retreive all accounts.")
            .WithDescription("Returns: 200ok")
            .Produces<IEnumerable<Account>>(StatusCodes.Status200OK, "application/json");

        return app;
    }

    private static async Task<IResult> GetAllAccounts(
        [FromServices] IAccountService accountService)
    {
        var accounts = await accountService.GetAllAccounts();

        return Results.Ok(accounts);
    }
}