using Artkeeper.ElementClasses;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Artkeeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Thread updateThread;
        private Label timerLabel;
        private WindowProcessSelector windowProcessSelector;
        private WindowTimer timer;
        private Button timerButton;
        private bool initialized = false;

        private TimeSpan savedTime;

        public MainWindow()
        {
            InitializeComponent();
            WindowChangeDetector.StartActiveWindowChangeDetection();

            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "data.txt"))
            {
                string data = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "data.txt");
                float savedSeconds = float.Parse(data);
                savedTime = TimeSpan.FromSeconds(savedSeconds);
            }

            timerLabel = (Label)FindName("TimerLabel");
            UpdateTimerText();

            timerButton = (Button)FindName("TimerButton");
            ComboBox windowSelector = (ComboBox)FindName("WindowSelector");

            windowProcessSelector = new WindowProcessSelector(windowSelector);
            timerButton.Click += OnTimerButtonClick;
            windowSelector.SelectionChanged += OnWindowSelectorSelectionChanged;

            Application.Current.Exit += OnApplicationExit;
        }

        private void OnApplicationExit(object sender, ExitEventArgs e)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "data.txt";
            Debug.WriteLine(path);
            File.WriteAllText(path, GetTotalTime().Seconds.ToString());
        }

        private void OnWindowSelectorSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (initialized)
            {
                timer.SetProcessToCheckFor(windowProcessSelector.GetProcess());
            }
        }

        public void OnTimerButtonClick(object sender, RoutedEventArgs e)
        {
            if (!initialized)
            {
                initialized = true;
                timer = new WindowTimer(windowProcessSelector.GetProcess());

                updateThread = new Thread(UpdateLoop);
                updateThread.IsBackground = true;
                updateThread.Start();
            }
            else
            {
                timer.ToggleTimer();
            }
            
            timerButton.Content = timer.GetTimerState() ? "Stop" : "Start";
        }

        private void ResetTime_Click(object sender, RoutedEventArgs e)
        {
            savedTime = TimeSpan.Zero;
         
            if (timer != null)
            {
                timer.ResetTime();
            }

            UpdateTimerText();
        }

        private void UpdateLoop()
        {
            while (true)
            {
                Thread.Sleep(100);

                if (timer != null)
                {
                    UpdateTimerText();
                }
            }
        }

        public void UpdateTimerText()
        {
            if (!IsShuttingDown() && timerLabel.Dispatcher != null)
            {
                timerLabel.Dispatcher.Invoke(() =>
                {
                    timerLabel.Content = $"{GetTotalTime():hh\\:mm\\:ss}";
                });
            }
        }

        private TimeSpan GetTotalTime()
        {
            if (timer == null)
            {
                return savedTime;
            }

            return savedTime + timer.GetTimeElapsed();
        }

        private bool IsShuttingDown()
        {
            if (Application.Current == null)
            {
                return true;
            }

            bool shuttingDown = false;

            Application.Current.Dispatcher.Invoke(() =>
            {
                shuttingDown = Application.Current.ShutdownMode == ShutdownMode.OnExplicitShutdown;
            });

            return shuttingDown;
        }
    }
}
