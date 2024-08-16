namespace OrderAggregator.Services.Interfaces
{
	public interface ICacheService
	{
		Task<T> GetAsync<T>(string key);
		Task<T> SetAsync<T>(string key, T value, int? expiration = null);
		Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> getData, int? expiration = null);
		Task RemoveAsync<T>(string key);
	}
}
