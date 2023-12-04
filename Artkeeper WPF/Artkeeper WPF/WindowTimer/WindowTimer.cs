using Artkeeper.Extensions;
using System;
using System.Diagnostics;

internal class WindowTimer
{
    private Stopwatch stopwatch = new Stopwatch();
    private Process processToCheckFor;
    public string ProcessInfo { private set; get; }
    private bool active = true;

    public WindowTimer(Process process)
    {
        processToCheckFor = process;
        ProcessInfo = process.GetProcessInfoString();

        WindowChangeDetector.OnWindowProcessChanged += OnWindowProcessChanged;
    }

    private void OnWindowProcessChanged(Process windowProcess)
    {
        if (!active)
        {
            return;
        }

        if (windowProcess.Modules[0].FileName == processToCheckFor.Modules[0].FileName)
        {
            stopwatch.Start();
        }
        else
        {
            stopwatch.Stop();
        }
    }

    public void ToggleTimer()
    {
        SetTimerState(!active);
    }

    public void SetTimerState(bool active)
    {
        this.active = active;

        if (active)
        {
            OnWindowProcessChanged(WindowChangeDetector.GetCurrentProcess());
        }
        else
        {
            stopwatch.Stop();
        }
    }

    public bool GetTimerState()
    {
        return active;
    }

    public TimeSpan GetTimeElapsed()
    {
        return stopwatch.Elapsed;
    }
}