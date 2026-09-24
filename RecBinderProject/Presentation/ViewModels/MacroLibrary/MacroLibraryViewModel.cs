using System.ComponentModel;

using RecBinderProject.Domain;

using RecBinderProject.Presentation.Factories;
using RecBinderProject.Presentation.ViewModels.Localization;
using RecBinderProject.Presentation.ViewModels.MacroExecution;


namespace RecBinderProject.Presentation.ViewModels.MacroLibrary
{
    public class MacroLibraryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
 
        private readonly IMacroLibraryViewModelFactory _viewModelFactory;

        public LocalizationViewModel LocalizationViewModel { get; private set; }
        public MacroLibraryListViewModel MacroListViewModel { get; private set; }

        private MacroPlaybackViewModel? _macroPlaybackViewModel;
        public MacroPlaybackViewModel? MacroPlaybackViewModel
        {
            get => this._macroPlaybackViewModel;
            set
            {
                if (this._macroPlaybackViewModel != value)
                {
                    this._macroPlaybackViewModel = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.MacroPlaybackViewModel)));
                }
            }
        }


        public MacroLibraryViewModel(IMacroLibraryViewModelFactory viewModelFactory, LocalizationViewModel localizationViewModel, MacroLibraryListViewModel macroListViewModel, MacroPlaybackViewModel? macroPlaybackViewModel = null)
        {
            this._viewModelFactory = viewModelFactory;

            this.LocalizationViewModel = localizationViewModel;

            this.MacroListViewModel = macroListViewModel;
            this.MacroPlaybackViewModel = macroPlaybackViewModel;

            this.MacroListViewModel.SelectedMacroChanged += this.OnMacroSelected;
        }


        public async Task InitializeAsync()
        {
            await this.MacroListViewModel.LoadMacrosAsync();
        }


        private void OnMacroSelected(Macro? macro)
        {
            if (macro is not null)
            {
                this.MacroPlaybackViewModel = this._viewModelFactory.CreatePlayback(macro);
            }
            else
            {
                this.MacroPlaybackViewModel = null;
            }
        }
    }
}
