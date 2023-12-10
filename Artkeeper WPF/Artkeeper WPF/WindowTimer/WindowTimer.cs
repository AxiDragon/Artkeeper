using System;
using System.Diagnostics;

internal class WindowTimer
{
    private Stopwatch stopwatch = new Stopwatch();
    private string fileNameToCheckFor;
    private bool active;

    public WindowTimer(Process process, bool isActive = false)
    {
        active = isActive;
        fileNameToCheckFor = process.Modules[0].FileName;

        WindowChangeDetector.OnWindowProcessChanged += OnWindowProcessChanged;
        CheckProcess();
    }

    public WindowTimer(string fileName, bool isActive = false)
    {
        active = isActive;
        fileNameToCheckFor = fileName;

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
        CheckProcess();
    }

    public void SetProcessToCheckFor(string processFileName)
    {
        fileNameToCheckFor = processFileName;
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

    private void CheckProcess() => OnWindowProcessChanged(WindowChangeDetector.GetCurrentProcess());

    public bool GetTimerState() => active;

    public string GetProcessFileName() => fileNameToCheckFor;

    public TimeSpan GetTimeElapsed() => stopwatch.Elapsed;

    internal void ResetTime() => stopwatch.Reset();
}