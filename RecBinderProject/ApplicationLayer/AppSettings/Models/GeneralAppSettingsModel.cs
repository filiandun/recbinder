using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace RecBinderProject.ApplicationLayer.AppSettings.Models
{
    public class GeneralAppSettingsModel : IEquatable<GeneralAppSettingsModel>, ICloneable
    {
        public AppLanguage AppLanguage { get; set; }
        public AppTheme AppTheme { get; set; }
        public string SaveDirectory { get; set; }

        public Size MonitorSize { get; set; }


        public GeneralAppSettingsModel()
        {
            this.AppLanguage = AppLanguage.System;
            this.AppTheme = AppTheme.System;
            this.SaveDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Macros");

            this.MonitorSize = SystemInformation.PrimaryMonitorSize;
        }

        public GeneralAppSettingsModel(AppLanguage language, AppTheme theme, string saveDirectory, Size monitorSize)
        {
            this.AppLanguage = language;
            this.AppTheme = theme;
            this.SaveDirectory = saveDirectory;

            this.MonitorSize = monitorSize;
        }


        public GeneralAppSettingsModel(AppLanguage language, AppTheme theme, string saveDirectory, int monitorWidth, int monitorHeight)
        {
            this.AppLanguage = language;
            this.AppTheme = theme;
            this.SaveDirectory = saveDirectory;

            this.MonitorSize = new Size(monitorWidth, monitorHeight);
        }


        public static GeneralAppSettingsModel Default()
        {
            return new GeneralAppSettingsModel()
            {
                AppLanguage = AppLanguage.System,
                AppTheme = AppTheme.System,
                SaveDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Macros"),
                MonitorSize = SystemInformation.PrimaryMonitorSize
            };
        }

        public bool Equals(GeneralAppSettingsModel? other) // TODO может и не надо, просто сделать модель record, а не class
        {
            if (other is null) return false;

            return this.AppLanguage == other.AppLanguage &&
                   this.AppTheme == other.AppTheme &&
                   this.SaveDirectory == other.SaveDirectory &&
                   this.MonitorSize == other.MonitorSize;
        }

        // TODO может и не надо, просто сделать модель record, а не class
        public object Clone() => new GeneralAppSettingsModel(this.AppLanguage, this.AppTheme, this.SaveDirectory, this.MonitorSize);
    }
}
