namespace TDLXUtils.Tools
{
    public class Config
    {
        /*
        MinimumLogLevel=Information
        DateTimeFormat=dd.MM.yyyy HH:mm:ss
        DiscordToken=Token
        GeminiKey=Key
        DBHost=IP
        DBPort=Port
        DBName=Database
        DBUser=Username
        DBPassword=Pa$$w0rd
        */
        public required string MinimumLogLevel { get; set; }
        public required string DateTimeFormat { get; set; }
        public required string DiscordToken { get; set; }
        public required string GeminiKey { get; set; }
        public required string DBHost { get; set; }
        public required string DBPort { get; set; }
        public required string DBName { get; set; }
        public required string DBUser { get; set; }
        public required string DBPassword { get; set; }
    }   
    public static class ConfigReader
    {
        public static Config ReadConfig()
        {
            string filePath = $"{AppDomain.CurrentDomain.BaseDirectory}TDLXUtils.conf";
            Dictionary<string, string> confFile = new ();
            using (StreamReader sr = new StreamReader(filePath))
            {
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("# "))
                        continue;

                    var parts = line.Split('=', 2);
                    if (parts.Length != 2)
                        throw new Exception($"Invalid body for config line \"{line}\"");

                    string key = parts[0].Trim();
                    string value = parts[1].Trim();
                    confFile.Add(key, value);
                }
            }
            return new Config()
            {
                MinimumLogLevel = confFile["MinimumLogLevel"],
                DateTimeFormat = confFile["DateTimeFormat"],
                DiscordToken = confFile["DiscordToken"],
                GeminiKey = confFile["GeminiKey"],
                DBHost = confFile["DBHost"],
                DBPort = confFile["DBPort"],
                DBName = confFile["DBName"],
                DBUser = confFile["DBUser"],
                DBPassword = confFile["DBPassword"],
            };
        }
    }
}
