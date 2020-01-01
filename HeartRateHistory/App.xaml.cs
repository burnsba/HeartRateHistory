using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace HeartRateHistory
{
    /// <summary>
    /// Interaction logic for App.xaml .
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// App startup method.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Args.</param>
        private void App_Startup(object sender, StartupEventArgs e)
        {
            // Hook general uncaught exception events to show user. (this won't help with some of the CLR/pinvoke errors that might occur)
            Dispatcher.UnhandledException += (s, e) =>
            {
                e.Handled = true;
                ShowUnhandledException(e.Exception, "Dispatcher.UnhandledException");
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                e.SetObserved();
                ShowUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException");
            };
        }

        /// <summary>
        /// Shows error window for uncaught exceptions. Closes application once
        /// the error window is closed.
        /// </summary>
        /// <param name="ex">Exception to display.</param>
        /// <param name="source">Source of exception.</param>
        private void ShowUnhandledException(Exception ex, string source)
        {
            var ewvm = new ViewModels.ErrorWindowViewModel($"Unhandled exception in application: {source}", ex)
            {
                ExitOnClose = true,
            };

            Workspace.RecreateSingletonWindow<Windows.ErrorWindow>(ewvm);
        }
    }
}
