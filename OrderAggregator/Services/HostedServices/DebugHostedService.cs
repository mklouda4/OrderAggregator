using OrderAggregator.Data;
using OrderAggregator.Data.Dao;

namespace OrderAggregator.Services.HostedServices
{
	public class DebugHostedService(IServiceScopeFactory factory) : IHostedService, IDisposable
	{
		public async Task StartAsync(CancellationToken cancellationToken)
		{
			using var scope = factory.CreateScope();
			var repository = scope.ServiceProvider.GetService<RepositoryContext>();
			if (repository != null)
			{
				await SeedData(repository);
				_ = await repository.SaveChangesAsync(cancellationToken);
			}
		}

		private static async Task SeedData(RepositoryContext repository)
		{
			await repository.Products.AddRangeAsync(
				new ProductDao { Id = "1", Code = "A", Name = "Product A" },
				new ProductDao { Id = "2", Code = "B", Name = "Product B" },
				new ProductDao { Id = "3", Code = "C", Name = "Product C" }
			);
		}

		public Task StopAsync(CancellationToken cancellationToken)
			=> Task.CompletedTask;

		public void Dispose() 
		{
			GC.SuppressFinalize(this);
		}
	}
}
