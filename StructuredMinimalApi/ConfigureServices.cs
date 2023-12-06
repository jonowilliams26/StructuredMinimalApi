using Serilog;
using StructuredMinimalApi.Endpoints;

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
		builder.Services.AddEndpoints();
		builder.Services.AddValidatorsFromAssemblyContaining<Program>();
		builder.Services.AddDatabase(builder.Configuration);
	}

	private static void AddEndpoints(this IServiceCollection services)
	{
		services.Scan(scan => scan
			.FromAssemblyOf<Program>()
			.AddClasses(classes => classes.AssignableTo<IEndpoint>())
			.AsImplementedInterfaces()
			.WithTransientLifetime());
	}

	private static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<AppDbContext>(options =>
		{
			options.UseSqlServer(configuration.GetConnectionString("Default"));
		});
	}
}
