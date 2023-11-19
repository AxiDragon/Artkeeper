using System.Diagnostics;

internal class WindowTimer
{
    private bool runThread = false;
    private Stopwatch stopwatch = new Stopwatch();
    private Thread stopwatchThread;
    private Process processToCheckFor;

    public WindowTimer()
    {
        processToCheckFor = GetProcessToCheckFor();
        Console.WriteLine("Tracking for: " + processToCheckFor.MainWindowTitle);

        WindowChangeDetector.OnWindowProcessChanged += OnWindowProcessChanged;
    }

    private void OnWindowProcessChanged(Process windowProcess)
    {
        Console.WriteLine("New Process: " + windowProcess);

        if (windowProcess.Modules[0].FileName == processToCheckFor.Modules[0].FileName)
        {
            StartTimer();
        }
        else
        {
            Console.WriteLine("Stopping timer");
            StopTimer();
        }
    }

    private void StopTimer()
    {
        runThread = false;
        stopwatch.Stop();
    }

    private void StartTimer()
    {
        if (stopwatchThread != null && stopwatchThread.IsAlive)
        {
            return;
        }

        stopwatchThread = new Thread(RunStopwatch);

        stopwatchThread.Start();
    }

    private void RunStopwatch()
    {
        runThread = true;
        stopwatch.Start();

        while (runThread)
        {
            Console.WriteLine("Time spent on window: " + stopwatch.Elapsed);

            Thread.Sleep(100);
        }
    }

    private Process GetProcessToCheckFor()
    {
        Console.WriteLine("Select the window you want to track: ");
        List<Process> processWindows = GetProcessWindows();

        for (int i = 0; i < processWindows.Count; i++)
        {
            Console.WriteLine(i + ": " + processWindows[i].MainWindowTitle);
        }

        Process? processToCheckFor = null;

        do
        {
            Console.WriteLine("Enter the number of the window you want to track: ");
            string? input = Console.ReadLine();

            if (input == null)
            {
                Console.WriteLine("Invalid input! Please enter a number between " + 0 + " and " + (processWindows.Count - 1));
                continue;
            }

            bool gotIndex = int.TryParse(input, out int index);

            if (gotIndex && index >= 0 && index < processWindows.Count)
            {
                return processWindows[index];
            }

            Console.WriteLine("Invalid input! Please enter a number between " + 0 + " and " + (processWindows.Count - 1));
        } while (processToCheckFor == null);

        return processToCheckFor;
    }

    private List<Process> GetProcessWindows()
    {
        List<Process> windowProcesses = new List<Process>();

        Process[] processList = Process.GetProcesses();

        foreach (Process process in processList)
        {
            if (!string.IsNullOrEmpty(process.MainWindowTitle))
            {
                //this process has a window
                windowProcesses.Add(process);
            }
        }

        return windowProcesses;
    }
}