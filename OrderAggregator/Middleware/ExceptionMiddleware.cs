namespace OrderAggregator.Middleware
{
	public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
	{
		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await next(context);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "An unexpected error occurred.");
				await HandleExceptionAsync(context, ex);
			}
		}

		private static Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			var code = StatusCodes.Status500InternalServerError;

			if (exception is ArgumentException) code = StatusCodes.Status400BadRequest;
			else if (exception is UnauthorizedAccessException) code = StatusCodes.Status401Unauthorized;

			context.Response.ContentType = "application/json";
			context.Response.StatusCode = code;

			var result = new
			{
				StatusCode = code,
				Message = "An unexpected error occurred.",
				Details = exception.Message
			};

			return context.Response.WriteAsJsonAsync(result);
		}
	}
}
