using RecBinderProject.Models;
using System.IO;
using System.Text.Json;

namespace RecBinderProject.Services
{
    public class SettingsService : ISettingsService
    {
        private string _filePath;
        public AppSettings AppSettings { set; get; }

        public SettingsService() 
        {
            string folder = AppDomain.CurrentDomain.BaseDirectory;
            this._filePath = Path.Combine(folder, "settings.json");

            this.Load();
        }

        public void Load()
        {
            if (!File.Exists(this._filePath))
            {
                this.AppSettings = new AppSettings();
            }

            string json = File.ReadAllText(this._filePath);

            this.AppSettings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
        }

        public void Save()
        {
            string json = JsonSerializer.Serialize(this.AppSettings);     
            File.WriteAllText(this._filePath, json);

            this.Load();
        }
    }
}
