using ModernWpf.Controls;

using System.Diagnostics;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections.ObjectModel;

using RecBinderProject.Models;
using RecBinderProject.Models.MacroRepository;
using RecBinderProject.Services.LocalizationService;
using RecBinderProject.Services.MainWindowViewModelFactory;


namespace RecBinderProject.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly ILocalizationService _localizationService;

        private readonly IMacroRepository _macroRepository;

        private readonly IMainWindowViewModelFactory _viewModelFactory;


        public List<Macro> _macros;
        public ObservableCollection<MacroControl> MacroControls { get; private set; }


        private MacroControl? _selectedMacro;
        public MacroControl? SelectedMacro
        {
            get => this._selectedMacro;
            set
            {
                if (this._selectedMacro is not null && this._selectedMacro.Equals(value)) return;

                this._selectedMacro = value;
                this.MacroSelectionChanged();
            }
        }

        private object? _currentViewModel;

        public object? CurrentViewModel
        {
            get => this._currentViewModel;
            set
            {
                if (Equals(this._currentViewModel, value)) return;

                this._currentViewModel = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.CurrentViewModel)));
            }
        }


        public ICommand OpenRecordingCommand { get; set; }
        public ICommand OpenSettingsCommand { get; set; }


        public MainWindowViewModel(ILocalizationService localizationService, IMacroRepository macroRepository, IMainWindowViewModelFactory viewModelFactory)
        {
            this._localizationService = localizationService;

            this._macroRepository = macroRepository;

            this._viewModelFactory = viewModelFactory;

            this._macros = new();

            this.OpenRecordingCommand = new RelayCommand(this.OnOpenRecordingCommand);
            this.OpenSettingsCommand = new RelayCommand(this.OnOpenSettingsCommand);

            this.RefreshMacrosCommand();
        }

        public void MacroSelectionChanged()
        {
            if (this.SelectedMacro is null) throw new ArgumentNullException("selected macro is null");

            Macro macro = this._macroRepository.Read(this.SelectedMacro.MacroName) ?? throw new ArgumentNullException("selected macro is not found");

            this.CurrentViewModel = this._viewModelFactory.CreatePlayback(macro);
        }

        private void OnOpenRecordingCommand()
        {
            this.CurrentViewModel = this._viewModelFactory.CreateRecording();
        }

        private void OnOpenSettingsCommand()
        {
            this.CurrentViewModel = this._viewModelFactory.GetAppSettings();
        }


        public void RefreshMacrosCommand()
        {
            MacroLoadResult macroLoadResult = this._macroRepository.LoadAll();

            Debug.WriteLine($"NEW MACROS: {macroLoadResult.Macros.Count}");

            this._macros.Clear();
            foreach (Macro macro in macroLoadResult.Macros)
            {
                this._macros.Add(macro);
            }

            if (macroLoadResult.FailedFiles.Count > 0)
            {
                ContentDialog contentDialog = new ContentDialog()
                {
                    Content = $"Некоторые из записей не были загружены ({macroLoadResult.FailedFiles.Count})",
                    CloseButtonText = "Закрыть",
                };
                contentDialog.ShowAsync();
            }

            this.UpdateMacrosControls();
        }


        private void UpdateMacrosControls()
        {
            ObservableCollection<MacroControl> macroControls = new();

            foreach (Macro macro in this._macros)
            {
                macroControls.Add(new MacroControl(macro.Metadata.Name, macro.Metadata.Description));
            }

            this.MacroControls = macroControls;

            Debug.WriteLine($"update macros controls {this._macros.Count}");
        }
    }
}
