using System.Diagnostics;
using System.ComponentModel;
using System.Collections.ObjectModel;

using ModernWpf.Controls;

using RecBinderProject.Domain;
using RecBinderProject.Presentation.Factories;

using RecBinderProject.Infrastructure.MacroRepository;
using RecBinderProject.Infrastructure.LocalizationService;


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
                Debug.WriteLine("MacroItems update");
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.MacroItems)));
            }
        }


        public MacroListViewModel(ILocalizationService localizationService, IMacroRepository macroRepository, IMainWindowViewModelFactory viewModelFactory)
        {
            this._macroRepository = macroRepository;

            this._macroItems = new ObservableCollection<MacroListItemViewModel>();
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
