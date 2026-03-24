using System.Windows.Controls;

using RecBinderProject.Services.AppSettingsService;
using RecBinderProject.ViewModels.AppSettingsViewModels;

namespace RecBinderProject.Pages
{
    public partial class AppSettingsPage : Page
    {
        public AppSettingsPage(AppSettingsViewModel appSettingsViewModel)
        {
            InitializeComponent();

            this.DataContext = appSettingsViewModel;
        }
    }
}
