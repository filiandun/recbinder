using System.Windows;
using RecBinderProject.Infrastructure.Services.AppSettings;


namespace RecBinderProject.Presentation.Services
{
    public class ThemeService : IThemeService
    {
        public void ApplyTheme(AppTheme theme)
        {
#pragma warning disable WPF0001 // TODO getter для Application.Current.ThemeMode - эксперементальная фича, поэтому надо отключить warning 
            switch (theme)
            {
                case AppTheme.System: Application.Current.ThemeMode = ThemeMode.System; return;

                case AppTheme.Dark: Application.Current.ThemeMode = ThemeMode.Dark; return;
                case AppTheme.Light: Application.Current.ThemeMode = ThemeMode.Light; return;
            }
#pragma warning restore WPF0001 // включение warning обратно
        }
    }
}
