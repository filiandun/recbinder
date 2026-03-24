using System.Windows;
using System.Diagnostics;
using System.Collections.ObjectModel;

using ModernWpf.Controls;

using RecBinderProject.Pages;
using RecBinderProject.Models;
using RecBinderProject.ViewModels;
using RecBinderProject.Models.MacroRepository;
using RecBinderProject.Services.LocalizationService;
using RecBinderProject.ViewModels.MacroMetadataViewModels;
using RecBinderProject.ViewModels.MacroExecutionViewModels;
using RecBinderProject.Services.MacroExecutionService.Implementations;


namespace RecBinderProject
{
    public partial class MainWindow : Window
    {
        private readonly IMacroRepository _macroRepository;
        private readonly ILocalizationService _localizationService;

        private AppSettingsPage _settingsPage;
        private RecordingPage _recordingPage;

        public ObservableCollection<Macro> Macros { get; private set; }

        public MainWindow(IMacroRepository recordingRepository, ILocalizationService localizationService, AppSettingsPage settingsPage)
        {
            InitializeComponent();

            this._macroRepository = recordingRepository;
            this._localizationService = localizationService;

            this._settingsPage = settingsPage;

            // TODO костыль с созданием тут страницы, так как это позволяет сохранять "состояние" страницы (заполненные поля и выбранные настройки)

            LocalizationViewModel localizationViewModel = new LocalizationViewModel(localizationService);

            MacroMetadataViewModel macroMetadataViewModel = new MacroMetadataViewModel();

            MacroMetadata macroMetadata = new MacroMetadata();
            MacroOptions macroOptions = new MacroOptions();
            MacroRecordingService macroRecordingService = new MacroRecordingService(recordingRepository, macroMetadata, macroOptions);

            MacroExecutionStatusViewModel macroExecutionStatusViewModel = new MacroExecutionStatusViewModel(macroRecordingService, localizationService, "Create");

            MacroRecordingViewModel macroRecordingViewModel = new MacroRecordingViewModel(localizationViewModel, macroMetadataViewModel, macroExecutionStatusViewModel, macroRecordingService);

            this._recordingPage = new RecordingPage(macroRecordingViewModel);

            this.Macros = new ObservableCollection<Macro>();

            macroRecordingViewModel.MacroRecorded += MacroRecordingViewModel_MacroRecorded;

            this.Macros.CollectionChanged += Macros_CollectionChanged;

            this.MacroRecordingViewModel_MacroRecorded();
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (this.navigationView.SelectedItem is RecordControl recordControl)    
            {
                Macro recordingInput = this._macroRepository.Read(recordControl.RecordName) ?? throw new ArgumentException($"Record '{recordControl.RecordName}' not found");

                MacroOptions recordOptions = new MacroOptions();

                LocalizationViewModel localizationViewModel = new LocalizationViewModel(this._localizationService);

                MacroMetadataViewModel macroMetadataViewModel = new MacroMetadataViewModel();

                MacroPlaybackService macroPlaybackService = new MacroPlaybackService(recordOptions, recordingInput);

                MacroExecutionStatusViewModel macroExecutionStatusViewModel = new MacroExecutionStatusViewModel(macroPlaybackService, this._localizationService, "Play");

                MacroPlaybackViewModel playRecordViewModel = new MacroPlaybackViewModel(localizationViewModel, macroMetadataViewModel, macroExecutionStatusViewModel, macroPlaybackService);

                PlaybackPage recordPage = new PlaybackPage(playRecordViewModel);
                this.frame.Navigate(recordPage);
            }
            else if (this.navigationView.SelectedItem is NavigationViewItem navigationViewItem)
            {
                switch (navigationViewItem.Tag)
                {
                    case "createRecord":
                        this.frame.Navigate(this._recordingPage);
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

        private void MacroRecordingViewModel_MacroRecorded()
        {
            List<Macro> list = this._macroRepository.LoadAll();

            Debug.WriteLine($"NEW MACROS: {list.Count}");

            this.navigationView.MenuItems.Clear();

            this.Macros.Clear();
            foreach (var macro in list)
            {
                this.Macros.Add(macro);
            }
        }

        private void Macros_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Debug.WriteLine("NEW MACRO");

            if (e.NewItems != null)
                foreach (Macro macro in e.NewItems)
                {
                    this.navigationView.MenuItems.Add(new RecordControl(macro.Metadata.Name, macro.Metadata.Description));
                }

            if (e.OldItems != null)
                foreach (Macro macro in e.OldItems)
                {
                    var item = this.navigationView.MenuItems
                        .OfType<RecordControl>()
                        .FirstOrDefault(x => x.RecordName == macro.Metadata.Name);

                    if (item != null)
                    {
                        this.navigationView.MenuItems.Remove(item);
                    }
                }
        }
    }
}