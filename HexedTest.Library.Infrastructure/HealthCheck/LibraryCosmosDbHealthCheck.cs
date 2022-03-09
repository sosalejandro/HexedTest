namespace HexedTest.Library.Infrastructure.HealthCheck;
public class LibraryCosmosDbHealthCheck : IHealthCheck
{
    private readonly CosmosClient cosmosClient;
    private readonly IConfiguration configuration;

    public LibraryCosmosDbHealthCheck(IConfiguration configuration)
    {
        this.configuration = configuration;
        cosmosClient = new CosmosClient(configuration["CosmosDb:ConnectionString"]);
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        await cosmosClient.ReadAccountAsync();
        var databaseId = configuration["CosmosDb:DatabaseId"];
        var containerId = configuration["CosmosDb:ContainerId"];
        var container = cosmosClient.GetContainer(databaseId, containerId);
        var containerProperties = await container.ReadContainerAsync(cancellationToken: cancellationToken);
        return containerProperties.StatusCode == System.Net.HttpStatusCode.OK ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
    }
}
