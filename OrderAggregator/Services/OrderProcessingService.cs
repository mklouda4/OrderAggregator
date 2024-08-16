using OrderAggregator.Data.Dao;
using OrderAggregator.Services.Interfaces;

namespace OrderAggregator.Services
{
	public class OrderProcessingService(ILogger<OrderProcessingService> logger, IRepositoryService repository) : IOrderProcessingService
	{
		public async Task SendOrdersAsync()
		{
			var orders = await repository.GetAggregatedOrdersForCoreSystem();
			if (orders.Count != 0)
			{
				logger.LogInformation($"Processing orders");
				logger.LogInformation($"[ID][PRODUCT_CODE][QUANTITY]");
				foreach (var order in orders)
				{
					var result = await SendOrder(order);
					if (!result)
					{
						await repository.AddOrUpdateAggregatedOrder(order);
					}
				}
			}
			else
			{
				logger.LogInformation($"No orders to process");
			}
		}

		private async Task<bool> SendOrder(AggregatedOrderDao order)
		{
			try
			{
				await Task.Run(() => logger.LogInformation($"[{order.Id}][{order.Product?.Code}][{order.Quantity}]"));
				return true;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"Send order ID {order.Id} to core error: {ex.Message}");
				return false;
			}
		}
	}
}
