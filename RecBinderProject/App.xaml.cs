using System.Windows;
using System.Windows.Threading;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using RecBinderProject.ViewModels;
using RecBinderProject.Models.MacroRepository;
using RecBinderProject.Services.AppSettingsService;
using RecBinderProject.Services.LocalizationService;
using RecBinderProject.ViewModels.AppSettingsViewModels;
using RecBinderProject.Views;
using RecBinderProject.Services.MainWindowViewModelFactory;


namespace RecBinderProject
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            ServiceCollection services = new ServiceCollection();

            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Debug);
            });

            // services
            services.AddSingleton<IMacroRepository, MacroRepository>();

            services.AddSingleton<ILocalizationService, LocalizationService>();
            services.AddSingleton<IAppSettingsService, AppSettingsService>();

            // view models
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<IMainWindowViewModelFactory, MainWindowViewModelFactory>();

            services.AddSingleton<AppSettingsViewModel>();
            services.AddSingleton<GeneralAppSettingsSectionViewModel>();
            //services.AddSingleton<ProfileAppSettingsSectionViewModel>();
            //services.AddSingleton<AdvancedAppSettingsSectionViewModel>();

            // pages
            services.AddSingleton<AppSettingsControl>();


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
