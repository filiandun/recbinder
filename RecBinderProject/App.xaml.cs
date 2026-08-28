using System.Windows;
using System.Windows.Threading;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using RecBinderProject.Presentation.Views;

using RecBinderProject.Infrastructure.MacroRepository;
using RecBinderProject.Infrastructure.AppSettingsService;
using RecBinderProject.Infrastructure.GlobalHotkeyService;
using RecBinderProject.Infrastructure.LocalizationService;

using RecBinderProject.Presentation.Services;
using RecBinderProject.Presentation.Factories;
using RecBinderProject.Presentation.ViewModels.MainWindow;
using RecBinderProject.Presentation.ViewModels.AppSettings;
using RecBinderProject.Presentation.ViewModels.Localization;
using RecBinderProject.Presentation.ViewModels.MacroExecution;


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

            services.AddSingleton<IThemeService, ThemeService>();

            services.AddSingleton<IDialogService, DialogService>();

            services.AddSingleton<GlobalHotkeyService>();

            // view models
            services.AddSingleton<LocalizationViewModel>();

            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<IMainWindowViewModelFactory, MainWindowViewModelFactory>();
            services.AddSingleton<MacroListViewModel>();
            services.AddSingleton<MacroLibraryViewModel>();
            services.AddSingleton<IMacroLibraryViewModelFactory, MacroLibraryViewModelFactory>();

            services.AddSingleton<AppSettingsViewModel>();
            services.AddSingleton<GeneralAppSettingsSectionViewModel>();
            //services.AddSingleton<ProfileAppSettingsSectionViewModel>();
            //services.AddSingleton<AdvancedAppSettingsSectionViewModel>();


            // views
            services.AddSingleton<AppSettingsView>();


            this.ServiceProvider = services.BuildServiceProvider();

            // 
            var appSettingsService = this.ServiceProvider.GetRequiredService<IAppSettingsService>();
            appSettingsService.Load();

            var localizationService = this.ServiceProvider.GetRequiredService<ILocalizationService>();
            localizationService.ApplyLanguage(appSettingsService.AppSettings.General.AppLanguage);

            var themeService = this.ServiceProvider.GetRequiredService<IThemeService>();
            themeService.ApplyTheme(appSettingsService.AppSettings.General.AppTheme);

            var mainWindow = ActivatorUtilities.CreateInstance<MainWindow>(this.ServiceProvider);
            mainWindow.Show();

            base.OnStartup(e);
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("error: " + e.Exception.Message);
            e.Handled = true;
        }
    }
}
