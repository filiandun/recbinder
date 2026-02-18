using System.Windows;
using ModernWpf.Controls;
using Microsoft.Extensions.DependencyInjection;

using RecBinderProject.Pages;
using RecBinderProject.Models;
using RecBinderProject.ViewModels;
using RecBinderProject.Services.RecordService;
using RecBinderProject.ViewModels.RecordViewModels;
using RecBinderProject.Services.LocalizationService;
using RecBinderProject.Models.RecordingRepository;


namespace RecBinderProject
{
    public partial class MainWindow : Window
    {
        private readonly IRecordingRepository _recordingRepository;
        private readonly ILocalizationService _localizationService;

        private SettingsPage _settingsPage;
        private CreateRecordPage _createRecordPage;

        public MainWindow(IRecordingRepository recordingRepository, ILocalizationService localizationService, SettingsPage settingsPage)
        {
            InitializeComponent();

            this._recordingRepository = recordingRepository;
            this._localizationService = localizationService;

            this._settingsPage = settingsPage;

            // TODO костыль с созданием тут страницы, так как это позволяет сохранять "состояние" страницы (заполненные поля и выбранные настройки)
            RecordingMetadata recordingMetadata = new RecordingMetadata();
            RecordOptions recordOptions = new RecordOptions();

            RecordingService recordingService = new RecordingService(recordingRepository, recordingMetadata, recordOptions);

            CreateRecordViewModel createRecordViewModel = new CreateRecordViewModel(recordingRepository, recordingService, localizationService, "Create");
            this._createRecordPage = new CreateRecordPage(createRecordViewModel);

            this.UpdateRecordings();
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (this.navigationView.SelectedItem is RecordControl recordControl)
            {

                RecordingInput recordingInput = this._recordingRepository.Read(recordControl.RecordName) ?? throw new ArgumentException($"Record '{recordControl.RecordName}' not found");

                RecordOptions recordOptions = new RecordOptions();

                PlaybackService playbackService = new PlaybackService(recordOptions, recordingInput);

                PlayRecordViewModel playRecordViewModel = new PlayRecordViewModel(playbackService, recordingInput, this._localizationService, "Play");

                PlayRecordPage recordPage = new PlayRecordPage(playRecordViewModel);
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
            List<RecordingInput> recordingInputList = this._recordingRepository.LoadAll();

            if (this.navigationView.MenuItems.Count > 0)
            {
                this.navigationView.MenuItems.Clear();
            }

            if (recordingInputList.Count > 0)
            {
                foreach (RecordingInput recordingInput in recordingInputList)
                {
                    this.navigationView.MenuItems.Add(new RecordControl(recordingInput.RecordingMetadata.Name, recordingInput.RecordingMetadata.Description));
                }
            }
        }
    }
}