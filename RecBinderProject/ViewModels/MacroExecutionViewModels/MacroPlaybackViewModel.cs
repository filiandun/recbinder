using System.Diagnostics;

using RecBinderProject.Services.MacroExecutionService;
using RecBinderProject.ViewModels.MacroMetadataViewModels;
using RecBinderProject.Services.MacroExecutionService.Interfaces;
using RecBinderProject.Models;


namespace RecBinderProject.ViewModels.MacroExecutionViewModels
{
    public class MacroPlaybackViewModel : MacroExecutionViewModelBase
    {
        private readonly IMacroPlaybackService _macroPlaybackService;

        public MacroMetadataViewerViewModel MacroMetadataViewerViewModel { get; init; }


        public MacroPlaybackViewModel(
            IMacroPlaybackService macroPlaybackService,
            LocalizationViewModel localizationViewModel, 
            MacroMetadataViewerViewModel macroMetadataViewerViewModel, 
            MacroExecutionStatusViewModel macroExecutionStatusViewModel, 
            MacroOptions macroOptions) : base(macroPlaybackService, localizationViewModel, macroExecutionStatusViewModel, macroOptions)
        {
            this._macroPlaybackService = macroPlaybackService;

            this.MacroMetadataViewerViewModel = macroMetadataViewerViewModel;
        }


        protected override void OnStartCommand()
        {
            if (this._service.State == MacroExecutionState.Paused) // TODO это не зона ответсвтвенности VM, она не должна проверять, она должна тупо вызывать метод
            {
                this._macroPlaybackService.Resume();
            }
            else
            {
                this._macroPlaybackService.Start();
            }
        }

        protected override void OnResumeCommand()
        {
            //this._recordProcess.Start();
            Debug.WriteLine("Play resume TODO");
        }

        protected override void OnPauseCommand()
        {
            this._macroPlaybackService.Pause();
        }

        protected override void OnStopCommand()
        {
            this._macroPlaybackService.Stop();
        }
    }
}
