﻿using System.Windows;
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
