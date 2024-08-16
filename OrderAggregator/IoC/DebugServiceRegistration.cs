using Microsoft.AspNetCore.Mvc;
using OrderAggregator.Services.HostedServices;

namespace OrderAggregator.IoC
{
    public static class DebugServiceRegistration
	{
		public static IServiceCollection AddDebugServices(this IServiceCollection services)
		{
			_ = services.AddHostedService<DebugHostedService>();
			_ = services.AddMvc()
				.ConfigureApiBehaviorOptions(options =>
				{
					options.InvalidModelStateResponseFactory = context =>
					{
						if (context?.ModelState?.IsValid != true)
						{
							System.Diagnostics.Debug.WriteLine("ModelState is not valid");
							System.Diagnostics.Debug.WriteLine("Invalid properties:");
							foreach (var key in context?.ModelState?.Keys ?? new())
							{
								var entry = context?.ModelState[key];
								if (entry?.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
								{
									foreach (var error in entry.Errors)
									{
										System.Diagnostics.Debug.WriteLine($"{key} : {error.ErrorMessage}");
									}
								}

							}
						}
						return new BadRequestResult();
					};
				});

			return services;
		}
	}
}
