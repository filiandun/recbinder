using RecBinderProject.Domain;

using RecBinderProject.ApplicationLayer.MacroExecutionService;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Playback;
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


        public MainWindowViewModelFactory(ILocalizationService localizationService, LocalizationViewModel localizationViewModel, IMacroRepository macroRepository, IAppSettingsService appSettingsService, IThemeService themeService)
        {
            this._localizationService = localizationService;
            this._localizationViewModel = localizationViewModel;

            this._macroRepository = macroRepository;

            this._appSettingsService = appSettingsService;
            this._themeService = themeService;
        }


        public MacroLibraryViewModel CreateLibrary()
        {
            LocalizationViewModel localizationViewModel = new LocalizationViewModel(this._localizationService);

            MacroListViewModel macroListViewModel = new MacroListViewModel(this._localizationService, this._macroRepository);

            var macroLoadResult = this._macroRepository.LoadAll();

            MacroPlaybackViewModel macroPlaybackViewModel = this.CreatePlayback(macroLoadResult.Macros[0]);

            MacroLibraryViewModel macroLibraryViewModel = new MacroLibraryViewModel(this, localizationViewModel, macroListViewModel, macroPlaybackViewModel);

            return macroLibraryViewModel;
        }

        public MacroPlaybackViewModel CreatePlayback(Macro macro)
        {
            LocalizationViewModel localizationViewModel = new LocalizationViewModel(this._localizationService);

            MacroMetadataViewerViewModel macroMetadataViewerViewModel = new MacroMetadataViewerViewModel(macro.Metadata);

            MacroExecutionOptions macroOptions = new MacroExecutionOptions();
            MacroPlaybackService macroPlaybackService = new MacroPlaybackService(macroOptions, macro);

            MacroExecutionStatusViewModel macroExecutionStatusViewModel = new MacroExecutionStatusViewModel(macroPlaybackService, this._localizationService, "Play");

            MacroPlaybackViewModel macroPlaybackViewModel = new MacroPlaybackViewModel(macroPlaybackService, localizationViewModel, macroMetadataViewerViewModel, macroExecutionStatusViewModel, macroOptions);

            return macroPlaybackViewModel;
        }

        public MacroRecordingViewModel CreateRecording()
        {
            LocalizationViewModel localizationViewModel = new LocalizationViewModel(this._localizationService);

            MacroMetadataCreatorViewModel macroMetadataCreatorViewModel = new MacroMetadataCreatorViewModel();

            MacroMetadata macroMetadata = new MacroMetadata();
            MacroExecutionOptions macroOptions = new MacroExecutionOptions();
            MacroRecordingService macroRecordingService = new MacroRecordingService(this._macroRepository, macroMetadata, macroOptions);

            MacroExecutionStatusViewModel macroExecutionStatusViewModel = new MacroExecutionStatusViewModel(macroRecordingService, this._localizationService, "Create");

            MacroRecordingViewModel macroRecordingViewModel = new MacroRecordingViewModel(macroRecordingService, localizationViewModel, macroMetadataCreatorViewModel, macroExecutionStatusViewModel, macroOptions);

            return macroRecordingViewModel;
        }

        public AppSettingsViewModel GetAppSettings()
        {
            return new AppSettingsViewModel(this._appSettingsService, this._localizationViewModel, new GeneralAppSettingsSectionViewModel(this._localizationService, this._appSettingsService, this._themeService));
        }
    }
}
