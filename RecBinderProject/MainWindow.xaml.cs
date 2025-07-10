using ModernWpf.Controls;
using System.Windows;

namespace RecBinderProject
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
                        CreateRecordPage createRecordPage = new CreateRecordPage();
                        this.frame.Navigate(createRecordPage);
                        createRecordPage.StopRecording += RecordingPage_StopRecording;

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

        private void RecordingPage_StopRecording(object? sender, EventArgs e)
        {
            if (sender is CreateRecordPage createRecordPage)
            {
                this.navigationView.MenuItems.Add(new RecordControl(createRecordPage.RecordingInput));
            }
            else
            {
                throw new ArgumentException("sender должен быть типа CreateRecordPage", nameof(sender));
            }
        }
    }
}