using System.Diagnostics;
using System.ComponentModel;
using System.Collections.ObjectModel;

using RecBinderProject.Domain;

using RecBinderProject.Presentation.Commands;
using RecBinderProject.Infrastructure.Repositories;
using RecBinderProject.Infrastructure.Services.AppSettings;


namespace RecBinderProject.Presentation.ViewModels.MacroLibrary
{
    public class MacroListViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action<Macro?>? SelectedMacroChanged;

        private readonly IAppSettingsService _appSettingsService;
        private readonly IMacroRepository _macroRepository;


        private ObservableCollection<MacroListItemViewModel> _macroItems;
        public ObservableCollection<MacroListItemViewModel> MacroItems
        {
            get => this._macroItems;
            set
            {
                if (ReferenceEquals(this._macroItems, value)) return;

                this._macroItems = value;

                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.MacroItems)));
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.MacrosCount)));
            }
        }

        private MacroListItemViewModel? _selectedMacro;
        public MacroListItemViewModel? SelectedMacro
        {
            get => this._selectedMacro;
            set
            {
                if (ReferenceEquals(this._selectedMacro, value)) return;

                this._selectedMacro = value;

                this.SelectedMacroChanged?.Invoke(this._selectedMacro?.Macro);
            }
        }

        public int MacrosCount => this._macroItems.Count;

        public RelayCommand RefreshMacrosCommand { get; private set; }
        public RelayCommand OpenSaveFolderCommand { get; private set; }


        public MacroListViewModel(IAppSettingsService appSettingsService, IMacroRepository macroRepository)
        {
            this._appSettingsService = appSettingsService;
            this._macroRepository = macroRepository;

            this._macroItems = new ObservableCollection<MacroListItemViewModel>();

            this.RefreshMacrosCommand = new RelayCommand(this.RefreshMacros);
            this.OpenSaveFolderCommand = new RelayCommand(this.OpenSaveFolder);

            this._macroRepository.MacroSaved += OnMacroSaved;
        }

        private void OnMacroSaved() => this.RefreshMacros();

        public void RefreshMacros()
        {
            MacroLoadResult macroLoadResult = this._macroRepository.LoadAll();

            Debug.WriteLine($"NEW MACROS: {macroLoadResult.Macros.Count}");

            if (macroLoadResult.FailedFiles.Count > 0)
            {
                //ContentDialog contentDialog = new ContentDialog()
                //{
                //    Content = $"Некоторые из записей не были загружены ({macroLoadResult.FailedFiles.Count})",
                //    CloseButtonText = "Закрыть",
                //};
                //contentDialog.ShowAsync();
            }

            this.UpdateMacrosControls(macroLoadResult.Macros);
        }

        public void OpenSaveFolder()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = this._appSettingsService.AppSettings.General.SaveDirectory,
                UseShellExecute = true
            });
        }


        private void UpdateMacrosControls(List<Macro> macros)
        {
            ObservableCollection<MacroListItemViewModel> macroItems = new();

            foreach (Macro macro in macros)
            {
                macroItems.Add(new MacroListItemViewModel(macro));
            }

            this.MacroItems = macroItems;

            Debug.WriteLine($"update macros controls {macros.Count}");
        }
    }
}
