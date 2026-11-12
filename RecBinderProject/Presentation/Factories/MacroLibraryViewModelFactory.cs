using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using RecBinderProject.ApplicationLayer.MacroExecutionService.Engine;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Playback;

using RecBinderProject.Domain;

using RecBinderProject.Infrastructure.Services.Localization;

using RecBinderProject.Presentation.Services.Dialog;

using RecBinderProject.Presentation.ViewModels.Localization;
using RecBinderProject.Presentation.ViewModels.MacroOptions;
using RecBinderProject.Presentation.ViewModels.MacroExecution;
using RecBinderProject.Presentation.ViewModels.MacroMetadataExecution;
using RecBinderProject.Presentation.ViewModels.MacroExecution.Status;


namespace RecBinderProject.Presentation.Factories
{
    public class MacroLibraryViewModelFactory : IMacroLibraryViewModelFactory
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        private IServiceScope? _currentScope;
        private IMacroPlaybackService? _macroPlaybackSerice;

        private readonly ILoggerFactory _loggerFactory;

        private readonly IDialogService _dialogService;

        private readonly ILocalizationService _localizationService;
        private readonly LocalizationViewModel _localizationViewModel;

        private readonly MacroEventPlayer _macroEventPlayer;
        private readonly IMacroPlaybackService _macroPlaybackService;

        private readonly MacroPlaybackOptionsViewModel _playbackOptionsViewModel;


        public MacroLibraryViewModelFactory(
            IServiceScopeFactory serviceScopeFactory,
            ILoggerFactory loggerFactory, 

            IDialogService dialogService, 

            ILocalizationService localizationService, 
            LocalizationViewModel localizationViewModel,

            MacroEventPlayer macroEventPlayer,
            IMacroPlaybackService macroPlaybackService,

            MacroPlaybackOptionsViewModel playbackOptionsViewModel)
        {
            this._serviceScopeFactory = serviceScopeFactory;

            this._loggerFactory = loggerFactory;

            this._dialogService = dialogService;

            this._localizationService = localizationService;
            this._localizationViewModel = localizationViewModel;

            this._macroEventPlayer = macroEventPlayer;
            this._macroPlaybackService = macroPlaybackService;

            this._playbackOptionsViewModel = playbackOptionsViewModel;
        }


        public MacroPlaybackViewModel CreatePlayback(Macro macro)
        {
            this._currentScope?.Dispose();

            try
            {
                this._currentScope = this._serviceScopeFactory.CreateScope();

                MacroEventPlayer macroEventPlayer = this._currentScope.ServiceProvider.GetRequiredService<MacroEventPlayer>();

                MacroEventPlayer macroEventPlayer = this._currentScope.ServiceProvider.GetRequiredService<MacroEventPlayer>();
            }
            catch
            {

            }

            MacroPlaybackService macroPlaybackService = new MacroPlaybackService(this._loggerFactory.CreateLogger<>, macro, this._playbackOptions);

            MacroMetadataViewerViewModel macroMetadataViewerViewModel = new MacroMetadataViewerViewModel(macro.Metadata);
            MacroPlaybackStatusViewModel macroPlaybackStatusViewModel = new MacroPlaybackStatusViewModel(macroPlaybackService, this._localizationService);

            ILogger<MacroPlaybackViewModel> macroPlaybackViewModelLogger = this._loggerFactory.CreateLogger<MacroPlaybackViewModel>();
            MacroPlaybackViewModel macroPlaybackViewModel = new MacroPlaybackViewModel(macroPlaybackViewModelLogger, this._dialogService, macroPlaybackService, this._localizationViewModel, macroPlaybackStatusViewModel, macroMetadataViewerViewModel, this._playbackOptionsViewModel);

            throw new NotImplementedException();
        }
    }
}
