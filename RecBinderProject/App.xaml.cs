using Microsoft.Extensions.DependencyInjection;
using ModernWpf;
using RecBinderProject.Pages;
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
        public IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            ServiceCollection services = new ServiceCollection();

            services.AddSingleton<IRecordingRepository, RecordingRepository>();
            services.AddSingleton<ISettingsService, SettingsService>();
            //services.AddSingleton<CreateRecordPage>();
            services.AddSingleton<SettingsPage>();

            this.ServiceProvider = services.BuildServiceProvider();

            base.OnStartup(e);
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Ошибка: " + e.Exception.Message);
            e.Handled = true;
        }
    }
}
