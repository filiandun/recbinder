using ModernWpf;
using System.IO;
using System.Collections;
using System.ComponentModel;

using RecBinderProject.Models;


namespace RecBinderProject.ViewModels.AppSettingsViewModels
{
    public class AppSettingsEditorViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        private readonly Dictionary<string, List<string>> _errors;

        private AppSettings _appSettingsOriginal;
        private AppSettings _appSettingsEditable;

        public bool HasEdits => !this._appSettingsOriginal.Equals(this._appSettingsEditable);
        public bool HasErrors => this._errors.Count != 0;

        public string SaveDirectory
        {
            get => this._appSettingsEditable.SaveDirectory;
            set
            {
                if (!Equals(this._appSettingsEditable.SaveDirectory, value))
                {
                    this._appSettingsEditable.SaveDirectory = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.HasEdits)));

                    this.ValidateSaveDirectory();
                }
            }
        }

        public ApplicationTheme Theme
        {
            get => this._appSettingsEditable.ApplicationTheme;
            set
            {
                if (!Equals(this._appSettingsEditable.ApplicationTheme, value))
                {
                    this._appSettingsEditable.ApplicationTheme = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.HasEdits)));
                }
            }
        }

        public int MonitorWidth
        {
            get => this._appSettingsEditable.MonitorSize.Width;
            set
            {
                if (!Equals(this._appSettingsEditable.MonitorSize.Width, value))
                {
                    this._appSettingsEditable.MonitorSize = new System.Drawing.Size(value, this._appSettingsEditable.MonitorSize.Height);
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.HasEdits)));
                }
            }
        }

        public int MonitorHeight
        {
            get => this._appSettingsEditable.MonitorSize.Height;            
            set
            {
                if (!Equals(this._appSettingsEditable.MonitorSize.Height, value))
                {
                    this._appSettingsEditable.MonitorSize = new System.Drawing.Size(this._appSettingsEditable.MonitorSize.Width, value);
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.HasEdits)));
                }
            }
        }

        public AppSettingsEditorViewModel(AppSettings appSettings)
        {
            this._errors = new Dictionary<string, List<string>>();

            this._appSettingsOriginal = appSettings;
            this._appSettingsEditable = (AppSettings)appSettings.Clone();
        }

        public AppSettings GetValidatedModel()
        {
            //if (this.HasErrors) throw new InvalidOperationException("AppSettingsEditorViewModel has error(s)");

            return (AppSettings)this._appSettingsEditable.Clone();
        }

        public void Save()
        {
            if (this.HasErrors) throw new InvalidOperationException("AppSettingsEditorViewModel has error(s)");

            this._appSettingsOriginal = (AppSettings)_appSettingsEditable.Clone();

            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.HasEdits)));
        }

        public void Cancel()
        {
            this._appSettingsEditable = (AppSettings)this._appSettingsOriginal.Clone();

            this.RaiseAll();
        }

        public void Reset()
        {
            this._appSettingsEditable = new AppSettings();

            this.RaiseAll();
        }


        public IEnumerable GetErrors(string? propertyName)
        {
            if (propertyName != null && this._errors.TryGetValue(propertyName, out List<string>? value))
            {
                return value;
            }

            return null;
        }

        private void AddError(string propertyName, string error)
        {
            if (!this._errors.ContainsKey(propertyName))
            {
                this._errors[propertyName] = new List<string>();
            }
            if (!this._errors[propertyName].Contains(error))
            {
                this._errors[propertyName].Add(error);
                this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        private void ClearErrors(string propertyName)
        {
            if (this._errors.ContainsKey(propertyName))
            {
                this._errors.Remove(propertyName);
                this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        private void ValidateSaveDirectory()
        {
            if (string.IsNullOrWhiteSpace(this._appSettingsEditable.SaveDirectory) || !Directory.Exists(this._appSettingsEditable.SaveDirectory)) 
            {
                this.AddError(nameof(this.SaveDirectory), "Указанный путь некорректен или не существует");
            }
            else
            {
                this.ClearErrors(nameof(this.SaveDirectory));
            }
        }

        private void RaiseAll()
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SaveDirectory)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Theme)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MonitorWidth)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MonitorHeight)));

            this._errors.Clear();
            this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(null));
        }
    }
}