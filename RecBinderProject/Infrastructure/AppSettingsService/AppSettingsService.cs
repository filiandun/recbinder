using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;


namespace RecBinderProject.Infrastructure.AppSettingsService
{
    public class AppSettingsService : IAppSettingsService
    {
        private readonly ILogger<AppSettingsService> _logger;

        private readonly string _fileName;
        private readonly string _filePath;

        public AppSettingsLoadState LoadState { get; set; }

        public AppSettings AppSettings { get; set; } // TODO подумать над тем, чтобы убрать публичный set и сделать метод (для того, чтобы не было подвешенного состояния, когда сохранённые настройки в файл отличаются от этих)


        public AppSettingsService(ILogger<AppSettingsService> logger) 
        {
            this._logger = logger;

            this._fileName = "settings.json";
            this._filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this._fileName);

            this.AppSettings = new AppSettings();

            this.LoadState = AppSettingsLoadState.NotLoaded;
        }

        
        public void Load()
        {
            if (!File.Exists(this._filePath))
            {
                this._logger.LogInformation("App settings file not found, creating default file {AppSettingsFile}", this._filePath);

                this.AppSettings = new AppSettings();
                this.Save();

                this.LoadState = AppSettingsLoadState.MissingFile;

                return;
            }

            try
            {
                this._logger.LogInformation("Loading app settings from {AppSettingsFile}", this._filePath);

                using FileStream fileStream = File.OpenRead(this._filePath);
                AppSettings? loadedSettings = JsonSerializer.Deserialize<AppSettings>(fileStream);

                this.AppSettings = loadedSettings ?? new AppSettings();

                this.LoadState = AppSettingsLoadState.LoadedFromFile;
            }
            catch (JsonException ex)
            {
                this._logger.LogError(ex, "Failed to parse app settings file {AppSettingsFile}", this._filePath);
                 
                this.AppSettings = new AppSettings();
                this.Save();

                this.LoadState = AppSettingsLoadState.CorruptedFile;
            }
            catch (IOException ex)
            {   
                this._logger.LogError(ex, "Failed to read app settings file {AppSettingsFile}", this._filePath);
                this.AppSettings = new AppSettings();

                this.LoadState = AppSettingsLoadState.CorruptedFile;
            }
            catch (UnauthorizedAccessException ex)
            {
                this._logger.LogError(ex, "No access to app settings file {AppSettingsFile}", this._filePath);
                this.AppSettings = new AppSettings();

                this.LoadState = AppSettingsLoadState.AccessDenied;
            }
        }

        public void Save()
        {
            try
            {
                this._logger.LogInformation("Saving app settings to {AppSettingsFile}", this._filePath);

                using FileStream stream = File.Create(this._filePath);
                JsonSerializer.Serialize(stream, this.AppSettings, new JsonSerializerOptions { WriteIndented = true });
            }
            catch (JsonException ex)
            {
                this._logger.LogError(ex, "Failed to parse app settings file {AppSettingsFile}", this._filePath);

                this.AppSettings = new AppSettings();
                this.Save();
            }
            catch (IOException ex)
            {
                this._logger.LogError(ex, "Failed to create app settings file {AppSettingsFile}", this._filePath);
                this.AppSettings = new AppSettings();
            }
            catch (UnauthorizedAccessException ex)
            {
                this._logger.LogError(ex, "No access to app settings file {AppSettingsFile}", this._filePath);
                this.AppSettings = new AppSettings();
            }

        }

        public void Reset()
        {
            this._logger.LogInformation("Reset app settings");

            this.AppSettings = new AppSettings();
        }
    }
}