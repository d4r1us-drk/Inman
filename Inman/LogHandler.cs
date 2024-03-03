using System;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Inman;

public class LogHandler<T>
{
    private static readonly LoggingConfiguration _config = CreateConfiguration();

    private readonly Logger _log = LogManager.GetLogger(typeof(T).FullName);

    private static LoggingConfiguration CreateConfiguration()
    {
        var config = new LoggingConfiguration();
        var logfile = new FileTarget("logfile") { FileName = "InvoiceSystem_Logging.log" };
        var logconsole = new ConsoleTarget("logconsole");

        config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
        config.AddRule(LogLevel.Trace, LogLevel.Fatal, logfile);

        return config;
    }

    public LogHandler()
    {
        LogManager.Configuration = _config;
    }

    public void LogInfo(string message)
    {
        _log.Info(message);
    }

    public void LogDebug(string message)
    {
        _log.Debug(message);
    }

    public void LogError(string message, Exception ex)
    {
        _log.Error(ex, message);
    }

    public void LogWarn(string message, Exception ex)
    {
        _log.Warn(ex, message);
    }

    public void LogFatal(string message, Exception ex)
    {
        _log.Fatal(ex, message);
    }
}