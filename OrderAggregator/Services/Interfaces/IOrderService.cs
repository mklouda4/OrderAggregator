using OrderAggregator.Data.Dao;
using OrderAggregator.Models;

namespace OrderAggregator.Services.Interfaces
{
	public interface IOrderService
    {
		Task<IEnumerable<Product>> GetProducts();
		Task CreateOrders(IEnumerable<Order> orders);
	}
}
