using System.Windows;
using ModernWpf.Controls;
using Microsoft.Extensions.DependencyInjection;

using RecBinderProject.Pages;
using RecBinderProject.ViewModels;
using RecBinderProject.Models;


namespace RecBinderProject
{
    public partial class MainWindow : Window
    {
        private SettingsPage _settingsPage;
        private CreateRecordPage _createRecordPage;


        public MainWindow()
        {
            InitializeComponent();

            IServiceProvider serviceProvider = ((App)Application.Current).ServiceProvider;

            this._settingsPage = serviceProvider.GetRequiredService<SettingsPage>();
            //this._createRecordPage = serviceProvider.GetRequiredService<CreateRecordPage>();

            IRecordingRepository recordingRepository = serviceProvider.GetRequiredService<IRecordingRepository>();
            this._createRecordPage = new CreateRecordPage(recordingRepository);

            //this._createRecordPage.OnStopRecording += CreateRecordPage_StopRecording;

            this.UpdateRecordings();
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (this.navigationView.SelectedItem is RecordControl recordControl)
            {
                IServiceProvider serviceProvider = ((App)Application.Current).ServiceProvider;
                IRecordingRepository recordingRepository = serviceProvider.GetRequiredService<IRecordingRepository>();

                ShowRecordPage recordPage = new ShowRecordPage(recordingRepository, recordControl.RecordName);
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
        }

        private void CreateRecordPage_StopRecording()
        {
            this.UpdateRecordings();
        }

        private void UpdateRecordings()
        {
            IServiceProvider serviceProvider = ((App)Application.Current).ServiceProvider;
            IRecordingRepository recordingRepository = serviceProvider.GetRequiredService<IRecordingRepository>();

            List<RecordingInput> recordingInputList = recordingRepository.LoadAll();
            this.navigationView.MenuItems.Clear();

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