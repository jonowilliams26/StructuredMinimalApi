global using FluentValidation;
global using Microsoft.AspNetCore.Http.HttpResults;
global using Microsoft.EntityFrameworkCore;
global using StructuredMinimalApi.Database;
global using StructuredMinimalApi.Api;
global using StructuredMinimalApi.Types;
using Serilog;
using StructuredMinimalApi;

Log.Logger = new LoggerConfiguration()
	.WriteTo.Console()
	.CreateBootstrapLogger();

try
{
	Log.Information("Starting application...");
	var builder = WebApplication.CreateBuilder(args);
	Log.Information("Environment: {Environment}", builder.Environment.EnvironmentName);
	builder.AddServices();
	var app = builder.Build();
	await app.Configure();
	app.Run();
}
catch (Exception ex)
{
	Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
	Log.CloseAndFlush();
}