using OrderAggregator.IoC;
using OrderAggregator.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services
	.AddCommonServices()
	.AddDataServices()
	.AddCacheServices(new OrderAggregator.Services.Cache.DistributedCacheCfg());

#if DEBUG
builder.Services.AddDebugServices();
#endif

var app = builder.Build();

app.AddSwagger();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<ExceptionMiddleware>();

app.Run();
