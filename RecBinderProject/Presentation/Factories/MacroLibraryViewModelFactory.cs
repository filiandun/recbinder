using RecBinderProject.Domain;
using Microsoft.Extensions.Logging;

using RecBinderProject.ApplicationLayer.MacroExecutionService.Options;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Playback;

using RecBinderProject.Presentation.ViewModels.MacroOptions;
using RecBinderProject.Presentation.ViewModels.Localization;
using RecBinderProject.Presentation.ViewModels.MacroExecution;
using RecBinderProject.Presentation.ViewModels.MacroMetadataExecution;
using RecBinderProject.Presentation.ViewModels.MacroExecution.Status;

using RecBinderProject.Infrastructure.Services.Localization;


namespace RecBinderProject.Presentation.Factories
{
    public class MacroLibraryViewModelFactory : IMacroLibraryViewModelFactory
    {
        private readonly ILoggerFactory _loggerFactory;

        private readonly ILocalizationService _localizationService;
        private readonly LocalizationViewModel _localizationViewModel;

        private readonly MacroPlaybackOptions _playbackOptions;
        private readonly MacroPlaybackOptionsViewModel _playbackOptionsViewModel;


        public MacroLibraryViewModelFactory(ILoggerFactory loggerFactory, ILocalizationService localizationService, LocalizationViewModel localizationViewModel, MacroPlaybackOptions playbackOptions, MacroPlaybackOptionsViewModel playbackOptionsViewModel)
        {
            this._loggerFactory = loggerFactory;

            this._localizationService = localizationService;
            this._localizationViewModel = localizationViewModel;

            this._playbackOptions = playbackOptions;
            this._playbackOptionsViewModel = playbackOptionsViewModel;
        }


        public MacroPlaybackViewModel CreatePlayback(Macro macro)
        {
            MacroPlaybackService macroPlaybackService = new MacroPlaybackService(this._loggerFactory, macro, this._playbackOptions);

            MacroMetadataViewerViewModel macroMetadataViewerViewModel = new MacroMetadataViewerViewModel(macro.Metadata);
            MacroPlaybackStatusViewModel macroPlaybackStatusViewModel = new MacroPlaybackStatusViewModel(macroPlaybackService, this._localizationService);
            
            MacroPlaybackViewModel macroPlaybackViewModel = new MacroPlaybackViewModel(macroPlaybackService, this._localizationViewModel, macroPlaybackStatusViewModel, macroMetadataViewerViewModel, this._playbackOptionsViewModel);

            return macroPlaybackViewModel;
        }
    }
}
