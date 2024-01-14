using Artkeeper.StaticClasses;
using System.Windows;

namespace Artkeeper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            WindowChangeDetector.StartActiveWindowChangeDetection();
            SavingSystem.Initialize();
            Update.Initialize();
            ArtkeeperEventManager.Initialize();
        }
        public static bool IsShuttingDown()
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