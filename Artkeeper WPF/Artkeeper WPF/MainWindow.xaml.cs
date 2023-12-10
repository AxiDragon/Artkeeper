using Artkeeper.StaticClasses;
using Artkeeper.UserControls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private Label headerLabel;
        private List<TimerControl> timerControls = new List<TimerControl>();

        public MainWindow()
        {
            InitializeComponent();

            StickyHeader stickyHeader = (StickyHeader)FindName("StickyHeader");
            headerLabel = (Label)stickyHeader.FindName("HeaderLabel");

            timerStackPanel = (StackPanel)FindName("TimerStackPanel");

            LoadTimers();

            if (timerControls.Count == 0)
            {
                AddTimer();
            }

            Application.Current.Exit += OnApplicationExit;

            Update.OnUpdate += UpdateHeaderLabel;
            UpdateHeaderLabel();
        }

        private void UpdateHeaderLabel()
        {
            if (!App.IsShuttingDown() && headerLabel.Dispatcher != null)
            {
                headerLabel.Dispatcher.Invoke(() =>
                {
                    headerLabel.Content = $"Artkeeper - Total Time: {GetTotalTime():hh\\:mm\\:ss}";
                });
            }
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
            //this is currently the only thing saving so it's fine to just save everything
            SavingSystem.ClearSaveData();

            Debug.WriteLine("Saving " + timerControls.Count + " timers");

            for (int i = 0; i < timerControls.Count; i++)
            {
                SavingSystem.AddNewData($"Timer{i}", timerControls[i].GetData());
            }

            SavingSystem.Save();
        }


        private TimeSpan GetTotalTime()
        {
            TimeSpan totalTime = TimeSpan.Zero;

            for (int i = 0; i < timerControls.Count; i++)
            {
                totalTime += timerControls[i].GetTotalTime();
            }

            return totalTime;
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

            timerControl.OnRemoveRequested += RemoveTimer;

            return timerControl;
        }

        public TimerControl AddTimer(TimerControlData data)
        {
            TimerControl timerControl = new TimerControl(data);

            int index = timerStackPanel.Children.Count - 1;
            timerStackPanel.Children.Insert(index, timerControl);

            timerControls.Add(timerControl);

            timerControl.OnRemoveRequested += RemoveTimer;

            return timerControl;
        }

        public void RemoveTimer(TimerControl timerControl)
        {
            timerStackPanel.Children.Remove(timerControl);
            timerControls.Remove(timerControl);
        }
    }
}
