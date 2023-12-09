using Artkeeper.UserControls;
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

        public MainWindow()
        {
            InitializeComponent();

            timerStackPanel = (StackPanel)FindName("TimerStackPanel");
        }

        public void AddTimer_Click(object sender, RoutedEventArgs e)
        {
            TimerControl timerControl = new TimerControl();

            int index = timerStackPanel.Children.Count - 1;
            timerStackPanel.Children.Insert(index, timerControl);
        }
    }
}
