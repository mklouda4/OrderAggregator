using Microsoft.EntityFrameworkCore;
using OrderAggregator.Data.Dao;
using OrderAggregator.Models;
using OrderAggregator.Services.Interfaces;

namespace OrderAggregator.Services
{
	public class OrderService(IRepositoryService repository) : IOrderService
	{
		public async Task CreateOrders(IEnumerable<Order> orders)
		{
			var groupedOrders = orders
				.GroupBy(x => x.ProductId)
				.Select(x => new Order() { ProductId = x.Key, Quantity = x.Sum(o => o.Quantity) });
			await AggregateOrdersInBatchesAsync(groupedOrders, 100);
		}

		private async Task AggregateOrdersInBatchesAsync(IEnumerable<Order> orders, int batchSize)
		{
			var ordersList = orders.ToList();
			int totalOrders = ordersList.Count;
			int numberOfBatches = (int)Math.Ceiling((double)totalOrders / batchSize);

			for (int batchIndex = 0; batchIndex < numberOfBatches; batchIndex++)
			{
				var batch = ordersList.Skip(batchIndex * batchSize).Take(batchSize).ToList();
				var productIds = batch.Select(o => o.ProductId).Distinct().ToList();

				var products = (await repository.GetProductsListAsync(p => productIds.Contains(p.Code)))
					.ToDictionary(p => p.Code);

				var existingAggregatesDict = (await repository.GetAggregatedOrdersListAsync(o => productIds.Contains(o.Product.Code)))
					.ToDictionary(o => o.Product.Code);

				foreach (var order in batch)
				{
					var retries = 3;
					while (retries > 0)
					{
						try
						{
							if (existingAggregatesDict.TryGetValue(order.ProductId, out var aggregatedOrder))
							{
								aggregatedOrder.Quantity += order.Quantity;
								_ = await repository.AddOrUpdateAggregatedOrder(aggregatedOrder);
							}
							else
							{
								if (products.TryGetValue(order.ProductId, out var product))
								{
									aggregatedOrder = new AggregatedOrderDao
									{
										Product = product,
										Quantity = order.Quantity
									};
									_ = await repository.AddOrUpdateAggregatedOrder(aggregatedOrder);
								}
								else
								{
									throw new ArgumentException($"Product with ID {order.ProductId} not found");
								}
							}

							await repository.Commit();

							break;
						}
						catch (DbUpdateConcurrencyException)
						{
							// Handle concurrency issue by retrying
							retries--;
							if (retries == 0)
							{
								throw; // Rethrow if retries exhausted
							}
						}
					}
				}
			}
		}

		public async Task<IEnumerable<Product>> GetProducts()
		{
			var list = await repository.GetProductsListAsync();
			return list.Select(x => new Product() { Code = x.Code, Name = x.Name });
		}
	}
}
