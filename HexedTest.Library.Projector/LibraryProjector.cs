using HexedTest.Library.Domain.Repositories;
using HexedTest.Library.Domain.SeedWork;
using HexedTest.Library.Infrastructure.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HexedTest.Library.Projector;

public class LibraryProjector
{
    private readonly IConfiguration configuration;
    private readonly IAggregateStore<Domain.Entities.Library> libraryAggregateStore;
    private readonly ILibraryRepository libraryRepository;
    public LibraryProjector(IConfiguration configuration, IAggregateStore<Domain.Entities.Library> libraryAggregateStore, ILibraryRepository libraryRepository)
    {
        this.configuration = configuration;
        this.libraryAggregateStore = libraryAggregateStore;
        this.libraryRepository = libraryRepository;
    }

    [Function("LibraryProjector")]
    public async Task Run([CosmosDBTrigger(
            databaseName: "HexedTest",
            collectionName: "Library",
            ConnectionStringSetting = "CosmosDbConnectionString",
            CreateLeaseCollectionIfNotExists = true,
            LeaseCollectionName = "leases")] IReadOnlyList<CosmosEvent> input, FunctionContext context)

    {
        var logger = context.GetLogger("LibraryProjector");
        if (input == null || !input.Any())
        {
            return;
        }

        logger.LogInformation("Items received: " + input.Count);

        var library = await libraryAggregateStore.LoadAsync();

        await libraryRepository.AddLibrary(library, new System.Threading.CancellationToken());

        logger.LogInformation("Projection done");
    }
}

