using System.Diagnostics;
using System.IO;

namespace Artkeeper.Extensions
{
    internal static class ProcessExtensions
    {
        public static string GetProcessInfoString(this Process process, bool trimExe = false)
        {
            return process.MainWindowTitle + " (" + GetProcessFileName(process, trimExe) + ")";
        }

        public static string GetProcessFileName(this Process process, bool trimExe = false)
        {
            string fileName = Path.GetFileName(process.MainModule?.FileName);

            if (trimExe && !string.IsNullOrEmpty(fileName))
            {
                fileName = fileName.Replace(".exe", "");
            }

            return fileName;
        }
    }
}
