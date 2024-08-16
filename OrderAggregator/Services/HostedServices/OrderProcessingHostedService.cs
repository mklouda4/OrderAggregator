using OrderAggregator.Services.Interfaces;

namespace OrderAggregator.Services.HostedServices
{
    public class OrderProcessingHostedService(IServiceScopeFactory factory) : IHostedService, IDisposable
    {
        private Timer? _timer;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(async _ =>
            {
                using var scope = factory.CreateScope();
                var service = scope.ServiceProvider.GetService<IOrderProcessingService>();
                if (service != null)
                {
                    await service.SendOrdersAsync();
                }
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(20));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _ = (_timer?.Change(Timeout.Infinite, 0));
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
