using System.Windows.Input;
using System.ComponentModel;

using RecBinderProject.ApplicationLayer.MacroExecutionService;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Common;

using RecBinderProject.Presentation.Commands;
using RecBinderProject.Presentation.ViewModels.Localization;
using RecBinderProject.Presentation.ViewModels.MacroExecution.Status;


namespace RecBinderProject.Presentation.ViewModels.MacroExecution
{
    public abstract class MacroExecutionViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected readonly IMacroExecutionInfo _service;

        public LocalizationViewModel LocalizationViewModel { get; init; }
        public MacroExecutionStatusViewModelBase StatusViewModel { get; init; }

        public ICommand StartCommand { get; private set; }
        public ICommand PauseCommand { get; private set; }
        public ICommand StopCommand { get; private set; }

        public bool CanStart => this._service.State != MacroExecutionState.Playing;
        public bool CanPause => this._service.State == MacroExecutionState.Playing;
        public bool CanStop => this._service.State == MacroExecutionState.Playing || this._service.State == MacroExecutionState.Paused;

        public bool CanEditMetadata => this._service.State != MacroExecutionState.Playing;


        public MacroExecutionViewModelBase(IMacroExecutionInfo service, LocalizationViewModel localizationViewModel, MacroExecutionStatusViewModelBase statusViewModel)
        {
            this._service = service;

            this.LocalizationViewModel = localizationViewModel;
            this.StatusViewModel = statusViewModel;

            this.StartCommand = new RelayCommand(this.OnStartCommand, () => this.CanStart);
            this.PauseCommand = new RelayCommand(this.OnPauseCommand, () => this.CanPause);
            this.StopCommand = new RelayCommand(this.OnStopCommand, () => this.CanStop);

            this._service.StateChanged += this.OnServiceStateChanged;
        }


        private void OnServiceStateChanged(MacroExecutionState state)
        {
            (this.StartCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (this.PauseCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (this.StopCommand as RelayCommand)?.RaiseCanExecuteChanged();

            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.CanEditMetadata)));
        }


        protected abstract void OnStartCommand();
        protected abstract void OnResumeCommand();
        protected abstract void OnPauseCommand();
        protected abstract void OnStopCommand();
    }
}
