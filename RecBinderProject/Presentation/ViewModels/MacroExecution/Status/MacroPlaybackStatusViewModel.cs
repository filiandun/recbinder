
using RecBinderProject.ApplicationLayer.Localization;
using RecBinderProject.ApplicationLayer.MacroExecution.Abstractions;

namespace RecBinderProject.Presentation.ViewModels.MacroExecution.Status
{
    public class MacroPlaybackStatusViewModel : MacroExecutionStatusViewModelBase
    {
        protected override string HintLocalizationKey => "Play_ButtonHint";
        protected override string ProcessingLocalizationKey => "Play_Playing";
        protected override string PausedLocalizationKey => "Play_Paused";
        protected override string CompletedLocalizationKey => "Play_Completed";
        protected override string StoppedLocalizationKey => "Play_Stopped";

        public MacroPlaybackStatusViewModel(IMacroPlaybackService playbackService, ILocalizationService localizationService) : base(playbackService, localizationService)
        {
        }
    }
}
