using RecBinderProject.Infrastructure.AppSettingsService;


namespace RecBinderProject.Presentation.Services
{
    public interface IThemeService
    {
        public void ApplyTheme(AppTheme theme);
    }
}
