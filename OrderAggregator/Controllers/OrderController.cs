using Microsoft.AspNetCore.Mvc;
using OrderAggregator.Data.Dao;
using OrderAggregator.Models;
using OrderAggregator.Services.Interfaces;
using System.Net;

namespace OrderAggregator.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class OrderController(ILogger<OrderController> logger, IOrderService orderService) : ControllerBase
	{
		/// <summary>
		/// Accepts one or more orders.
		/// </summary>
		/// <param name="orders">A list of orders to be added.</param>
		/// <returns>Returns HTTP status code 200 (OK) if the orders are successfully accepted.</returns>
		/// <response code="200">The orders were successfully added.</response>
		/// <response code="400">Invalid order format or other validation issue.</response>
		/// <response code="500">Internal error when technical problems.</response>
		[HttpPost("create")]
		[ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
		[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
		public async Task<IActionResult> CreateOrders([FromBody] IEnumerable<Order> orders)
		{
			if(!ModelState.IsValid) return BadRequest(ModelState);
			try
			{
				await orderService.CreateOrders(orders);
				return Ok();
			}
			catch (ArgumentException ex)
			{
				return StatusCode(StatusCodes.Status400BadRequest, new ValidationProblemDetails
				{
					Detail = ex.Message,
					Status = StatusCodes.Status400BadRequest
				});
			}
			catch (Exception ex)
			{
				logger.LogError(ex, ex.Message);
				return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
				{
					Detail = $"Your order could not be processed. Please contact out support service.",
					Status = StatusCodes.Status500InternalServerError,
					Title = "Internal Server Error"
				});
			}
		}

		/// <summary>
		/// Return list or avalaible products
		/// </summary>
		/// <returns>Returns HTTP status code 200 (OK) if the products are successfully loaded.</returns>
		/// <response code="200">The products were successfully loaded.</response>
		/// <response code="500">Internal error when technical problems.</response>
		[HttpGet("products")]
		[ProducesResponseType(typeof(IEnumerable<ProductDao>), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
		public async Task<ActionResult<IEnumerable<ProductDao>>> ProductList()
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			try
			{
				var result = await orderService.GetProducts();
				return Ok(result);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, ex.Message);
				return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
				{
					Detail = $"Your request could not be processed. Please contact out support service.",
					Status = StatusCodes.Status500InternalServerError,
					Title = "Internal Server Error"
				});
			}
		}
	}
}
