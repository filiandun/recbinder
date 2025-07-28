using ModernWpf;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void ThemeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.themeSelector.SelectedItem is ComboBoxItem item && item.Tag is string tag)
            {
                AppTheme selectedTheme = tag == "Light" ? AppTheme.Light : AppTheme.Dark;

                ((App)Application.Current).SettingsService.AppSettings.Theme = selectedTheme;
                ((App)Application.Current).SettingsService.Save();

                ThemeManager.Current.ApplicationTheme = selectedTheme == AppTheme.Dark  ? ApplicationTheme.Dark : ApplicationTheme.Light;
            }
        }

    }
}
