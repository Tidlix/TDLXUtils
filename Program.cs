using Microsoft.Extensions.Logging;
using TDLXUtils.Tools;

namespace  TDLXUtils
{
    public class Program
    {
        # pragma warning disable CS8618
        public static Config Config {get; private set;}
        # pragma warning restore CS8618

        public static async Task Main(string[] args)
        {
            Config = ConfigReader.ReadConfig();

            // Configuring Logs
            Enum.TryParse(Config.MinimumLogLevel, true, out LogLevel logLevel);
            LogEngine.SetMinimumLogLevel(logLevel);
            LogEngine.SetDateTimeFormat(Config.DateTimeFormat);

            // Configure DB
            DatabaseEngine.Initialize(Config.DBHost, Config.DBPort, Config.DBName, Config.DBUser, Config.DBPassword);

            // Configure Discord
            await DiscordEngine.CreateDiscordConnectionAsync(Config.DiscordToken, LogLevel.Trace);


            while(true);
        }
    }
}