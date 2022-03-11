using HexedTest.Library.Domain.Repositories;
using HexedTest.Library.Domain.SeedWork;
using HexedTest.Library.Infrastructure;
using HexedTest.Library.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace HexedTest.Library.Projector;

public class Program
{
    public async static Task Main()
    {
        var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<IAggregateStore<Domain.Entities.Library>, LibraryAggregateStore>();
                services.AddDbContext<LibraryContext>(options =>
                {
                    options.UseSqlServer(hostContext.Configuration.GetConnectionString("Default"));
                });
                services.AddSingleton<ILibraryRepository, LibraryRepository>();
            })
            .Build();

        await host.RunAsync();
    }
}

