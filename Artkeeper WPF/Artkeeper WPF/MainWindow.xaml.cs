using Artkeeper.ElementClasses;
using System;
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

        public MainWindow()
        {
            InitializeComponent();
            WindowChangeDetector.StartActiveWindowChangeDetection();

            timerLabel = (Label)FindName("TimerLabel");
            timerButton = (Button)FindName("TimerButton");
            ComboBox windowSelector = (ComboBox)FindName("WindowSelector");

            windowProcessSelector = new WindowProcessSelector(windowSelector);
            timerButton.Click += OnTimerButtonClick;
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
            timer.ResetTime();
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
                    timerLabel.Content = $"{timer.GetTimeElapsed():hh\\:mm\\:ss}";
                });
            }
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
