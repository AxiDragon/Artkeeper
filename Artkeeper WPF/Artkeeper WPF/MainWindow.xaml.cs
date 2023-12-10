using Artkeeper.StaticClasses;
using Artkeeper.UserControls;
using System.Collections.Generic;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace Artkeeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private StackPanel timerStackPanel;
        private List<TimerControl> timerControls = new List<TimerControl>();

        public MainWindow()
        {
            InitializeComponent();

            timerStackPanel = (StackPanel)FindName("TimerStackPanel");

            LoadTimers();

            if (timerControls.Count == 0)
            {
                AddTimer();
            }

            Application.Current.Exit += OnApplicationExit;
        }

        private void LoadTimers()
        {
            Dictionary<string, object> saveData = SavingSystem.SaveData;
            List<TimerControlData> timers = new List<TimerControlData>();

            foreach (KeyValuePair<string, object> pair in saveData)
            {
                if (pair.Key.StartsWith("Timer"))
                {
                    try
                    {
                        TimerControlData timerData = JsonSerializer.Deserialize<TimerControlData>(pair.Value.ToString());
                        timers.Add(timerData);
                    }
                    catch (JsonException)
                    {
                        //ignore
                    }
                }
            }

            for (int i = 0; i < timers.Count; i++)
            {
                AddTimer(timers[i]);
            }
        }

        private void OnApplicationExit(object sender, ExitEventArgs e)
        {
            for (int i = 0; i < timerControls.Count; i++)
            {
                SavingSystem.AddNewData($"Timer{i}", timerControls[i].GetData());
            }

            SavingSystem.Save();
        }

        public void AddTimer_Click(object sender, RoutedEventArgs e)
        {
            AddTimer();
        }

        public TimerControl AddTimer()
        {
            TimerControl timerControl = new TimerControl();

            int index = timerStackPanel.Children.Count - 1;
            timerStackPanel.Children.Insert(index, timerControl);

            timerControls.Add(timerControl);

            return timerControl;
        }

        public TimerControl AddTimer(TimerControlData data)
        {
            TimerControl timerControl = new TimerControl(data);

            int index = timerStackPanel.Children.Count - 1;
            timerStackPanel.Children.Insert(index, timerControl);

            timerControls.Add(timerControl);

            return timerControl;
        }
    }
}
