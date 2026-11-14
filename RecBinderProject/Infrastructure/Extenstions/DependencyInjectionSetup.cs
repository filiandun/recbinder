using Microsoft.Extensions.DependencyInjection;

using RecBinderProject.ApplicationLayer.MacroExecutionService.Common;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Playback;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Recording;

using RecBinderProject.Infrastructure.Repositories;

using RecBinderProject.Infrastructure.Factories;

using RecBinderProject.Infrastructure.Services.AppSettings;
using RecBinderProject.Infrastructure.Services.Input;
using RecBinderProject.Infrastructure.Services.GlobalHotkey;
using RecBinderProject.Infrastructure.Services.Localization;

using RecBinderProject.Presentation.Services;
using RecBinderProject.Presentation.Services.Dialog;

using RecBinderProject.Presentation.Factories;

using RecBinderProject.Presentation.ViewModels.MainWindow;
using RecBinderProject.Presentation.ViewModels.AppSettings;
using RecBinderProject.Presentation.ViewModels.MacroLibrary;
using RecBinderProject.Presentation.ViewModels.Localization;
using RecBinderProject.Presentation.ViewModels.MacroExecution;
using RecBinderProject.Presentation.ViewModels.MacroExecution.Status;



namespace RecBinderProject.Infrastructure.Extenstions
{
    public static class DependencyInjectionSetup
    {
        public static IServiceCollection AddApplicationComponents(this IServiceCollection services)
        {
            // Common
            services.AddSingleton<IDialogService, DialogService>();

            services.AddSingleton<IMacroRepository, MacroRepository>();

            services.AddSingleton<ILocalizationService, LocalizationService>();
            services.AddSingleton<IAppSettingsService, AppSettingsService>();
            services.AddSingleton<IThemeService, ThemeService>();

            services.AddSingleton<LocalizationViewModel>();
            services.AddSingleton<GlobalHotkeyService>();

            // Macro Recording
            services.AddSingleton<IInputListener, RawInputListener>();
            services.AddSingleton<IMacroEventRecorderFactory, MacroEventRecorderFactory>();
            services.AddSingleton<IMacroRecordingService, MacroRecordingService>();

            services.AddSingleton<MacroRecordingStatusViewModel>();
            services.AddSingleton<MacroRecordingViewModel>();

            // Macro Library
            services.AddSingleton<IMacroEventPlayerFactory, MacroEventPlayerFactory>();
            services.AddSingleton<IMacroPlaybackService, MacroPlaybackService>();

            services.AddSingleton<MacroPlaybackStatusViewModel>();

            services.AddSingleton<IMacroLibraryViewModelFactory, MacroLibraryViewModelFactory>();
            services.AddSingleton<MacroLibraryListViewModel>();
            services.AddSingleton<MacroLibraryViewModel>();

            // App Settings  
            services.AddSingleton<GeneralAppSettingsSectionViewModel>();
            //services.AddSingleton<AdvancedAppSettingsSectionViewModel>();
            //services.AddSingleton<ProfileAppSettingsSectionViewModel>();
            services.AddSingleton<AppSettingsViewModel>();

            // Main Window
            services.AddSingleton<MainWindowViewModel>();

            return services;
        }

        public static IServiceProvider InitApplicationState(this IServiceProvider serviceProvider)
        {
            IAppSettingsService appSettingsService = serviceProvider.GetRequiredService<IAppSettingsService>();
            appSettingsService.Load();

            ILocalizationService localizationService = serviceProvider.GetRequiredService<ILocalizationService>();
            localizationService.ApplyLanguage(appSettingsService.AppSettings.General.AppLanguage);

            IThemeService themeService = serviceProvider.GetRequiredService<IThemeService>();
            themeService.ApplyTheme(appSettingsService.AppSettings.General.AppTheme);

            return serviceProvider;
        }
    }
}
