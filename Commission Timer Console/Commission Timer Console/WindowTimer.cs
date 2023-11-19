using System;
using System.Diagnostics;

internal class WindowTimer
{
    private Stopwatch stopwatch = new Stopwatch();
    //TODO: get a proper way to identify the window
    //window name can change, multiple windows can have the same class name,
    //and process id changes every time the window is opened
    private string windowClassToCheckFor = string.Empty;

    public WindowTimer()
    {
        string windowTitle = GetWindowTitleToCheckFor();

        WindowChangeDetector.OnWindowTitleChanged += OnWindowTitleChanged;
        //check if the window is already active
        OnWindowTitleChanged(WindowChangeDetector.CurrentWindowName);
    }

    private void OnWindowTitleChanged(string windowTitle)
    {
        Console.WriteLine("New class: " + windowTitle);

        if (windowTitle == windowClassToCheckFor)
        {
            stopwatch.Start();
        }
        else
        {
            stopwatch.Stop();
        }
    }

    private string GetWindowTitleToCheckFor()
    {
        Console.WriteLine("Select the window you want to track: ");
        List<string> windowTitles = GetWindowTitles();

        for (int i = 0; i < windowTitles.Count; i++)
        {
            Console.WriteLine(i + ": " + windowTitles[i]);
        }

        while (true)
        {
            Console.WriteLine("Enter the number of the window you want to track: ");
            string? input = Console.ReadLine();

            if (input == null)
            {
                Console.WriteLine("Invalid input!");
                continue;
            }

            bool gotIndex = int.TryParse(input, out int index);

            if (gotIndex && index >= 0 && index < windowTitles.Count)
            {
                return windowTitles[index];
            }

            Console.WriteLine("Invalid input!");
        }
    }

    private List<string> GetWindowTitles()
    {
        List<string> windowTitles = new List<string>();

        Process[] processList = Process.GetProcesses();

        foreach (Process process in processList)
        {
            if (!string.IsNullOrEmpty(process.MainWindowTitle))
            {
                windowTitles.Add(process.MainWindowTitle);
            }
        }

        return windowTitles;
    }
}