namespace OrderAggregator.IoC
{
	public static class SwaggerRegistration
	{
		public static WebApplication AddSwagger(this WebApplication app, string prefix = "openapi")
		{
			_ = app
				.UseSwagger(c =>
				{
					c.RouteTemplate = $"{prefix}/{{documentName}}/swagger.json";
				})
				.UseSwaggerUI(c =>
				{
					c.DocumentTitle = $"{nameof(OrderAggregator)} API documentation";
					c.RoutePrefix = $"{prefix}";
					c.SwaggerEndpoint($"/{prefix}/v1/swagger.json", $"{nameof(OrderAggregator)} API v1");
				});
			return app;
		}
	}
}
