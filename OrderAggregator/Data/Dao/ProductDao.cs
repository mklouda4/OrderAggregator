namespace OrderAggregator.Data.Dao
{
	public class ProductDao : BaseDao
	{
		public required string Code { get; set; }
		public required string Name { get; set; }
	}
}
