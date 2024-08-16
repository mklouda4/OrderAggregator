namespace OrderAggregator.Extension
{
	public static class ObjectExtension
	{
		public static T? Deserialize<T>(this string data) => string.IsNullOrEmpty(data) ? default : System.Text.Json.JsonSerializer.Deserialize<T>(data);
		public static string Serialize(this object data) => System.Text.Json.JsonSerializer.Serialize(data);
	}
}
