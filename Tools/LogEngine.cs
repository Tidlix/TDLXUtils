using Microsoft.Extensions.Logging;

namespace TDLXUtils.Tools
{
    public static class LogEngine
    {
        public static LogLevel MinimumLogLevel = LogLevel.Information;
        public static string DateTimeFormat = "dd.MM.yyyy HH:mm:ss";
        public static void SetMinimumLogLevel(LogLevel minimumLoglevel)
        {
            MinimumLogLevel = minimumLoglevel;
        }
        public static void SetDateTimeFormat(string dateTimeFormat)
        {
            DateTimeFormat = dateTimeFormat;
        }

        public static void Log(string message, LogLevel logLevel)
        {
            if (logLevel < MinimumLogLevel) return;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"[{DateTime.Now.ToString(DateTimeFormat)} - {logLevel}] ");
            Console.ResetColor();
            Console.WriteLine(message);
        }
    }   
}