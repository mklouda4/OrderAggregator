using System.ComponentModel.DataAnnotations;

namespace OrderAggregator.Models
{
	public class Order
	{
		/// <summary>
		/// Product code
		/// </summary>
		[Required(ErrorMessage = "ProductId is required.")]
		[StringLength(100, MinimumLength = 1, ErrorMessage = "ProductId must be between 1 and 100 characters.")]
		public required string ProductId { get; set; }

		/// <summary>
		/// Quantity
		/// </summary>
		[Required(ErrorMessage = "Quantity is required.")]
		[Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive value.")]
		public required int Quantity { get; set; }
	}
}
