namespace XDebugTogglerWindows.Config
{
    public class ConfigManager : IConfigManager
    {
        private string iniPath = Application.StartupPath + "\\config.ini";
        private IniParser iniParser;
        public ConfigManager()
        {
            this.iniParser = new IniParser(iniPath);
        }

        public void WriteToConfigFile(string value, string key)
        {
            if (!File.Exists(iniPath))
            {
                File.Create(iniPath).Close();
            }

            iniParser.Write(key, value, "Config");
        }

        public string ReadFromConfigFile(string key)
        {
            if (!File.Exists(iniPath))
            {
                return "";
            }

            return iniParser.Read(key, "Config");
        }
    }
}
