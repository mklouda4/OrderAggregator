using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using OrderAggregator.Data;
using OrderAggregator.Models;
using OrderAggregator.Services;
using OrderAggregator.Services.Cache;
using OrderAggregator.Services.Interfaces;
using OrderAggregator.Services.Repository;
using Xunit.Sdk;

namespace OrderAggregatorTest
{
	[TestClass]
	public class UnitTest
	{
		private OrderService _orderService;
		private IRepositoryService _repository;
		private RepositoryContext _context;
		private ICacheService _cache;

		[TestInitialize]
		public void Setup()
		{
			var options = new DbContextOptionsBuilder<RepositoryContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;

			var memoryDistributedCacheOptions = Options.Create(new MemoryDistributedCacheOptions());
			var memoryDistributedCache = new MemoryDistributedCache(memoryDistributedCacheOptions);

			_cache = new CacheService(memoryDistributedCache, new DistributedCacheCfg());
			_context = new RepositoryContext(options);
			_repository = new InMemoryRepositoryService(_context, _cache);
			_orderService = new OrderService(_repository);

			SeedData();
		}
		[TestCleanup]
		public void Cleanup()
		{
			_context.Database.EnsureDeleted();
		}
		private void SeedData()
		{
			_context.Products.AddRange(
				new() { Id = "1", Code = "A", Name = "Product A" },
				new() { Id = "2", Code = "B", Name = "Product B" },
				new() { Id = "3", Code = "C", Name = "Product C" }
			);

			_ = _context.SaveChanges();
		}

		[TestMethod]
		public async Task AggregateOrdersCorrectly()
		{
			var orders = new List<Order>
			{
				new() { ProductId = "A", Quantity = 10 },
				new() { ProductId = "B", Quantity = 20 },
				new() { ProductId = "A", Quantity = 5 }
			};
			await _orderService.CreateOrders(orders);
			var aggregatedOrders = await _repository.GetAggregatedOrdersListAsync();

			Assert.AreEqual(2, aggregatedOrders.Count);
			Assert.AreEqual(15, aggregatedOrders.First(a => a.Product.Code == "A").Quantity);
			Assert.AreEqual(20, aggregatedOrders.First(a => a.Product.Code == "B").Quantity);
		}
		[TestMethod]
		public async Task AggregateOrdersSendToCore()
		{
			var orders = new List<Order>
			{
				new() { ProductId = "A", Quantity = 10 },
				new() { ProductId = "B", Quantity = 20 },
				new() { ProductId = "A", Quantity = 5 }
			};
			await _orderService.CreateOrders(orders);
			var aggregatedOrders = await _repository.GetAggregatedOrdersForCoreSystem();

			Assert.AreEqual(2, aggregatedOrders.Count);
			Assert.AreEqual(15, aggregatedOrders.First(a => a.Product.Code == "A").Quantity);
			Assert.AreEqual(20, aggregatedOrders.First(a => a.Product.Code == "B").Quantity);

			aggregatedOrders = await _repository.GetAggregatedOrdersListAsync();

			Assert.AreEqual(2, aggregatedOrders.Count);
			Assert.AreEqual(0, aggregatedOrders.First(a => a.Product.Code == "A").Quantity);
			Assert.AreEqual(0, aggregatedOrders.First(a => a.Product.Code == "B").Quantity);

			aggregatedOrders = await _repository.GetAggregatedOrdersForCoreSystem();

			Assert.AreEqual(0, aggregatedOrders.Count);
		}
		[TestMethod]
		public async Task ProductsCacheTest()
		{
			var expectedCount = _context.Products.Count();
			var products = await _repository.GetProductsListAsync();
			Assert.AreEqual(expectedCount, products.Count);
		}
	}
}