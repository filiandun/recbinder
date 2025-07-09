using Gma.System.MouseKeyHook;// библиотека для считывания нажатия клавиш и движения мышью
using ModernWpf.Controls;

using System.Windows.Forms;

using ModernWpf;
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
            if (this.navigationView.SelectedItem is RecordedEventControl recordedEventControl)
            {
                RecordPage recordPage = new RecordPage("RecordName", 20, DateTime.Now, "RecordDescription");
                this.frame.Navigate(recordPage);
            }
            else if (this.navigationView.SelectedItem is NavigationViewItem navigationViewItem)
            {
                switch (navigationViewItem.Tag)
                {
                    case "createRecord":
                        RecordingPage recordingPage = new RecordingPage();
                        this.frame.Navigate(recordingPage);
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
    }
}