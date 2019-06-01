using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;


namespace farmer
{
    class IniFile   // revision 11
    {

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public static void Write(string filePath, string Section, string Key, string Value)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            if (!File.Exists(filePath))
                using (File.Create(filePath)) { };
            WritePrivateProfileString(Section, Key, Value, filePath);
        }
        public static string Read(string filePath, string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, filePath);
            return temp.ToString();
        }
    }
}
