namespace HexedTest.Library.Api.Extensions;

public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// Setups the Builder methods
    /// </summary>
    /// <param name="builder"></param>
    public static void SetupBuilder(this WebApplicationBuilder builder, string connectionId = "Default", bool test = false)
    {

        
        builder.Logging.SetupLogging();
        builder.Host.SetupSerilog();

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle        
        builder.Services.SetupControllers();
        if (!test)
        {
            builder.Services.RegisterServices(builder.Configuration, connectionId);
        }
        builder.Services.RegisterSwaggerDocumentation();
    }
}
