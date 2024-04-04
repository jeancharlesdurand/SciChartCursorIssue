using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using SciChart.Charting;
using SciChart.Charting.Visuals;
using SciChart.Drawing.VisualXcceleratorRasterizer;
using SciChart.Examples.ExternalDependencies.Controls.ExceptionView;

namespace SciChartExport
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
		     DispatcherUnhandledException += OnDispatcherUnhandledException;

            Task.Run(() =>
            {
                try
                {
                    VisualXcceleratorEngine.UseAutoShutdown = false;
                    VisualXcceleratorEngine.RestartEngine(); 
                }
                catch
                {
                }
            });

            InitializeComponent();

            // TODO: Put your SciChart Runtime License Key here if needed
            // SciChartSurface.SetRuntimeLicenseKey(@"{YOUR SCICHART WPF v8 RUNTIME LICENSE KEY}");
            SciChartSurface.SetRuntimeLicenseKey("NPIONtR4tli22z52D/k6pSG1HEJNJnNb6marAZRhWDYsTWZdlAZO7pJVZ91hx+VIrkqVXvpsRoZjXH4nOV4Lm828fvQSlZbvBCBvPM+6zqrAs5n9VKFfWPnv9rqGGyPbzgxu8/pww7n/MHT/4Wdf5DdCkKm+41/lQ6DQpsFTw/Q/4GFtktXu0Iq7sNXhXRycYuZK0q1zsBRWxco59k0Cz1eCGuuC5tHKxw1+78bsVM9iqAkdJ4ckpTOHCJyU38j8/BSMUoyShdWPHn4EzZTDu1MAlLMTM+Mz96XB9LHBthzu79WedWeDf508F0KWLN9qgfiMGAJ7fx3u7hJafoWbdLrWv+K/eo9GKBTHlnD4lm8RBoNWt9BqjWaPCIuqclhh3FLhfvfcR3jdFfLRN+cmjtjLSHvm4lm7ymseZCgwMdPa7Qn/Uc3Rx5AbOje9GsS1/xcXhiqnjpYa//jxGBU7uTDwyw==");
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {            
            var exceptionView = new ExceptionView(e.Exception)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
            };

            exceptionView.ShowDialog();

            e.Handled = true;
        }
    }
}
