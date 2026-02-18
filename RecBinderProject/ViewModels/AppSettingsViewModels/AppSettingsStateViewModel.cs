using System.ComponentModel;


namespace RecBinderProject.ViewModels.AppSettingsViewModels
{
    public class AppSettingsStateViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        
        private readonly AppSettingsEditorViewModel _appSettingsEditorViewModel;

        public bool CanSave() => this._appSettingsEditorViewModel.HasEdits && !this._appSettingsEditorViewModel.HasErrors;
        public bool CanCancel() => this._appSettingsEditorViewModel.HasEdits;
        public bool CanReset() => true; // TODO зачем?

        public string StatusText => this.StatusTextContent();

        public AppSettingsStateViewModel(AppSettingsEditorViewModel appSettingsEditorViewModel)
        {
            this._appSettingsEditorViewModel = appSettingsEditorViewModel;

            this._appSettingsEditorViewModel.PropertyChanged += (_, __) => RaiseAll();
            this._appSettingsEditorViewModel.ErrorsChanged += (_, __) => RaiseAll();
        }

        private void RaiseAll()
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanSave)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanCancel)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanReset)));

            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusText)));
        }

        private string StatusTextContent()
        {
            return  this._appSettingsEditorViewModel.HasErrors ? "Есть ошибки" : 
                    this._appSettingsEditorViewModel.HasEdits ? "Есть несохранённые изменения" :
                    "Настройки сохранены";
        }
    }
}
