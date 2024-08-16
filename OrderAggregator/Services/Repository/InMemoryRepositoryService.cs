using Microsoft.EntityFrameworkCore;
using OrderAggregator.Data;
using OrderAggregator.Data.Dao;
using OrderAggregator.Services.Interfaces;
using System.Linq.Expressions;

namespace OrderAggregator.Services.Repository
{
	public class InMemoryRepositoryService(RepositoryContext repository, ICacheService cache) : IRepositoryService
	{
		public Task Commit()
			=> repository.SaveChangesAsync();

		public async Task<string> AddOrUpdateAggregatedOrder(AggregatedOrderDao order)
		{
			if (order.Id is null)
			{
				order.Id = Guid.NewGuid().ToString();
				await repository.AddAsync(order);
			}
			else
			{
				repository.Update(order);
			}
			return order.Id;
		}
		public Task<List<AggregatedOrderDao>> GetAggregatedOrdersListAsync(Expression<Func<AggregatedOrderDao, bool>>? filter = null)
		{
			var query = repository.AggregatedOrders
				.Include(i => i.Product);
			if (filter != null)
				_ = query.Where(filter);
			return query.ToListAsync();
		}
		public async Task<List<AggregatedOrderDao>> GetAggregatedOrdersForCoreSystem()
		{
			var ordersToFetch = await repository.AggregatedOrders
				.Where(x => x.Quantity > 0)
				.Include(i => i.Product)
				.ToListAsync();

			var list = ordersToFetch.Select(order => new AggregatedOrderDao
			{
				Id = order.Id,
				Product = order.Product,
				Quantity = order.Quantity
			}).ToList();

			await repository.AggregatedOrders
				.Where(x => x.Quantity > 0)
				.ForEachAsync(x => x.Quantity = 0);
			await repository.SaveChangesAsync();
			return list;
		}

		public async Task<string> AddOrUpdateProduct(ProductDao product)
		{
			if (product.Id is null)
			{
				product.Id = Guid.NewGuid().ToString();
				await repository.AddAsync(product);
			}
			else
			{
				repository.Update(product);
			}
			await cache.RemoveAsync<List<ProductDao>>($"{nameof(ProductDao)}List");
			return product.Id;
		}
		public async Task<List<ProductDao>> GetProductsListAsync(Expression<Func<ProductDao, bool>>? filter = null)
		{
			var set = repository.Products;
			var query = set.AsQueryable();
			if (filter != null)
			{
				query = query.Where(filter);
				return await query.ToListAsync();
			}
			else
			{
				return await cache.GetOrSetAsync($"{nameof(ProductDao)}List", () => query.ToListAsync());
			}
		}
	}
}
