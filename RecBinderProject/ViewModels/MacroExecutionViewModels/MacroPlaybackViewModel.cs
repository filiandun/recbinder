using System.Diagnostics;
using System.Windows.Input;
using System.ComponentModel;

using RecBinderProject.Models;
using RecBinderProject.ViewModels.MacroMetadataViewModels;
using RecBinderProject.Services.MacroExecutionService.Interfaces;
using RecBinderProject.Services.MacroExecutionService;


namespace RecBinderProject.ViewModels.MacroExecutionViewModels
{
    public class MacroPlaybackViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly IMacroPlaybackService _macroPlaybackService;

        public LocalizationViewModel LocalizationViewModel { get; init; }
        public MacroMetadataViewModel MacroMetadataViewModel { get; init; }
        public MacroExecutionStatusViewModel MacroExecutionStatusViewModel { get; init; }
        public MacroOptions MacroOptions { get; private set; }

        public ICommand StartCommand { get; private set; }
        public ICommand PauseCommand { get; private set; }
        public ICommand StopCommand { get; private set; }

        public bool CanStart() => this._macroPlaybackService.State != MacroExecutionState.Playing;
        public bool CanPause() => this._macroPlaybackService.State == MacroExecutionState.Playing;
        public bool CanStop() => this._macroPlaybackService.State == MacroExecutionState.Playing || this._macroPlaybackService.State == MacroExecutionState.Paused;

        public bool CanEditMetadata => this._macroPlaybackService.State != MacroExecutionState.Playing;


        public MacroPlaybackViewModel(LocalizationViewModel localizationViewModel, MacroMetadataViewModel macroMetadataViewModel, MacroExecutionStatusViewModel macroExecutionStatusViewModel, IMacroPlaybackService macroPlaybackService)
        {
            this._macroPlaybackService = macroPlaybackService;

            this.LocalizationViewModel = localizationViewModel;
            this.MacroMetadataViewModel = macroMetadataViewModel;
            this.MacroExecutionStatusViewModel = macroExecutionStatusViewModel;

            this.MacroOptions = new MacroOptions();

            this.StartCommand = new RelayCommand(this.OnStartCommand, this.CanStart);
            this.PauseCommand = new RelayCommand(this.OnPauseCommand, this.CanPause);
            this.StopCommand = new RelayCommand(this.OnStopCommand, this.CanStop);

            this._macroPlaybackService.StateChanged += this.MacroPlaybackService_StateChanged;
        }


        private void MacroPlaybackService_StateChanged(MacroExecutionState obj)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.CanStart)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.CanPause)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.CanStop)));

            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.CanEditMetadata)));
        }


        public void OnStartCommand()
        {
            if (this._macroPlaybackService.State == MacroExecutionState.Paused) // TODO это не зона ответсвтвенности VM, она не должна проверять, она должна тупо вызывать метод
            {
                this._macroPlaybackService.Resume();
            }
            else
            {
                this._macroPlaybackService.Start();
            }
        }

        public void OnResumeCommand()
        {
            //this._recordProcess.Start();
            Debug.WriteLine("Play resume TODO");
        }

        public void OnPauseCommand()
        {
            this._macroPlaybackService.Pause();
        }

        public void OnStopCommand()
        {
            this._macroPlaybackService.Stop();
        }
    }
}
