using Microsoft.EntityFrameworkCore;
using OrderAggregator.Data.Dao;

namespace OrderAggregator.Data
{
	public class RepositoryContext(DbContextOptions<RepositoryContext> options) : DbContext(options)
	{
		public DbSet<ProductDao> Products { get; set; }
		public DbSet<AggregatedOrderDao> AggregatedOrders { get; set; }
	}
}
