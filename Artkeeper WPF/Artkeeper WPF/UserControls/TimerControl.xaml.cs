using Artkeeper.ElementClasses;
using Artkeeper.Extensions;
using Artkeeper.StaticClasses;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Artkeeper.UserControls
{
    public partial class TimerControl : UserControl
    {
        private Label timerLabel;
        private WindowProcessSelector windowProcessSelector;
        private WindowTimer timer;
        private Button timerButton;

        private TimeSpan savedTime = TimeSpan.Zero;

        public Action<TimerControl> OnRemoveRequested;

        public TimerControl()
        {
            InitializeComponent();

            InitializeClass();
        }

        public TimerControl(TimerControlData data)
        {
            InitializeComponent();

            savedTime = data.Time;

            InitializeClass();

            timer.SetProcessToCheckFor(data.ProcessFileName);
            timer.SetTimerState(data.Active);

            UpdateTimerButtonText();

            windowProcessSelector.UpdateText(data.ProcessFileName.GetProcessFileName(true, true));
        }

        private void InitializeClass()
        {
            timerLabel = (Label)FindName("TimerLabel");

            timerButton = (Button)FindName("TimerButton");
            timerButton.Click += OnTimerButtonClick;

            Update.OnUpdate += UpdateTimerText;

            UpdateTimerText();

            ComboBox windowSelector = (ComboBox)FindName("WindowSelector");
            windowProcessSelector = new WindowProcessSelector(windowSelector);

            timer = new WindowTimer(windowProcessSelector.GetProcess());

            windowSelector.SelectionChanged += OnWindowSelectorSelectionChanged;

            UpdateTimerButtonText();
        }

        private void OnWindowSelectorSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            timer.SetProcessToCheckFor(windowProcessSelector.GetProcess());
        }

        public void OnTimerButtonClick(object sender, RoutedEventArgs e)
        {
            timer.ToggleTimer();

            UpdateTimerButtonText();
        }

        private void UpdateTimerButtonText()
        {
            timerButton.Content = timer.GetTimerState() ? "Stop" : "Start";
        }

        private void OnResetTimerClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to reset the timer?", "Reset Timer", MessageBoxButton.YesNo, MessageBoxImage.Question);
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

        private void OnRemoveTimerClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to remove the timer?", "Remove Timer", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                OnRemoveRequested?.Invoke(this);
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

        public TimeSpan GetTotalTime()
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

        public TimerControlData GetData()
        {
            return new TimerControlData(GetTotalTime(), timer.GetProcessFileName(), timer.GetTimerState());
        }
    }

    [Serializable]
    public class TimerControlData
    {
        public TimeSpan Time { get; set; }
        public string ProcessFileName { get; set; }
        public bool Active { get; set; }

        public TimerControlData(TimeSpan time, string processFileName, bool active)
        {
            Time = time;
            ProcessFileName = processFileName;
            Active = active;
        }
    }
}