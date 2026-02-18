using RecBinderProject.Models;

namespace RecBinderProject.Services.AppSettingsService
{
    public interface IAppSettingsService
    {
        public AppSettings AppSettings { get; set; }

        public void Load();
        public void Save();
        public void Reset();

    }
}
