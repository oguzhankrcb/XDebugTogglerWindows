namespace XDebugTogglerWindows.Config
{
    public interface IConfigManager
    {
        string ReadFromConfigFile(string key);
        void WriteToConfigFile(string value, string key);
    }
}