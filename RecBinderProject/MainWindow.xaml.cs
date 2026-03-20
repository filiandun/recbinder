using System.Windows;
using ModernWpf.Controls;
using Microsoft.Extensions.DependencyInjection;

using RecBinderProject.Pages;
using RecBinderProject.Models;
using RecBinderProject.ViewModels;
using RecBinderProject.Services.RecordService;
using RecBinderProject.ViewModels.RecordViewModels;
using RecBinderProject.Services.LocalizationService;
using RecBinderProject.Models.MacroRepository;


namespace RecBinderProject
{
    public partial class MainWindow : Window
    {
        private readonly IMacroRepository _recordingRepository;
        private readonly ILocalizationService _localizationService;

        private AppSettingsPage _settingsPage;
        private RecordingPage _createRecordPage;

        public MainWindow(IMacroRepository recordingRepository, ILocalizationService localizationService, AppSettingsPage settingsPage)
        {
            InitializeComponent();

            this._recordingRepository = recordingRepository;
            this._localizationService = localizationService;

            this._settingsPage = settingsPage;

            // TODO костыль с созданием тут страницы, так как это позволяет сохранять "состояние" страницы (заполненные поля и выбранные настройки)
            MacroMetadata recordingMetadata = new MacroMetadata();
            MacroOptions recordOptions = new MacroOptions();

            MacroRecordService recordingService = new MacroRecordService(recordingRepository, recordingMetadata, recordOptions);

            MacroRecordingViewModel createRecordViewModel = new MacroRecordingViewModel(recordingRepository, recordingService, localizationService, "Create");
            this._createRecordPage = new RecordingPage(createRecordViewModel);

            this.UpdateRecordings();
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (this.navigationView.SelectedItem is RecordControl recordControl)
            {

                Macro recordingInput = this._recordingRepository.Read(recordControl.RecordName) ?? throw new ArgumentException($"Record '{recordControl.RecordName}' not found");

                MacroOptions recordOptions = new MacroOptions();

                MacroPlaybackService playbackService = new MacroPlaybackService(recordOptions, recordingInput);

                MacroPlaybackViewModel playRecordViewModel = new MacroPlaybackViewModel(playbackService, recordingInput, this._localizationService, "Play");

                PlaybackPage recordPage = new PlaybackPage(playRecordViewModel);
                this.frame.Navigate(recordPage);
            }
            else if (this.navigationView.SelectedItem is NavigationViewItem navigationViewItem)
            {
                switch (navigationViewItem.Tag)
                {
                    case "createRecord":
                        this.frame.Navigate(this._createRecordPage);
                        break;

                    case "help":

                        break;

                    case "Параметры":
                        this.frame.Navigate(this._settingsPage);

                        break;
                }
            }
            else if (this.navigationView.SelectedItem == null)
            {
                ContentDialog contentDialog = new ContentDialog()
                {
                    Content = "Вы ничего не выбрали",
                    CloseButtonText = "Закрыть",
                };
                contentDialog.ShowAsync();
            }

            //this.UpdateRecordings();
        }


        private void UpdateRecordings()
        {
            List<Macro> recordingInputList = this._recordingRepository.LoadAll();

            if (this.navigationView.MenuItems.Count > 0)
            {
                this.navigationView.MenuItems.Clear();
            }

            if (recordingInputList.Count > 0)
            {
                foreach (Macro recordingInput in recordingInputList)
                {
                    this.navigationView.MenuItems.Add(new RecordControl(recordingInput.Metadata.Name, recordingInput.Metadata.Description));
                }
            }
        }
    }
}