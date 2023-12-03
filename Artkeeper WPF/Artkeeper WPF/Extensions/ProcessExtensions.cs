using System.Diagnostics;
using System.IO;

namespace Artkeeper.Extensions
{
    internal static class ProcessExtensions
    {
        public static string GetProcessInfoString(this Process process)
        {
            return process.MainWindowTitle + " (" + Path.GetFileName(process.MainModule?.FileName) + ")";
        }

        public static string GetProcessFileName(this Process process)
        {
            return Path.GetFileName(process.MainModule?.FileName);
        }
    }
}
