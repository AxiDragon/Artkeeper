﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

internal static class WindowChangeDetector
{
    private static Thread detectChangeThread;
    private static bool runThread = false;

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
        while (runThread)
        {
            //TODO: just compare intptrs instead of getting process, should be faster
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
