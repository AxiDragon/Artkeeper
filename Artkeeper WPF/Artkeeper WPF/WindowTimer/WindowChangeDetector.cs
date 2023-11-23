using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

internal static class WindowChangeDetector
{
    private static Thread detectChangeThread;
    private static bool runThread = false;

    private static IntPtr currentWindowProcessPtr;

    public static IntPtr CurrentWindowProcessPtr
    {
        get => currentWindowProcessPtr;
        set
        {
            if (currentWindowProcessPtr != value)
            {
                Debug.WriteLine("Window changed");
                currentWindowProcessPtr = value;

                if (GetWindowThreadProcessId(value, out uint processId) != 0)
                {
                    OnWindowProcessChanged?.Invoke(Process.GetProcessById((int)processId));
                }
            }
        }
    }

    public static event Action<Process> OnWindowProcessChanged = delegate { };

    [DllImport("user32.dll")]
    public static extern int GetForegroundWindow();

    [DllImport("user32.dll")]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    private static void DetectActiveWindowChange()
    {
        while (runThread)
        {
            CurrentWindowProcessPtr = (IntPtr)GetForegroundWindow();

            Thread.Sleep(100);
        }
    }

    public static void StartActiveWindowChangeDetection()
    {
        if (detectChangeThread != null && detectChangeThread.IsAlive)
        {
            return;
        }

        runThread = true;

        detectChangeThread = new Thread(DetectActiveWindowChange);
        detectChangeThread.IsBackground = true;
        detectChangeThread.Start();
    }

    public static void StopActiveWindowChangeDetection()
    {
        runThread = false;
    }
}
