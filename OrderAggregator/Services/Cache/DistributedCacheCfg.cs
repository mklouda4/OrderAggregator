using OrderAggregator.Services.Interfaces;

namespace OrderAggregator.Services.Cache
{
	public class DistributedCacheCfg : IDistributedCacheCfg
	{
		public int? AbsoluteExpiration { get; set; }
		public int? AbsoluteExpirationRelativeToNow { get; set; } = 60;
		public int? SlidingExpiration { get; set; } = 10;
	}
}
