using Microsoft.Extensions.Caching.Distributed;
using OrderAggregator.Extension;
using OrderAggregator.Services.Interfaces;

namespace OrderAggregator.Services.Cache
{
	public class CacheService(IDistributedCache cache, IDistributedCacheCfg configuration) : ICacheService
	{
		public async Task<T> GetAsync<T>(string key)
		{
#pragma warning disable CS8603 // Possible null reference return.
			var value = await cache.GetStringAsync(key);
			if (!string.IsNullOrEmpty(value))
				return value.Deserialize<T>();
			return default;
#pragma warning restore CS8603 // Possible null reference return.
		}

		public async Task<T> SetAsync<T>(string key, T value, int? expiration = null)
		{
			var absoluteExpiration = expiration ?? configuration.AbsoluteExpiration;
			var absoluteExpirationRelativeToNow = expiration ?? configuration.AbsoluteExpirationRelativeToNow;
			var slidingExpiration = expiration ?? configuration.SlidingExpiration;

			var options = new DistributedCacheEntryOptions
			{
				AbsoluteExpiration = absoluteExpiration.HasValue ? DateTime.Now.AddMinutes(absoluteExpiration.Value) : null,
				AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow.HasValue ? TimeSpan.FromMinutes(absoluteExpirationRelativeToNow.Value) : null,
				SlidingExpiration = slidingExpiration.HasValue ? TimeSpan.FromMinutes(slidingExpiration.Value) : null
			};

#pragma warning disable CS8604 // Possible null reference argument.
			await cache.SetStringAsync(key, value?.Serialize(), options);
#pragma warning restore CS8604 // Possible null reference argument.
			return value;
		}

		public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> getData, int? expiration = null)
		{
			var result = await GetAsync<T>(key);
			if (result is null)
			{
				var task = new Lazy<Task<T>>(getData);
				result = await SetAsync(key, await task.Value, expiration);
			}
			return result;
		}

		public async Task RemoveAsync<T>(string key)
		{
			var result = await GetAsync<T>(key);
			if (result is not null)
			{
				var options = new DistributedCacheEntryOptions
				{
					AbsoluteExpiration = DateTime.Now.AddMilliseconds(1),
					AbsoluteExpirationRelativeToNow = TimeSpan.FromMilliseconds(1),
					SlidingExpiration = TimeSpan.FromMilliseconds(1)
				};
				await cache.SetStringAsync(key, result.Serialize(), options);
			}
		}
	}
}
