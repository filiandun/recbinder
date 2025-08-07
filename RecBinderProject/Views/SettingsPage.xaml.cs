using Microsoft.Win32;
using ModernWpf;
using RecBinderProject.Models;
using RecBinderProject.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RecBinderProject.Pages
{
    /// <summary>
    /// Логика взаимодействия для SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        private readonly ISettingsService _settingsService;

        public SettingsPage(ISettingsService settingsService)
        {
            InitializeComponent();

            this._settingsService = settingsService;

            foreach (AppTheme appTheme in Enum.GetValues(typeof(AppTheme)))
            {
                this.themeSelector.Items.Add(appTheme);
            }

            this.pathToRecordsTextBox.Text = this._settingsService.AppSettings.SaveDirectory;
        }

        public void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.themeSelector.SelectedItem is AppTheme selectedAppTheme)
            {
                this._settingsService.AppSettings.Theme = (AppTheme)this.themeSelector.SelectedItem;

                ThemeManager.Current.ApplicationTheme = selectedAppTheme == AppTheme.Dark ? ApplicationTheme.Dark : ApplicationTheme.Light;
            }

            this._settingsService.AppSettings.SaveDirectory = this.pathToRecordsTextBox.Text;
            this._settingsService.Save();
        }

        public void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.themeSelector.Text = this._settingsService.AppSettings.Theme.ToString();
            this.pathToRecordsTextBox.Text = this._settingsService.AppSettings.SaveDirectory;
        }

        public void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            this._settingsService.AppSettings = new AppSettings();
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
