using Serilog;

namespace StructuredMinimalApi;

public static class ConfigureApp
{
	public static async Task Configure(this WebApplication app)
	{
		app.UseSerilogRequestLogging();
		app.UseSwagger();
		app.UseSwaggerUI();
		app.UseHttpsRedirection();
		app.MapEndpoints();
		await app.EnsureDatabaseCreated();
	}
	
	private static async Task EnsureDatabaseCreated(this WebApplication app)
	{
		app.Logger.LogInformation("Ensuring database is created...");
        using var scope = app.Services.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await database.Database.MigrateAsync();
		app.Logger.LogInformation("Database created / updated.");
    }
}
