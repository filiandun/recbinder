using RecBinderProject.ApplicationLayer.AppSettings.Models;

namespace RecBinderProject.ApplicationLayer.AppSettings
{
    public interface IAppSettingsService
    {
        public AppSettingsLoadState LoadState { get; set; }

        public AppSettingsModel AppSettings { get; set; } // TODO подумать над тем, чтобы убрать публичный set (сделать метод какой-нибудь)

        public void Load();
        public void Save();
        public void Reset();

    }
}
