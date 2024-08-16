namespace OrderAggregator.Data.Dao
{
	public class AggregatedOrderDao : BaseDao
	{
		public required ProductDao Product { get; set; }
		public required int Quantity { get; set; }
	}
}
