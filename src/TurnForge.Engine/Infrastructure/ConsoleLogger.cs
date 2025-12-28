using System;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Core.Logging;

namespace TurnForge.Engine.Infrastructure;

public sealed class ConsoleLogger : IGameLogger
{
    public LogLevel MinimumLevel { get; set; } = LogLevel.Info;

    public void Log(LogLevel level, string message, LogContext? context = null)
    {
        if (level < MinimumLevel) return;

        var levelTag = level switch
        {
            LogLevel.Debug => "[DEBUG]",
            LogLevel.Info => "[INFO]",
            LogLevel.Warning => "[WARN]",
            LogLevel.Error => "[ERROR]",
            _ => "[LOG]"
        };

        var contextStr = context?.ToString() ?? "";
        var separator = string.IsNullOrEmpty(contextStr) ? "" : " ";
        Console.WriteLine($"{levelTag}{separator}{contextStr} {message}");
    }

    public void LogError(string message, Exception? exception = null, LogContext? context = null)
    {
        Log(LogLevel.Error, message, context);
        if (exception != null) Console.WriteLine(exception);
    }
}
