using RecBinderProject.Domain;

using RecBinderProject.ApplicationLayer.MacroExecutionService;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Recording;

using RecBinderProject.Presentation.Services;
using RecBinderProject.Presentation.ViewModels.MainWindow;
using RecBinderProject.Presentation.ViewModels.AppSettings;
using RecBinderProject.Presentation.ViewModels.Localization;
using RecBinderProject.Presentation.ViewModels.MacroExecution;
using RecBinderProject.Presentation.ViewModels.MacroMetadataExecution;

using RecBinderProject.Infrastructure.MacroRepository;
using RecBinderProject.Infrastructure.AppSettingsService;
using RecBinderProject.Infrastructure.LocalizationService;



namespace RecBinderProject.Presentation.Factories
{
    public class MainWindowViewModelFactory : IMainWindowViewModelFactory
    {
        private readonly ILocalizationService _localizationService;
        private readonly LocalizationViewModel _localizationViewModel;

        private readonly IMacroRepository _macroRepository;

        private readonly IAppSettingsService _appSettingsService;
        private readonly IThemeService _themeService;

        private readonly IMacroLibraryViewModelFactory _macroLibraryViewModelFactory;


        public MainWindowViewModelFactory(ILocalizationService localizationService, LocalizationViewModel localizationViewModel, IMacroRepository macroRepository, IAppSettingsService appSettingsService, IThemeService themeService, IMacroLibraryViewModelFactory macroLibraryViewModelFactory)
        {
            this._localizationService = localizationService;
            this._localizationViewModel = localizationViewModel;

            this._macroRepository = macroRepository;

            this._appSettingsService = appSettingsService;
            this._themeService = themeService;

            this._macroLibraryViewModelFactory = macroLibraryViewModelFactory;
        }


        public MacroLibraryViewModel CreateLibrary()
        {
            MacroListViewModel macroListViewModel = new MacroListViewModel(this._localizationService, this._macroRepository);

            MacroLibraryViewModel macroLibraryViewModel = new MacroLibraryViewModel(this._macroLibraryViewModelFactory, this._localizationViewModel, macroListViewModel, null);

            return macroLibraryViewModel;
        }

        public MacroRecordingViewModel CreateRecording()
        {
            MacroMetadataCreatorViewModel macroMetadataCreatorViewModel = new MacroMetadataCreatorViewModel();

            MacroExecutionOptions macroOptions = new MacroExecutionOptions();
            MacroRecordingService macroRecordingService = new MacroRecordingService(this._macroRepository, macroOptions);

            MacroExecutionStatusViewModel macroExecutionStatusViewModel = new MacroExecutionStatusViewModel(macroRecordingService, this._localizationService, "Create");

            MacroRecordingViewModel macroRecordingViewModel = new MacroRecordingViewModel(macroRecordingService, this._localizationViewModel, macroMetadataCreatorViewModel, macroExecutionStatusViewModel, macroOptions);

            return macroRecordingViewModel;
        }

        public AppSettingsViewModel CreateAppSettings()
        {
            return new AppSettingsViewModel(this._appSettingsService, this._localizationViewModel, new GeneralAppSettingsSectionViewModel(this._localizationService, this._appSettingsService, this._themeService));
        }
    }
}
