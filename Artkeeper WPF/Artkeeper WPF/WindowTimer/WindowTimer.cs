using Artkeeper.Extensions;
using System;
using System.Diagnostics;

internal class WindowTimer
{
    private Stopwatch stopwatch = new Stopwatch();
    private string fileNameToCheckFor;
    public string ProcessInfo { private set; get; }
    private bool active = true;

    public WindowTimer(Process process)
    {
        fileNameToCheckFor = process.Modules[0].FileName;
        ProcessInfo = process.GetProcessInfoString();

        WindowChangeDetector.OnWindowProcessChanged += OnWindowProcessChanged;
        CheckProcess();
    }

    private void OnWindowProcessChanged(Process windowProcess)
    {
        if (!active)
        {
            return;
        }

        if (windowProcess.Modules[0].FileName == fileNameToCheckFor)
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

    public void SetProcessToCheckFor(Process process)
    {
        fileNameToCheckFor = process.Modules[0].FileName;
        ProcessInfo = process.GetProcessInfoString();
        CheckProcess();
    }

    public void SetTimerState(bool active)
    {
        this.active = active;

        if (active)
        {
            CheckProcess();
        }
        else
        {
            stopwatch.Stop();
        }
    }

    private void CheckProcess()
    {
        OnWindowProcessChanged(WindowChangeDetector.GetCurrentProcess());
    }

    public bool GetTimerState() => active;

    public TimeSpan GetTimeElapsed()
    {
        return stopwatch.Elapsed;
    }

    internal void ResetTime()
    {
        stopwatch.Reset();
    }
}