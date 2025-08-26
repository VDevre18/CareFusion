// Placeholder for Logging/Logger.cs
using Microsoft.Extensions.Logging;

namespace CareFusion.Common.Logging;

public class Logger<T>
{
    private readonly ILogger<T> _logger;
    public Logger(ILogger<T> logger) => _logger = logger;

    public void Info(string message, params object[] args) => _logger.LogInformation(message, args);
    public void Warn(string message, params object[] args) => _logger.LogWarning(message, args);
    public void Error(Exception ex, string message, params object[] args) => _logger.LogError(ex, message, args);
}
