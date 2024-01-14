using Artkeeper.StaticClasses;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Artkeeper.UserControls
{
    public partial class StickyHeader : UserControl
    {
        public StickyHeader()
        {
            InitializeComponent();
        }

        private void OnClickClose(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OnClickMinimize(object sender, RoutedEventArgs e)
        {
            Window mainWindow = Application.Current.MainWindow;

            if (mainWindow != null && mainWindow.WindowState == WindowState.Normal)
            {
                mainWindow.WindowState = WindowState.Minimized;
            }
        }

        private void OnClickReset(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to reset ALL timers?", "Reset Timers", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                ArtkeeperEventManager.EVENT_COLLECTION[EventCollection.ResetAllTimers]();
            }
        }

        private void DragThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            Window mainWindow = Application.Current.MainWindow;

            if (mainWindow != null)
            {
                mainWindow.Left += e.HorizontalChange;
                mainWindow.Top += e.VerticalChange;
            }
        }
    }
}
