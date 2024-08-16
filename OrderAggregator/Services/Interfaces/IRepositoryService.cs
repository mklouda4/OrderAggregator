using OrderAggregator.Data.Dao;
using System.Linq.Expressions;

namespace OrderAggregator.Services.Interfaces
{
	public interface IRepositoryService
	{
		Task Commit();

		Task<List<ProductDao>> GetProductsListAsync(Expression<Func<ProductDao, bool>>? filter = null);
		Task<List<AggregatedOrderDao>> GetAggregatedOrdersListAsync(Expression<Func<AggregatedOrderDao, bool>>? filter = null);
		Task<List<AggregatedOrderDao>> GetAggregatedOrdersForCoreSystem();

		Task<string> AddOrUpdateAggregatedOrder(AggregatedOrderDao order);
	}
}
