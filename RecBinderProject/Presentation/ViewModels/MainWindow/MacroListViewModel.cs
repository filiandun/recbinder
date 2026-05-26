using System.Diagnostics;
using System.ComponentModel;
using System.Collections.ObjectModel;

using ModernWpf.Controls;

using RecBinderProject.Domain;
using RecBinderProject.Presentation.Factories;

using RecBinderProject.Infrastructure.MacroRepository;
using RecBinderProject.Infrastructure.LocalizationService;
using RecBinderProject.Presentation.Commands;


namespace RecBinderProject.Presentation.ViewModels.MainWindow
{
    public class MacroListViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

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

        public int MacrosCount => this._macroItems.Count;

        public RelayCommand RefreshMacrosCommand { get; private set; }
        public RelayCommand OpenSaveFolderCommand { get; private set; }


        public MacroListViewModel(ILocalizationService localizationService, IMacroRepository macroRepository, IMainWindowViewModelFactory viewModelFactory)
        {
            this._macroRepository = macroRepository;

            this._macroItems = new ObservableCollection<MacroListItemViewModel>();

            this.RefreshMacrosCommand = new RelayCommand(this.RefreshMacros);
            this.OpenSaveFolderCommand = new RelayCommand(this.OpenSaveFolder);
        }


        public void RefreshMacros()
        {
            MacroLoadResult macroLoadResult = this._macroRepository.LoadAll();

            Debug.WriteLine($"NEW MACROS: {macroLoadResult.Macros.Count}");

            if (macroLoadResult.FailedFiles.Count > 0)
            {
                ContentDialog contentDialog = new ContentDialog()
                {
                    Content = $"Некоторые из записей не были загружены ({macroLoadResult.FailedFiles.Count})",
                    CloseButtonText = "Закрыть",
                };
                contentDialog.ShowAsync();
            }

            this.UpdateMacrosControls(macroLoadResult.Macros);
        }

        public void OpenSaveFolder()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = this._macroRepository.SaveDirectory,
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
