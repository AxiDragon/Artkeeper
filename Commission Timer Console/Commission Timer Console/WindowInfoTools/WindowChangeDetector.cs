using System.Runtime.InteropServices;
using System.Text;

//TODO: makes this static
internal static class WindowChangeDetector
{
    private const int nChars = 256;
    private static bool active = false;

    private static string currentWindowName = string.Empty;

    public static string CurrentWindowName
    {
        get => currentWindowName;
        set
        {
            if (currentWindowName != value)
            {
                currentWindowName = value;
                OnWindowTitleChanged?.Invoke(value);
            }
        }
    }

    public static event Action<string> OnWindowTitleChanged = delegate { };

    [DllImport("user32.dll")]
    public static extern int GetForegroundWindow();

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    private static void DetectActiveWindowChange()
    {
        while (active)
        {
            IntPtr handle = (IntPtr)GetForegroundWindow();
            StringBuilder className = new StringBuilder(nChars);

            if (GetClassName(handle, className, nChars) > 0)
            {
                CurrentWindowName = className.ToString();
            }

            Thread.Sleep(100);
        }
    }

    public static void StartActiveWindowChangeDetection()
    {
        if (active)
        {
            //already active
            return;
        }

        active = true;
        DetectActiveWindowChange();
    }

    public static void StopActiveWindowChangeDetection()
    {
        active = false;
    }
}
