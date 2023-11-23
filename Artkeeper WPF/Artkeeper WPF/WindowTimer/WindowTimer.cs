using Artkeeper.Extensions;
using System;
using System.Diagnostics;

internal class WindowTimer
{
    private Stopwatch stopwatch = new Stopwatch();
    private Process processToCheckFor;
    public string ProcessInfo { private set; get; }

    public WindowTimer(Process process)
    {
        processToCheckFor = process;
        ProcessInfo = process.GetProcessInfoString();

        WindowChangeDetector.OnWindowProcessChanged += OnWindowProcessChanged;
    }

    private void OnWindowProcessChanged(Process windowProcess)
    {
        if (windowProcess.Modules[0].FileName == processToCheckFor.Modules[0].FileName)
        {
            stopwatch.Start();
        }
        else
        {
            stopwatch.Stop();
        }
    }

    public TimeSpan GetTimeElapsed()
    {
        return stopwatch.Elapsed;
    }
}