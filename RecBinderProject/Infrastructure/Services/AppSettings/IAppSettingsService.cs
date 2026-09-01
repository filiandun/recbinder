namespace RecBinderProject.Infrastructure.Services.AppSettings
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
