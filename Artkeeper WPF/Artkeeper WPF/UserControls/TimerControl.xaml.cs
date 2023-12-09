using Artkeeper.ElementClasses;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Artkeeper.UserControls
{
    public partial class TimerControl : UserControl
    {
        private Thread updateThread;
        private Label timerLabel;
        private WindowProcessSelector windowProcessSelector;
        private WindowTimer timer;
        private Button timerButton;
        private bool initialized = false;

        private TimeSpan savedTime;

        public TimerControl()
        {
            InitializeComponent();

            timerLabel = (Label)FindName("TimerLabel");

            UpdateTimerText();

            ComboBox windowSelector = (ComboBox)FindName("WindowSelector");

            timerButton = (Button)FindName("TimerButton");
            timerButton.Click += OnTimerButtonClick;

            windowProcessSelector = new WindowProcessSelector(windowSelector);
            windowSelector.SelectionChanged += OnWindowSelectorSelectionChanged;
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
            MessageBoxResult result = MessageBox.Show("Are you sure you want to reset the time?", "Reset Time", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                savedTime = TimeSpan.Zero;

                if (timer != null)
                {
                    timer.ResetTime();
                }

                UpdateTimerText();
            }
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