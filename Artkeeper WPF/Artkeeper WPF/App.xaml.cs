using System.Windows;

namespace Artkeeper
{
    /// <summary>
    /// Interaction logic for
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            WindowChangeDetector.StartActiveWindowChangeDetection();
        }
    }
}
