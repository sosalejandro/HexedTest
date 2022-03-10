

namespace HexedTest.Library.Api.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds controllers and endpoints
    /// </summary>
    /// <param name="services"></param>
    public static void SetupControllers(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
    }

    /// <summary>
    /// Register infrastructure services
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="connectionId">Database configuration string</param>
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration, string connectionId)
    {
        services.AddHealthChecks()
            .AddCosmosDbCheck(configuration);

        services.AddDbContext<LibraryContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString(connectionId));
        });

        services.AddSingleton<IAggregateStore<Domain.Entities.Library>, LibraryAggregateStore>();
        services.AddScoped<LibraryApplicationService>();
    }
    /// <summary>
    /// Generates Swagger Docs
    /// </summary>
    /// <param name="services"></param>

    public static void RegisterSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "HexedTest.Library.Api", Version = "v1" });
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
    }
}



