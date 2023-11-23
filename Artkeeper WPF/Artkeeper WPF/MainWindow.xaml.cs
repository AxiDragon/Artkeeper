using Artkeeper.ElementClasses;
using System.Diagnostics;
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

        public MainWindow()
        {
            InitializeComponent();
            WindowChangeDetector.StartActiveWindowChangeDetection();

            timerLabel = (Label)FindName("TimerLabel");
            ComboBox windowSelector = (ComboBox)FindName("WindowSelector");

            windowProcessSelector = new WindowProcessSelector(windowSelector);
        }

        public void TimerButton_ClickStart(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Start button clicked");

            timer = new WindowTimer(windowProcessSelector.GetProcess());

            updateThread = new Thread(UpdateLoop);
            updateThread.Start();
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
                    timerLabel.Content = $"Time elapsed: {timer.GetTimeElapsed()}";
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
