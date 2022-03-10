namespace HexedTest.Library.Api.Extensions;

public static class LoggingExtensions
{
    /// <summary>
    /// Adds logging to the solution
    /// </summary>
    /// <param name="logger">ILoggerBuilder</param>
    public static void SetupLogging(this ILoggingBuilder logger)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("Logs/log.txt",
                restrictedToMinimumLevel: LogEventLevel.Information,
                rollingInterval: RollingInterval.Day)
            .WriteTo.File("Logs/error-logs.txt",
                 restrictedToMinimumLevel: LogEventLevel.Warning)
            .CreateLogger();

        logger.ClearProviders();
        logger.AddSerilog(Log.Logger);
    }
}