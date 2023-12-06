using Serilog;
using StructuredMinimalApi.Endpoints;

namespace StructuredMinimalApi;

public static class ConfigureApp
{
	public static void Configure(this WebApplication app)
	{
		app.UseSerilogRequestLogging();
		app.UseSwagger();
		app.UseSwaggerUI();
		app.UseHttpsRedirection();
		app.MapEndpoints();
	}

	private static void MapEndpoints(this WebApplication app)
	{
		var allEndpointsGroup = app.MapGroup("");
		allEndpointsGroup.WithOpenApi();

		var endpoints = app.Services.GetServices<IEndpoint>();
		foreach (var endpoint in endpoints)
		{
			endpoint.Map(allEndpointsGroup);
		}
	}
}
