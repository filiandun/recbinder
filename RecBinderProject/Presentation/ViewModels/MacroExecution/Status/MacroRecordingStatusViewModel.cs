
using RecBinderProject.ApplicationLayer.MacroExecutionService.Recording;
using RecBinderProject.Infrastructure.Services.Localization;


namespace RecBinderProject.Presentation.ViewModels.MacroExecution.Status
{
    public class MacroRecordingStatusViewModel : MacroExecutionStatusViewModelBase
    {
        protected override string HintLocalizationKey => "Create_ButtonHint";
        protected override string ProcessingLocalizationKey => "Create_Playing";
        protected override string PausedLocalizationKey => "Create_Paused";
        protected override string CompletedLocalizationKey => "Create_Completed";
        protected override string StoppedLocalizationKey => "Create_Stopped";

        public MacroRecordingStatusViewModel(IMacroRecordingService recordingService, ILocalizationService localizationService) : base(recordingService, localizationService)
        {
        }
    }
}
