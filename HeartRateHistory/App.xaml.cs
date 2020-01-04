using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using BurnsBac.WindowsAppToolkit;
using BurnsBac.WindowsAppToolkit.ViewModels;
using BurnsBac.WindowsAppToolkit.Windows;

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

            // Make sure the config window know where to find type information
            BurnsBac.HotConfig.TypeResolver.ConfigDataProvidersDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            // this isn't ideal, but the RgbConvert needs to listen to the messagebus for settings file changes, so make
            // sure that's loaded at least once.
            Converters.HeartRateRgbConverter.Setup();
        }

        /// <summary>
        /// Shows error window for uncaught exceptions. Closes application once
        /// the error window is closed.
        /// </summary>
        /// <param name="ex">Exception to display.</param>
        /// <param name="source">Source of exception.</param>
        private void ShowUnhandledException(Exception ex, string source)
        {
            var ewvm = new ErrorWindowViewModel($"Unhandled exception in application: {source}", ex)
            {
                ExitOnClose = true,
            };

            Workspace.RecreateSingletonWindow<ErrorWindow>(ewvm);
        }
    }
}
