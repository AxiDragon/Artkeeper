using System.Runtime.InteropServices;
using System.Text;

internal class WindowChangeDetector
{
    //TODO: makes this static
    private const int nChars = 256;
    private bool active = true;

    private string currentWindowName = string.Empty;

    public string CurrentWindowName
    {
        get => currentWindowName;
        set
        {
            if (currentWindowName != value)
            {
                currentWindowName = value;
                OnWindowTitleChanged?.Invoke(value);

                Console.WriteLine("New class: " + CurrentWindowName);
            }
        }
    }

    public event Action<string> OnWindowTitleChanged;

    [DllImport("user32.dll")]
    public static extern int GetForegroundWindow();

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    [DllImport("user32.dll")]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    public WindowChangeDetector(Action<string> action)
    {
        OnWindowTitleChanged += action;

        DetectActiveWindowChange();
    }

    private void DetectActiveWindowChange()
    {
        while (active)
        {
            IntPtr handle = (IntPtr)GetForegroundWindow();
            StringBuilder className = new StringBuilder(nChars);
            StringBuilder windowTitle = new StringBuilder(nChars);

            if (GetClassName(handle, className, nChars) > 0)
            {
                CurrentWindowName = className.ToString();
            }

            Thread.Sleep(1000);
        }
    }
}
