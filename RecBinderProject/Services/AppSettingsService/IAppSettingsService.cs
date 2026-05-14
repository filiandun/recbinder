using RecBinderProject.Models;
using RecBinderProject.Models.AppSettings;

namespace RecBinderProject.Services.AppSettingsService
{
    public interface IAppSettingsService
    {
        public AppSettingsLoadState LoadState { get; set; }

        public AppSettings AppSettings { get; set; } // TODO подумать над тем, чтобы убрать публичный set (сделать метод какой-нибудь)

        public void Load();
        public void Save();
        public void Reset();

    }
}
