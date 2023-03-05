using DistributedCache.Core.Interfaces;
using DistributedCache.Infrastructure.Services;
using ProtoBuf.Grpc.Server;

namespace DistributedCache.Configurations;

public static class AppConfigurator
{
    public static void ConfigureDI(IServiceCollection services)
    {
        services.AddScoped<ICacheService, CacheService>();
    }

    public static void ConfigureGrpcServices(IServiceCollection services)
    {
        services.AddGrpc();
        services.AddCodeFirstGrpc();
    }

    public static void ConfigurePipeline(WebApplication app)
    {
        //app.MapGrpcService<GreeterService>();
        app.MapGet("/",
            () =>
                "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
    }
}