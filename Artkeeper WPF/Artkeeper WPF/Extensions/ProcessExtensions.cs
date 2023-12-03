using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Artkeeper.Extensions
{
    internal static class ProcessExtensions
    {
        public static string GetProcessInfoString(this Process process, bool trimExe = false, bool capitalizeFirst = false)
        {
            return process.MainWindowTitle + " (" + GetProcessFileName(process, trimExe, capitalizeFirst) + ")";
        }

        public static string GetProcessFileName(this Process process, bool trimExe = false, bool capitalizeFirst = false)
        {
            string fileName = Path.GetFileName(process.MainModule?.FileName);

            if (trimExe && !string.IsNullOrEmpty(fileName))
            {
                fileName = fileName.Replace(".exe", "");
            }

            if (capitalizeFirst && !string.IsNullOrEmpty(fileName))
            {
                if (fileName.Length > 1)
                {
                    fileName = char.ToUpper(fileName[0]) + fileName.Substring(1);
                }
            }

            return fileName;
        }
    }
}
