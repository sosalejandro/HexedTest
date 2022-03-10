namespace HexedTest.Library.Api.Extensions;

public static class HostExtensions
{
    public static void SetupSerilog(this IHostBuilder host)
    {
        host.UseSerilog();
    }
}

