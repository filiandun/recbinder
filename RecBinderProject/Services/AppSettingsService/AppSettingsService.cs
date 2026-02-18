using System.Diagnostics;
using System.IO;
using System.Text.Json;

using RecBinderProject.Models;


namespace RecBinderProject.Services.AppSettingsService
{
    public class AppSettingsService : IAppSettingsService
    {
        private readonly string _filePath;

        public AppSettings AppSettings { get; set; } // TODO подумать над тем, чтобы убрать публичный set и сделать метод (для того, чтобы не было подвешенного состояния, когда сохранённые настройки в файл отличаются от этих)

        public AppSettingsService() 
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            this._filePath = Path.Combine(baseDirectory, "settings.json");

            this.AppSettings = new AppSettings();

            Load();
        }

        public void Load() // TODO добавить валидацию, если там фигня какая-то, то дефолтные значения выставлять (для конкретных полей или для всего - думай сам потом)
        {
            if (!File.Exists(this._filePath))
            {
                this.AppSettings = new AppSettings();
                Debug.WriteLine("APPSETTINGS LOAD: FILE NOT EXISTS");
                Save();
            }
            else
            {
                string json = File.ReadAllText(this._filePath);
                this.AppSettings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings(); // TODO логировать, что настройки есть, но подтянуть их не получилось, в случае ??

                Debug.WriteLine($"APPSETTINGS LOAD: {json}");
            }
        }

        public void Save()
        {
            string json = JsonSerializer.Serialize(this.AppSettings);     
            File.WriteAllText(this._filePath, json);

            Debug.WriteLine($"APPSETTINGS SAVE: {json}");
        }

        public void Reset()
        {
            Debug.WriteLine($"APPSETTINGS RESET: new APPSETTINGS");

            this.AppSettings = new AppSettings();
        }
    }
}
