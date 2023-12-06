global using FluentValidation;
global using Microsoft.EntityFrameworkCore;
global using OneOf;
global using StructuredMinimalApi.DataTypes;
global using StructuredMinimalApi.Database;
global using System.Security.Claims;
using Serilog;
using StructuredMinimalApi;

Log.Logger = new LoggerConfiguration()
	.WriteTo.Console()
	.CreateLogger();

try
{
	Log.Information("Starting application...");
	var builder = WebApplication.CreateBuilder(args);
	Log.Information("Environment: {Environment}", builder.Environment.EnvironmentName);
	builder.AddServices();
	var app = builder.Build();
	app.Configure();
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