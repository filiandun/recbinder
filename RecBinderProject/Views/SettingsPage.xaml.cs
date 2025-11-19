using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;

using RecBinderProject.Services;
using RecBinderProject.ViewModels;

namespace RecBinderProject.Pages
{
    public partial class SettingsPage : Page
    {
        private SettingsViewModel _settingsViewModel;

        public SettingsPage(ISettingsService settingsService)
        {
            InitializeComponent();

            this._settingsViewModel = new SettingsViewModel(settingsService);
            this.DataContext = this._settingsViewModel;

            this.themeSelector.Items.Clear();
            this.themeSelector.ItemsSource = this._settingsViewModel.GetThemes();
        }

        //public void SaveButton_Click(object sender, RoutedEventArgs e)
        //{
        //    ThemeManager.Current.ApplicationTheme = selectedAppTheme == AppTheme.Dark ? ApplicationTheme.Dark : ApplicationTheme.Light;
        //}

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
