using System.ComponentModel;
using System.Windows.Input;

using Microsoft.Extensions.Logging;

using RecBinderProject.ApplicationLayer.MacroExecutionService;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Common;

using RecBinderProject.Presentation.Commands;
using RecBinderProject.Presentation.Services.Dialog;
using RecBinderProject.Presentation.ViewModels.Localization;
using RecBinderProject.Presentation.ViewModels.MacroExecution.Status;


namespace RecBinderProject.Presentation.ViewModels.MacroExecution
{
    public abstract class MacroExecutionViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected readonly ILogger _logger;

        protected readonly IDialogService _dialogService;

        protected readonly IMacroExecutionInfo _executionService;

        public LocalizationViewModel LocalizationViewModel { get; private set; }
        public MacroExecutionStatusViewModelBase StatusViewModel { get; private set; }

        public ICommand StartCommand { get; private set; }
        public ICommand PauseCommand { get; private set; }
        public ICommand StopCommand { get; private set; }

        public bool CanStart => this._executionService.State != MacroExecutionState.Playing;
        public bool CanPause => this._executionService.State == MacroExecutionState.Playing;
        public bool CanStop => this._executionService.State == MacroExecutionState.Playing || this._executionService.State == MacroExecutionState.Paused;

        public bool CanEditMetadata => this._executionService.State != MacroExecutionState.Playing;


        public MacroExecutionViewModelBase(ILogger logger, IDialogService dialogService, IMacroExecutionInfo executionService, LocalizationViewModel localizationViewModel, MacroExecutionStatusViewModelBase statusViewModel)
        {
            this._logger = logger;

            this._dialogService = dialogService;

            this._executionService = executionService;

            this.LocalizationViewModel = localizationViewModel;
            this.StatusViewModel = statusViewModel;

            this.StartCommand = new RelayCommand(this.OnStartCommand, () => this.CanStart);
            this.PauseCommand = new RelayCommand(this.OnPauseCommand, () => this.CanPause);
            this.StopCommand = new RelayCommand(this.OnStopCommand, () => this.CanStop);

            this._executionService.StateChanged += this.OnServiceStateChanged;
        }


        private void OnServiceStateChanged(MacroExecutionState state)
        {
            (this.StartCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (this.PauseCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (this.StopCommand as RelayCommand)?.RaiseCanExecuteChanged();

            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.CanEditMetadata)));
        }


        protected abstract void OnStartCommand();
        protected abstract void OnPauseCommand();
        protected abstract void OnStopCommand();
    }
}
