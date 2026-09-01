using RecBinderProject.Domain;

using RecBinderProject.ApplicationLayer.MacroExecutionService;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Playback;

using RecBinderProject.Presentation.ViewModels.Localization;
using RecBinderProject.Presentation.ViewModels.MacroExecution;
using RecBinderProject.Presentation.ViewModels.MacroMetadataExecution;
using RecBinderProject.Presentation.ViewModels.MacroExecution.Status;

using RecBinderProject.Infrastructure.Services.Localization;


namespace RecBinderProject.Presentation.Factories
{
    public class MacroLibraryViewModelFactory : IMacroLibraryViewModelFactory
    {
        private readonly ILocalizationService _localizationService;
        private readonly LocalizationViewModel _localizationViewModel;


        public MacroLibraryViewModelFactory(ILocalizationService localizationService, LocalizationViewModel localizationViewModel)
        {
            this._localizationService = localizationService;
            this._localizationViewModel = localizationViewModel;
        }


        public MacroPlaybackViewModel CreatePlayback(Macro macro)
        {
            MacroExecutionOptions macroOptions = new MacroExecutionOptions();
            MacroPlaybackService macroPlaybackService = new MacroPlaybackService(macroOptions, macro);

            MacroMetadataViewerViewModel macroMetadataViewerViewModel = new MacroMetadataViewerViewModel(macro.Metadata);
            MacroPlaybackStatusViewModel macroPlaybackStatusViewModel = new MacroPlaybackStatusViewModel(macroPlaybackService, this._localizationService);
            
            MacroPlaybackViewModel macroPlaybackViewModel = new MacroPlaybackViewModel(macroPlaybackService, this._localizationViewModel, macroMetadataViewerViewModel, macroPlaybackStatusViewModel, macroOptions);

            return macroPlaybackViewModel;
        }
    }
}
