using NLog;

public class LoggerService
{
    private static readonly Logger UserLogger = LogManager.GetLogger("UserLogger");
    private static readonly Logger ErrorLogger = LogManager.GetLogger("ErrorLogger");

    public void LogUserAction(string message)
    {
        UserLogger.Info(message);
    }

    public void LogError(string message)
    {
        ErrorLogger.Error(message);
    }
}
