using Microsoft.OpenApi.Models;

namespace OrderAggregator.IoC
{
	public static class CommonRegistration
	{
		public static IServiceCollection AddCommonServices(this IServiceCollection services)
		{
			_ = services.AddLogging(config => {
				config.AddConsole();
				config.AddDebug();
			});
			
			_ = services.AddControllers();
			_ = services.AddEndpointsApiExplorer();
			_ = services.AddResponseCompression(options =>
			 {
				 options.EnableForHttps = true;
			 });
			_ = services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "Order API", Version = "v1" });

				var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				c.IncludeXmlComments(xmlPath);
			});

			return services;
		}
	}
}
