using System.Diagnostics;

using ModernWpf.Controls;

using RecBinderProject.ViewModels.MacroMetadataViewModels;
using RecBinderProject.Services.MacroExecutionService.Interfaces;
using RecBinderProject.Services.MacroExecutionService;


namespace RecBinderProject.ViewModels.MacroExecutionViewModels
{
    public class MacroRecordingViewModel : MacroExecutionViewModelBase
    {
        public event Action? MacroRecorded;

        private readonly IMacroRecordingService _macroRecordingService;

        public MacroRecordingViewModel(
            LocalizationViewModel localizationViewModel, 
            MacroMetadataViewModel macroMetadataViewModel, 
            MacroExecutionStatusViewModel macroExecutionStatusViewModel, 
            IMacroRecordingService macroRecordingService) : base(localizationViewModel, macroMetadataViewModel, macroExecutionStatusViewModel, macroRecordingService)
        {
            this._macroRecordingService = macroRecordingService;
        }


        protected override void OnStartCommand()
        {
            try
            {
                if (this._macroRecordingService.State == MacroExecutionState.Paused) // TODO это не зона ответсвтвенности VM, она не должна проверять, она должна тупо вызывать метод
                {
                    this._macroRecordingService.Resume();
                }
                else
                {
                    this.MacroMetadataViewModel.ApplyChanges(); // TODO сделать адекватнее как-нибудь

                    this._macroRecordingService.Start(this.MacroMetadataViewModel.RecordingMetadataModel);

                    //this.MacroRecorded?.Invoke();
                }
            }
            catch (ArgumentException ae)
            {
                ContentDialog contentDialog = new ContentDialog()
                {
                    Content = ae.Message,
                    CloseButtonText = "Закрыть",
                };
                contentDialog.ShowAsync();
            }
            catch (Exception ex)
            {
                ContentDialog contentDialog = new ContentDialog()
                {
                    Content = ex.Message,
                    CloseButtonText = "Закрыть",
                };
                contentDialog.ShowAsync();
            }
        }

        protected override void OnResumeCommand() // TODO под вопросом, так как отдельной кнопки продолжить нет (реализуется всё через кнопку start)
        {
            Debug.WriteLine("Play resume TODO");
        }

        protected override void OnPauseCommand()
        {
            this._macroRecordingService.Pause();
        }

        protected override void OnStopCommand()
        {
            this._macroRecordingService.Stop();
            this.MacroRecorded?.Invoke();
        }
    }
}

