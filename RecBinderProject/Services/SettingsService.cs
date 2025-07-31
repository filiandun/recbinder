using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RecBinderProject.Services
{
    public class SettingsService
    {
        private string _filePath;
        public AppSettings AppSettings { set; get; }

        public SettingsService() 
        {
            string folder = AppDomain.CurrentDomain.BaseDirectory;
            this._filePath = Path.Combine(folder, "settings.json");

            this.AppSettings = this.Load();
        }

        private AppSettings Load()
        {
            if (!File.Exists(this._filePath))
            {
                return new AppSettings();
            }

            string json = File.ReadAllText(this._filePath);

            return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
        }

        public void Save()
        {
            string json = JsonSerializer.Serialize(this.AppSettings);     
            File.WriteAllText(this._filePath, json);

            this.AppSettings = this.Load();
        }
    }
}
