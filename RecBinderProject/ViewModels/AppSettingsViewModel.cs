using ModernWpf;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using RecBinderProject.Models;

namespace RecBinderProject.ViewModels
{
    public class AppSettingsViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        private readonly Dictionary<string, List<string>> _errors;
        private AppSettings _appSettings;

        private string _saveDirectory;
        private ApplicationTheme _theme;
        private int _monitorWidth;
        private int _monitorHeight;

        public bool HasErrors => this._errors.Any();

        public string SaveDirectory
        {
            get => this._saveDirectory;
            set
            {
                this.Set(ref this._saveDirectory, value);
                //this.ValidateRecordName();
            }
        }
        public ApplicationTheme Theme
        {
            get => this._theme;
            set
            {
                this.Set(ref this._theme, value);
            }
        }
        public int MonitorWidth
        {
            get => this._monitorWidth;
            set
            {
                this.Set(ref this._monitorWidth, value);
            }
        }
        public int MonitorHeight
        {
            get => this._monitorHeight;
            set
            {
                this.Set(ref this._monitorHeight, value);
            }
        }

        public AppSettingsViewModel(AppSettings appSettings)
        {
            this._errors = new Dictionary<string, List<string>>();

            this.FromModel(appSettings);
        }

        public AppSettings ToModel()
        {
            try
            {
                AppSettings appSettings = new AppSettings(this._saveDirectory, this._theme, this._monitorWidth, this._monitorHeight);

                return appSettings;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void FromModel(AppSettings appSettings)
        {
            this.SaveDirectory = appSettings.SaveDirectory;
            this.Theme = appSettings.Theme;
            this.MonitorWidth = appSettings.MonitorSize.Width;
            this.MonitorHeight = appSettings.MonitorSize.Height;
        }


        public IEnumerable GetErrors(string? propertyName)
        {
            if (this._errors.ContainsKey(propertyName))
            {
                return this._errors[propertyName];
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


        private void Set<T>(ref T field, T value, [CallerMemberName] string propName = "")
        {
            if (!Equals(field, value))
            {
                field = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
