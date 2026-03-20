using System.Windows.Controls;

using RecBinderProject.Services.AppSettingsService;
using RecBinderProject.ViewModels.AppSettingsViewModels;

namespace RecBinderProject.Pages
{
    public partial class AppSettingsPage : Page
    {
        private AppSettingsViewModel _appSettingsViewModel;

        public AppSettingsPage(IAppSettingsService settingsService)
        {
            InitializeComponent();

            this._appSettingsViewModel = new AppSettingsViewModel(settingsService);
            this.DataContext = this._appSettingsViewModel;

            this.themeSelector.Items.Clear();
            this.themeSelector.ItemsSource = this._appSettingsViewModel.GetThemes();
        }
    }
}
