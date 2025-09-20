using WebApiTemplate.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureAppServices();

var app = builder.Build();

await app.ConfigureAppPipelineAsync();

await app.RunAsync();