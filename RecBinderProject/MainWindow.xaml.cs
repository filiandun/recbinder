using ModernWpf.Controls;
using System.Windows;

namespace RecBinderProject
{
    public partial class MainWindow : Window
    {
        private RecordingRepository _recordingRepository;

        public MainWindow()
        {
            InitializeComponent();

            this._recordingRepository = new RecordingRepository("C:\\Repositories\\RecBinder\\RecBinderProject\\Recordings");
            this.UpdateRecordings();
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (this.navigationView.SelectedItem is RecordControl recordControl)
            {
                RecordPage recordPage = new RecordPage(recordControl);
                this.frame.Navigate(recordPage);
            }
            else if (this.navigationView.SelectedItem is NavigationViewItem navigationViewItem)
            {
                switch (navigationViewItem.Tag)
                {
                    case "createRecord":
                        CreateRecordPage createRecordPage = new CreateRecordPage(this._recordingRepository);
                        this.frame.Navigate(createRecordPage);
                        createRecordPage.OnStopRecording += CreateRecordPage_StopRecording;

                        break;

                    case "help":

                        break;

                    case "settings":

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

        private void CreateRecordPage_StopRecording(object? sender, EventArgs e)
        {
            this.UpdateRecordings();
        }

        private void UpdateRecordings()
        {
            List<RecordingInput> recordings = this._recordingRepository.LoadAll();
            this.navigationView.MenuItems.Clear();

            foreach (RecordingInput recording in recordings)
            {
                this.navigationView.MenuItems.Add(new RecordControl(recording));
            }
        }
    }
}