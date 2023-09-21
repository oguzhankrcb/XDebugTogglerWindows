using System.Runtime.InteropServices;
using System.Text;

namespace XDebugTogglerWindows
{
    public class IniParser
    {
        string Path;

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string Section, string Key, string? Value, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string? Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        public IniParser(string? IniPath = null)
        {
            Path = IniPath ?? Application.StartupPath + "\\config.ini";
        }

        public string Read(string Key, string? Section = null, string Default = "")
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(Section, Key, Default, RetVal, 255, Path);
            return RetVal.ToString();
        }

        public void Write(string Key, string? Value, string Section = "")
        {
            WritePrivateProfileString(Section, Key, Value, Path);
        }

        public void DeleteKey(string Key, string Section = "")
        {
            Write(Key, null, Section);
        }

        public bool KeyExists(string Key, string? Section = null)
        {
            return Read(Key, Section).Length > 0;
        }
    }
}
