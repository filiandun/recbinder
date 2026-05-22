using System.IO;
using Microsoft.Win32;
using System.Collections;
using System.Windows.Input;
using System.Windows.Forms;
using System.ComponentModel;

using ModernWpf;

using RecBinderProject.Presentation.Commands;
using RecBinderProject.Infrastructure.AppSettingsService;
using System.Diagnostics;
using RecBinderProject.Presentation.Services;


namespace RecBinderProject.Presentation.ViewModels.AppSettings
{
    public class GeneralAppSettingsSectionViewModel : IAppSettingsSectionViewModel, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        private readonly Dictionary<string, List<string>> _errors;

        private readonly IAppSettingsService _appSettingsService;
        private readonly IThemeService _themeService;

        private GeneralAppSettings _generalAppSettingsOriginal;
        private GeneralAppSettings _generalAppSettingsEditable;

        public ICommand BrowseSaveDirectoryCommand { get; }

        public bool IsSaveAllowed => !this.HasErrors;
        public bool IsCancelAllowed => this.HasEdits;
        public bool IsResetAllowed => !this._generalAppSettingsEditable.Equals(GeneralAppSettings.Default()); // TODO: подумать над названием, сейчас оно не соответствует действию

        public bool HasEdits => !this._generalAppSettingsOriginal.Equals(this._generalAppSettingsEditable);
        public bool HasErrors => this._errors.Count != 0;

        public string StatusText => this.StatusTextContent(); // TODO: не реализовано, так как нет уведомления об изменении

        public string SaveDirectory
        {
            get => this._generalAppSettingsEditable.SaveDirectory;
            set
            {
                if (!Equals(this._generalAppSettingsEditable.SaveDirectory, value))
                {
                    this._generalAppSettingsEditable.SaveDirectory = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.HasEdits)));
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.SaveDirectory)));

                    this.ValidateSaveDirectory();
                }
            }
        }

        public AppTheme AppTheme
        {
            get => this._generalAppSettingsEditable.AppTheme;
            set
            {
                if (!Equals(this._generalAppSettingsEditable.AppTheme, value))
                {
                    this._generalAppSettingsEditable.AppTheme = value;

                    this._themeService.ApplyTheme(value);

                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.HasEdits)));
                }
            }
        }

        public int MonitorWidth
        {
            get => this._generalAppSettingsEditable.MonitorSize.Width;
            set
            {
                if (!Equals(this._generalAppSettingsEditable.MonitorSize.Width, value))
                {
                    this._generalAppSettingsEditable.MonitorSize = new System.Drawing.Size(value, this._generalAppSettingsEditable.MonitorSize.Height);
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.HasEdits)));
                }
            }
        }

        public int MonitorHeight
        {
            get => this._generalAppSettingsEditable.MonitorSize.Height;            
            set
            {
                if (!Equals(this._generalAppSettingsEditable.MonitorSize.Height, value))
                {
                    this._generalAppSettingsEditable.MonitorSize = new System.Drawing.Size(this._generalAppSettingsEditable.MonitorSize.Width, value);
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.HasEdits)));
                }
            }
        }

        public AppTheme[] AvailableAppThemes => (AppTheme[])Enum.GetValues(typeof(AppTheme));


        public Screen[] AvailableScreenResolutions => Screen.AllScreens; // TODO: всё, что связано с разрешением должно быть в Advanced, а не тут
        public String ScreenResolution
        {
            get => $"{this._generalAppSettingsEditable.MonitorSize.Width} x {this._generalAppSettingsEditable.MonitorSize.Height}";
        }


        public GeneralAppSettingsSectionViewModel(IAppSettingsService appSettingsService, IThemeService themeService)
        {
            this._errors = new Dictionary<string, List<string>>();

            this._appSettingsService = appSettingsService;
            this._themeService = themeService;

            this._generalAppSettingsOriginal = this._appSettingsService.AppSettings.General;
            this._generalAppSettingsEditable = (GeneralAppSettings)this._generalAppSettingsOriginal.Clone();

            this.BrowseSaveDirectoryCommand = new RelayCommand(BrowseSaveDirectory);
        }


        public void SaveLocal()
        {
            if (this.HasErrors) throw new InvalidOperationException("GeneralAppSettingsSectionsViewModel has error(s)");

            this._generalAppSettingsOriginal = (GeneralAppSettings)_generalAppSettingsEditable.Clone();

            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.HasEdits)));
        }

        public void ApplyToService()
        {
            if (this.HasErrors) throw new InvalidOperationException("GeneralAppSettingsSectionsViewModel has error(s)");

            this._appSettingsService.AppSettings.General = (GeneralAppSettings)this._generalAppSettingsEditable.Clone();
        }

        public void CancelLocal()
        {
            this._generalAppSettingsEditable = (GeneralAppSettings)this._generalAppSettingsOriginal.Clone();

            this.RaiseAll();
        }

        public void ResetLocal() // TODO: не сбрасываются ошибки на самой странице (выделение поля желтым остаётся)
        {
            this._generalAppSettingsEditable = GeneralAppSettings.Default();

            this.RaiseAll();
        }

        public void BrowseSaveDirectory()
        {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog();
            if (openFolderDialog.ShowDialog() == true)
            {
                this.SaveDirectory = openFolderDialog.FolderName;
            }
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
            if (string.IsNullOrWhiteSpace(this._generalAppSettingsEditable.SaveDirectory) || !Directory.Exists(this._generalAppSettingsEditable.SaveDirectory)) 
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
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.SaveDirectory)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.AppTheme)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.MonitorWidth)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.MonitorHeight)));

            this._errors.Clear();
            this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(null));
        }

        private string StatusTextContent()
        {
            return  this.HasErrors ? "Есть ошибки" :
                    this.HasEdits ? "Есть несохранённые изменения" :
                    "Настройки сохранены";
        }
    }
}