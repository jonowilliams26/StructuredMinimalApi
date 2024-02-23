using Serilog;

namespace StructuredMinimalApi;

public static class ConfigureServices
{
	public static void AddServices(this WebApplicationBuilder builder)
	{
		builder.Host.UseSerilog((context, configuration) =>
		{
			configuration.ReadFrom.Configuration(context.Configuration);
		});
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();
		builder.Services.AddValidatorsFromAssemblyContaining<Program>();
		builder.Services.AddDatabase(builder.Configuration);
	}

	private static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<AppDbContext>(options =>
		{
			options.UseSqlServer(configuration.GetConnectionString("Default"));
		});
	}
}
