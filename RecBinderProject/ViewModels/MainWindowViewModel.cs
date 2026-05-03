using ModernWpf.Controls;

using System.Diagnostics;
using System.ComponentModel;
using System.Collections.ObjectModel;

using RecBinderProject.Views;

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


        public List<Macro> _macros;
        public ObservableCollection<MacroNavigationItemViewModel> MacroItems { get; private set; }


        public MainWindowViewModel(ILocalizationService localizationService, IMacroRepository macroRepository, IMainWindowViewModelFactory viewModelFactory)
        {
            this._localizationService = localizationService;

            this._macroRepository = macroRepository;

            this._viewModelFactory = viewModelFactory;

            this._macros = new();
            this.MacroItems = new();

            this.RefreshMacros();
        }


        public void OpenPlayback(Macro macro)
        {
            this.CurrentViewModel = this._viewModelFactory.CreatePlayback(macro);
        }

        public void OpenRecording()
        {
            this.CurrentViewModel = this._viewModelFactory.CreateRecording();
        }

        //public void OpenHelp()
        //{
        //    //this.CurrentViewModel = this._viewModelFactory.GetHelp();
        //}

        public void OpenSettings()
        {
            this.CurrentViewModel = this._viewModelFactory.GetAppSettings();
        }


        public void RefreshMacros()
        {
            MacroLoadResult macroLoadResult = this._macroRepository.LoadAll();

            Debug.WriteLine($"NEW MACROS: {macroLoadResult.Macros.Count}");

            this._macros = macroLoadResult.Macros;

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
            ObservableCollection<MacroNavigationItemViewModel> macroControls = new();

            foreach (Macro macro in this._macros)
            {
                macroControls.Add(new MacroNavigationItemViewModel(macro));
            }

            this.MacroItems = macroControls;

            Debug.WriteLine($"update macros controls {this._macros.Count}");
        }
    }
}
