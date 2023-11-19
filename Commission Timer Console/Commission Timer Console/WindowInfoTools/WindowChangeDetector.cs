﻿using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

//TODO: makes this static
internal static class WindowChangeDetector
{
    private const int nChars = 256;
    private static bool active = false;

    private static Process currentWindowProcess;

    public static Process CurrentWindowProcess
    {
        get => currentWindowProcess;
        set
        {
            if (currentWindowProcess == null)
            {
                //first assignment
                currentWindowProcess = value;
                OnWindowProcessChanged?.Invoke(value);
                return;
            }

            if (currentWindowProcess.Id != value.Id)
            {
                currentWindowProcess = value;
                OnWindowProcessChanged?.Invoke(value);
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
        while (active)
        {
            IntPtr handle = (IntPtr)GetForegroundWindow();

            if (GetWindowThreadProcessId(handle, out uint processId) != 0)
            {
                CurrentWindowProcess = Process.GetProcessById((int)processId);
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
