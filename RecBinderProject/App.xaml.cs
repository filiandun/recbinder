using ModernWpf;
using RecBinderProject.Services;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Threading;

namespace RecBinderProject
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public SettingsService SettingsService { get; private set; }
        public RecordingRepository RecordingRepository { get; private set; }


        protected override void OnStartup(StartupEventArgs e)
        {
            this.SettingsService = new SettingsService();
            this.RecordingRepository = new RecordingRepository(this.SettingsService.AppSettings.SaveDirectory);

            base.OnStartup(e);
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Ошибка: " + e.Exception.Message);
            e.Handled = true;
        }
    }
}
