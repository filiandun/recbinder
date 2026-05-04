using RecBinderProject.Models;
using RecBinderProject.Models.MacroRepository;

using RecBinderProject.Services.AppSettingsService;
using RecBinderProject.Services.LocalizationService;
using RecBinderProject.Services.MacroExecutionService.Implementations;

using RecBinderProject.ViewModels;
using RecBinderProject.ViewModels.AppSettingsViewModels;
using RecBinderProject.ViewModels.MacroMetadataViewModels;
using RecBinderProject.ViewModels.MacroExecutionViewModels;


namespace RecBinderProject.Services.MainWindowViewModelFactory
{
    public class MainWindowViewModelFactory : IMainWindowViewModelFactory
    {
        private readonly ILocalizationService _localizationService;

        private readonly IMacroRepository _macroRepository;

        private readonly IAppSettingsService _appSettingsService;


        public MainWindowViewModelFactory(ILocalizationService localizationService, IMacroRepository macroRepository, IAppSettingsService appSettingsService)
        {
            this._localizationService = localizationService;
            this._macroRepository = macroRepository;
            this._appSettingsService = appSettingsService;
        }


        public MacroPlaybackViewModel CreatePlayback(Macro macro)
        {
            LocalizationViewModel localizationViewModel = new LocalizationViewModel(this._localizationService);

            MacroMetadataViewerViewModel macroMetadataViewerViewModel = new MacroMetadataViewerViewModel(macro.Metadata);

            MacroOptions macroOptions = new MacroOptions();
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
            MacroOptions macroOptions = new MacroOptions();
            MacroRecordingService macroRecordingService = new MacroRecordingService(this._macroRepository, macroMetadata, macroOptions);

            MacroExecutionStatusViewModel macroExecutionStatusViewModel = new MacroExecutionStatusViewModel(macroRecordingService, this._localizationService, "Create");

            MacroRecordingViewModel macroRecordingViewModel = new MacroRecordingViewModel(macroRecordingService, localizationViewModel, macroMetadataCreatorViewModel, macroExecutionStatusViewModel, macroOptions);

            return macroRecordingViewModel;
        }

        public AppSettingsViewModel GetAppSettings()
        {
            return new AppSettingsViewModel(this._appSettingsService, new GeneralAppSettingsSectionViewModel(this._appSettingsService));
        }
    }
}
