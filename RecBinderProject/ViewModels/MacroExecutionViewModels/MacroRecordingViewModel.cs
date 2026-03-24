using System.Diagnostics;
using System.Windows.Input;
using System.ComponentModel;

using ModernWpf.Controls;

using RecBinderProject.Models;
using RecBinderProject.ViewModels.MacroMetadataViewModels;
using RecBinderProject.Services.MacroExecutionService.Interfaces;
using RecBinderProject.Services.MacroExecutionService;


namespace RecBinderProject.ViewModels.MacroExecutionViewModels
{
    public class MacroRecordingViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public event Action? MacroRecorded;

        private readonly IMacroRecordingService _macroRecordingService;

        public LocalizationViewModel LocalizationViewModel { get; init; }
        public MacroMetadataViewModel MacroMetadataViewModel { get; init; }
        public MacroExecutionStatusViewModel MacroExecutionStatusViewModel { get; init; }
        public MacroOptions MacroOptions { get; private set; }

        public ICommand StartCommand { get; private set; }
        public ICommand PauseCommand { get; private set; }
        public ICommand StopCommand { get; private set; }

        public bool CanStart() => this._macroRecordingService.State != MacroExecutionState.Playing;
        public bool CanPause() => this._macroRecordingService.State == MacroExecutionState.Playing;
        public bool CanStop() => this._macroRecordingService.State == MacroExecutionState.Playing || this._macroRecordingService.State == MacroExecutionState.Paused;

        public bool CanEditMetadata => this._macroRecordingService.State != MacroExecutionState.Playing;


        public MacroRecordingViewModel(LocalizationViewModel localizationViewModel, MacroMetadataViewModel macroMetadataViewModel, MacroExecutionStatusViewModel macroExecutionStatusViewModel, IMacroRecordingService macroRecordingService)
        {
            this._macroRecordingService = macroRecordingService;

            this.LocalizationViewModel = localizationViewModel;
            this.MacroMetadataViewModel = macroMetadataViewModel;
            this.MacroExecutionStatusViewModel = macroExecutionStatusViewModel;

            this.MacroOptions = new MacroOptions();
            
            this.StartCommand = new RelayCommand(this.OnStartCommand, this.CanStart);
            this.PauseCommand = new RelayCommand(this.OnPauseCommand, CanPause);
            this.StopCommand = new RelayCommand(this.OnStopCommand, this.CanStop);

            this._macroRecordingService.StateChanged += this.MacroRecordingService_StateChanged;
        }


        private void MacroRecordingService_StateChanged(MacroExecutionState obj)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.CanStart)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.CanPause)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.CanStop)));

            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.CanEditMetadata)));
        }


        public void OnStartCommand()
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

        public void OnResumeCommand() // TODO под вопросом, так как отдельной кнопки продолжить нет (реализуется всё через кнопку start)
        {
            Debug.WriteLine("Play resume TODO");
        }

        public void OnPauseCommand()
        {
            this._macroRecordingService.Pause();
        }

        public void OnStopCommand()
        {
            this._macroRecordingService.Stop();
            this.MacroRecorded?.Invoke();
        }
    }
}

