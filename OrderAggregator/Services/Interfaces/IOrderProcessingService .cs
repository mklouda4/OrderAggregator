using OrderAggregator.Models;

namespace OrderAggregator.Services.Interfaces
{
    public interface IOrderProcessingService
	{
		Task SendOrdersAsync();
	}
}
