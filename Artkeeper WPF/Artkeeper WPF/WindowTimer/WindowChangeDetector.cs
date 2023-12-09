using Artkeeper.StaticClasses;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

internal static class WindowChangeDetector
{
    private static IntPtr currentWindowProcessPtr;

    public static IntPtr CurrentWindowProcessPtr
    {
        get => currentWindowProcessPtr;
        set
        {
            if (currentWindowProcessPtr != value)
            {
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
        CurrentWindowProcessPtr = (IntPtr)GetForegroundWindow();
    }

    public static Process GetCurrentProcess()
    {
        if (GetWindowThreadProcessId(CurrentWindowProcessPtr, out uint processId) != 0)
        {
            return Process.GetProcessById((int)processId);
        }
        else
        {
            return null;
        }
    }

    public static void StartActiveWindowChangeDetection()
    {
        Update.OnUpdate += DetectActiveWindowChange;
    }

    public static void StopActiveWindowChangeDetection()
    {
        Update.OnUpdate -= DetectActiveWindowChange;
    }
}
