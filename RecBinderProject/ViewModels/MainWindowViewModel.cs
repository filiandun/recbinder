using ModernWpf.Controls;

using System.Diagnostics;
using System.ComponentModel;
using System.Collections.ObjectModel;

using RecBinderProject.Models;
using RecBinderProject.Models.MacroRepository;

using RecBinderProject.Services.LocalizationService;
using RecBinderProject.Services.MainWindowViewModelFactory;
using RecBinderProject.ViewModels.MacroExecutionViewModels;


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


        private ObservableCollection<MacroNavigationItemViewModel> _macroItems;
        public ObservableCollection<MacroNavigationItemViewModel> MacroItems
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


        public MainWindowViewModel(ILocalizationService localizationService, IMacroRepository macroRepository, IMainWindowViewModelFactory viewModelFactory)
        {
            this._localizationService = localizationService;

            this._macroRepository = macroRepository;

            this._viewModelFactory = viewModelFactory;

            this._macroItems = new ObservableCollection<MacroNavigationItemViewModel>();

            this.RefreshMacros();
        }

        private void OnMacroRecorded()
        {
            Debug.WriteLine("MACRO RECORDED: REFRESH");
            this.RefreshMacros();
        }


        public void OpenPlayback(Macro macro)
        {
            this.CurrentViewModel = this._viewModelFactory.CreatePlayback(macro);
        }

        public void OpenRecording()
        {
            MacroRecordingViewModel recordingViewModel = this._viewModelFactory.CreateRecording();
            this.CurrentViewModel = recordingViewModel;

            recordingViewModel.MacroRecorded += this.OnMacroRecorded;
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
            ObservableCollection<MacroNavigationItemViewModel> macroItems = new();

            foreach (Macro macro in macros)
            {
                macroItems.Add(new MacroNavigationItemViewModel(macro));
            }

            this.MacroItems = macroItems;

            Debug.WriteLine($"update macros controls {macros.Count}");
        }
    }
}
