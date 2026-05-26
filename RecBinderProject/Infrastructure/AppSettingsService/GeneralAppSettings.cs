using System.IO;
using System.Drawing;
using System.Windows.Forms;


namespace RecBinderProject.Infrastructure.AppSettingsService
{
    public class GeneralAppSettings : IEquatable<GeneralAppSettings>, ICloneable
    {
        public AppLanguage AppLanguage { get; set; }
        public AppTheme AppTheme { get; set; }
        public string SaveDirectory { get; set; }

        public Size MonitorSize { get; set; }


        public GeneralAppSettings()
        {
            this.AppLanguage = AppLanguage.System;
            this.AppTheme = AppTheme.System;
            this.SaveDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Macros");

            this.MonitorSize = SystemInformation.PrimaryMonitorSize;
        }

        public GeneralAppSettings(AppLanguage language, AppTheme theme, string saveDirectory, Size monitorSize)
        {
            this.AppLanguage = language;
            this.AppTheme = theme;
            this.SaveDirectory = saveDirectory;

            this.MonitorSize = monitorSize;
        }


        public GeneralAppSettings(AppLanguage language, AppTheme theme, string saveDirectory, int monitorWidth, int monitorHeight)
        {
            this.AppLanguage = language;
            this.AppTheme = theme;
            this.SaveDirectory = saveDirectory;

            this.MonitorSize = new Size(monitorWidth, monitorHeight);
        }


        public static GeneralAppSettings Default()
        {
            return new GeneralAppSettings()
            {
                AppLanguage = AppLanguage.System,
                AppTheme = AppTheme.System,
                SaveDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Macros"),
                MonitorSize = SystemInformation.PrimaryMonitorSize
            };
        }

        public bool Equals(GeneralAppSettings? other) // TODO может и не надо, просто сделать модель record, а не class
        {
            if (other is null) return false;

            return this.AppLanguage == other.AppLanguage &&
                   this.AppTheme == other.AppTheme &&
                   this.SaveDirectory == other.SaveDirectory &&
                   this.MonitorSize == other.MonitorSize;
        }

        // TODO может и не надо, просто сделать модель record, а не class
        public object Clone() => new GeneralAppSettings(this.AppLanguage, this.AppTheme, this.SaveDirectory, this.MonitorSize);
    }
}
