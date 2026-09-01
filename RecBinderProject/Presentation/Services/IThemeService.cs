using RecBinderProject.Infrastructure.Services.AppSettings;


namespace RecBinderProject.Presentation.Services
{
    public interface IThemeService
    {
        public void ApplyTheme(AppTheme theme);
    }
}
