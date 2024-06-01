using AddressLibrary.Data.Context;
using AddressProvider.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddDbContext<DataContext>(x => x.UseSqlServer(Environment.GetEnvironmentVariable("SqlServer")));
        services.AddScoped<AddressService>();
        services.AddScoped<DeleteService>();
        services.AddScoped<GetAddressService>();
        services.AddScoped<GetOneService>();
        services.AddScoped<RemoveAllService>();
        services.AddScoped<UpdateService>();
    })
    .Build();

host.Run();
