using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;

using RecBinderProject.Services.AppSettingsService;
using RecBinderProject.ViewModels.AppSettingsViewModels;

namespace RecBinderProject.Pages
{
    public partial class SettingsPage : Page
    {
        private AppSettingsViewModel _settingsViewModel;

        public SettingsPage(IAppSettingsService settingsService)
        {
            InitializeComponent();

            this._settingsViewModel = new AppSettingsViewModel(settingsService);
            this.DataContext = this._settingsViewModel;

            this.themeSelector.Items.Clear();
            this.themeSelector.ItemsSource = this._settingsViewModel.GetThemes();
        }

        private void OpenFileDialogButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog();
            if ((bool)openFolderDialog.ShowDialog())
            {
                this.pathToRecordsTextBox.Text = openFolderDialog.FolderName;
            }
        }
    }
}
