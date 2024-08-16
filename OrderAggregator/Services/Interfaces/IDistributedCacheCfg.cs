namespace OrderAggregator.Services.Interfaces
{
	public interface IDistributedCacheCfg
	{
		int? AbsoluteExpiration { get; set; }
		int? AbsoluteExpirationRelativeToNow { get; set; }
		int? SlidingExpiration { get; set; }
	}
}
