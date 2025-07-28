using System.Windows;
using ModernWpf.Controls;

using RecBinderProject.Pages;


namespace RecBinderProject
{
    public partial class MainWindow : Window
    {
        private SettingsPage _settingsPage;
        private CreateRecordPage _createRecordPage;

        public MainWindow()
        {
            InitializeComponent();

            this.UpdateRecordings();

            this._settingsPage = new SettingsPage();

            this._createRecordPage = new CreateRecordPage();
            this._createRecordPage.OnStopRecording += CreateRecordPage_StopRecording;
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (this.navigationView.SelectedItem is RecordControl recordControl)
            {
                ShowRecordPage recordPage = new ShowRecordPage(recordControl.RecordName);
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
            List<RecordingInput> recordingInputList = ((App)Application.Current).RecordingRepository.LoadAll();
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