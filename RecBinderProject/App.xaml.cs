using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;

using RecBinderProject.Pages;
using RecBinderProject.Models.RecordingRepository;
using RecBinderProject.Services.AppSettingsService;
using RecBinderProject.Services.LocalizationService;


namespace RecBinderProject
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            ServiceCollection services = new ServiceCollection();

            services.AddSingleton<IRecordingRepository, RecordingRepository>();
            services.AddSingleton<IAppSettingsService, AppSettingsService>();
            services.AddSingleton<ILocalizationService, LocalizationService>();

            services.AddSingleton<SettingsPage>();

            this.ServiceProvider = services.BuildServiceProvider();

            var mainWindow = ActivatorUtilities.CreateInstance<MainWindow>(this.ServiceProvider);
            mainWindow.Show();

            base.OnStartup(e);
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Ошибка: " + e.Exception.Message);
            e.Handled = true;
        }
    }
}
