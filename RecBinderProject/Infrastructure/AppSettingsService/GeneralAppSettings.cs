using System.IO;
using System.Drawing;
using System.Windows.Forms;


namespace RecBinderProject.Infrastructure.AppSettingsService
{
    public class GeneralAppSettings : IEquatable<GeneralAppSettings>, ICloneable
    {
        public string SaveDirectory { get; set; }
        public AppTheme AppTheme { get; set; }

        public Size MonitorSize { get; set; }

        public GeneralAppSettings()
        {
            this.SaveDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Macros");
            this.AppTheme = AppTheme.System;
            this.MonitorSize = SystemInformation.PrimaryMonitorSize;
        }

        public GeneralAppSettings(string saveDirectory, AppTheme theme, Size monitorSize)
        {
            this.SaveDirectory = saveDirectory;
            this.AppTheme = theme;
            this.MonitorSize = monitorSize;
        }

        public GeneralAppSettings(string saveDirectory, AppTheme theme, int monitorWidth, int monitorHeight)
        {
            this.SaveDirectory = saveDirectory;
            this.AppTheme = theme;
            this.MonitorSize = new Size(monitorWidth, monitorHeight);
        }

        public static GeneralAppSettings Default()
        {
            return new GeneralAppSettings()
            {
                SaveDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Macros"),
                AppTheme = AppTheme.System,
                MonitorSize = SystemInformation.PrimaryMonitorSize
            };
        }

        public bool Equals(GeneralAppSettings? other) // TODO может и не надо, просто сделать модель record, а не class
        {
            if (other is null) return false;

            return this.SaveDirectory == other.SaveDirectory &&
                   this.AppTheme == other.AppTheme &&
                   this.MonitorSize == other.MonitorSize;
        }

        // TODO может и не надо, просто сделать модель record, а не class
        public object Clone() => new GeneralAppSettings(this.SaveDirectory, this.AppTheme, this.MonitorSize);
    }
}
