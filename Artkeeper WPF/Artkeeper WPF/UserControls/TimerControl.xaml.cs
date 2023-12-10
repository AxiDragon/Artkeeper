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

        private int id = 0;

        public TimerControl(int id)
        {
            InitializeComponent();

            this.id = id;

            InitializeClass();
        }

        public TimerControl(TimerControlData data)
        {
            InitializeComponent();

            this.id = data.Id;
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

        public TimerControlData GetData()
        {
            return new TimerControlData(id, GetTotalTime(), timer.GetProcessFileName(), timer.GetTimerState());
        }
    }

    [Serializable]
    public class TimerControlData
    {
        public int Id { get; set; }
        public TimeSpan Time { get; set; }
        public string ProcessFileName { get; set; }
        public bool Active { get; set; }

        public TimerControlData(int id, TimeSpan time, string processFileName, bool active)
        {
            Id = id;
            Time = time;
            ProcessFileName = processFileName;
            Active = active;
        }
    }
}