using System.Diagnostics;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;

using RecBinderProject.Domain;

using RecBinderProject.Infrastructure.Repositories;
using RecBinderProject.Infrastructure.Services.AppSettings;
using RecBinderProject.Infrastructure.Services.Localization;

using RecBinderProject.Presentation.Commands;
using RecBinderProject.Presentation.Services;


namespace RecBinderProject.Presentation.ViewModels.MacroLibrary
{
    public class MacroListViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action<Macro?>? SelectedMacroChanged;

        private readonly ILogger<MacroListViewModel> _logger;

        private readonly ILocalizationService _localizationService;
        private readonly IAppSettingsService _appSettingsService;
        private readonly IDialogService _dialogService;

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


        public MacroListViewModel(ILogger<MacroListViewModel> logger, ILocalizationService localizationService, IAppSettingsService appSettingsService, IDialogService dialogService, IMacroRepository macroRepository)
        {
            this._logger = logger;

            this._localizationService = localizationService;

            this._appSettingsService = appSettingsService;
            this._dialogService = dialogService;

            this._macroRepository = macroRepository;

            this._macroItems = new ObservableCollection<MacroListItemViewModel>();

            this.RefreshMacrosCommand = new RelayCommand(async () => await this.LoadMacrosAsync());
            this.OpenSaveFolderCommand = new RelayCommand(this.OnOpenSaveFolderCommand);

            this._macroRepository.MacroSaved += this.OnMacroSaved;
        }


        private void OnMacroSaved(Macro newMacro)
        {
            this._logger.LogDebug("Created new macro. Name: {MacroName}", newMacro.Metadata.Name);

            MacroListItemViewModel newMacroListItemViewModel = new MacroListItemViewModel(newMacro);

            this.MacroItems.Add(newMacroListItemViewModel);
            this.SelectedMacro = newMacroListItemViewModel;
        }

        public async Task LoadMacrosAsync()
        {
            try
            {
                MacroLoadResult loadResult = await Task.Run(() => this._macroRepository.LoadAll());

                this._logger.LogDebug("Loaded macros. Total macros: {Macros}, Failed files: {FailedMacros}.", loadResult.Macros.Count, loadResult.FailedFiles.Count);

                if (loadResult.FailedFiles.Count > 0)
                {
                    string title = this._localizationService.GetString(LocalizationScope.Common, "Dialog_Label_Attention");
                    string message = $"{this._localizationService.GetString(LocalizationScope.MacroList, "Load_Message_FailedFiles")} ({loadResult.FailedFiles.Count}):\n{string.Join("\n", loadResult.FailedFiles)}";

                    this._dialogService.Show(title, message);
                }

                this.MacroItems = new ObservableCollection<MacroListItemViewModel>(loadResult.Macros.Select(m => new MacroListItemViewModel(m)));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Failed to load macros due to an unexpected error.");

                string title = this._localizationService.GetString(LocalizationScope.Common, "Dialog_Label_Attention");
                string message = $"{this._localizationService.GetString(LocalizationScope.MacroList, "Load_Message_UnexpectedError")}";

                this._dialogService.Show(title, message);
            }
        }


        public void OnOpenSaveFolderCommand()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = this._appSettingsService.AppSettings.General.SaveDirectory,
                UseShellExecute = true
            });
        }
    }
}
