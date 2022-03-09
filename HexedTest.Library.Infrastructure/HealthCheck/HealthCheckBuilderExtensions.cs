
namespace HexedTest.Library.Infrastructure.HealthCheck;

public static class HealthCheckBuilderExtension
{
    public static IHealthChecksBuilder AddCosmosDbCheck(this IHealthChecksBuilder builder, IConfiguration configuration)
    {
        return builder.Add(new HealthCheckRegistration("TripsAgency", new LibraryCosmosDbHealthCheck(configuration), HealthStatus.Unhealthy, null));
    }
}

