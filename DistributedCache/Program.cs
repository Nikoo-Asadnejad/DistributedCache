using DistributedCache.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
AppConfigurator.ConfigureDI(builder.Services);
AppConfigurator.ConfigureGrpcServices(builder.Services);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

var app = builder.Build();
// Configure the HTTP request pipeline.
AppConfigurator.ConfigurePipeline(app);
app.Run();