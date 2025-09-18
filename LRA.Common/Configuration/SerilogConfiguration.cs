using Serilog;
using Serilog.Events;

namespace LRA.Common.Configuration;

public class SerilogConfiguration
{
    public required string LogFilePath { get; set; } 
    public LogEventLevel MinimumLevel { get; set; }
    public required string ConsoleOutputTemplate { get; set; }
    public RollingInterval RollingInterval { get; set; }
    public required string ServiceName { get; set; } 
}
