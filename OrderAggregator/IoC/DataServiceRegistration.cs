using Microsoft.EntityFrameworkCore;
using OrderAggregator.Data;
using OrderAggregator.Services;
using OrderAggregator.Services.HostedServices;
using OrderAggregator.Services.Interfaces;
using OrderAggregator.Services.Repository;

namespace OrderAggregator.IoC
{
    public static class DataServiceRegistration
	{
		public static IServiceCollection AddDataServices(this IServiceCollection services)
		{
			_ = services.AddHostedService<OrderProcessingHostedService>();
			_ = services.AddScoped<IOrderService, OrderService>();
			_ = services.AddScoped<IRepositoryService, InMemoryRepositoryService>();
			_ = services.AddScoped<IOrderProcessingService, OrderProcessingService>();
			_ = services.AddDbContext<RepositoryContext>(options =>
			{
				_ = options.UseInMemoryDatabase("InMemoryDatabase");
			});

			return services;
		}
	}
}
