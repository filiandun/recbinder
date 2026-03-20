using System.IO;
using System.Drawing;
using System.Windows.Forms;

using ModernWpf;


namespace RecBinderProject.Models.AppSettings
{
    public class GeneralAppSettings : IEquatable<GeneralAppSettings>, ICloneable
    {
        public string SaveDirectory { get; set; }
        public ApplicationTheme ApplicationTheme { get; set; } // TO DO применение темы не совсем работает, так как тема подтягивается сначала с файла настроек,
                                                               // а потом, когда уже main window запускается, то оно подтягивается как в системе из-за UI библиотеки
        public Size MonitorSize { get; set; }

        public GeneralAppSettings()
        {
            this.SaveDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Macros");
            this.ApplicationTheme = ApplicationTheme.Light;
            this.MonitorSize = SystemInformation.PrimaryMonitorSize;
        }

        public GeneralAppSettings(string saveDirectory, ApplicationTheme theme, Size monitorSize)
        {
            this.SaveDirectory = saveDirectory;
            this.ApplicationTheme = theme;
            this.MonitorSize = monitorSize;
        }

        public GeneralAppSettings(string saveDirectory, ApplicationTheme theme, int monitorWidth, int monitorHeight)
        {
            this.SaveDirectory = saveDirectory;
            this.ApplicationTheme = theme;
            this.MonitorSize = new Size(monitorWidth, monitorHeight);
        }

        public static GeneralAppSettings Default()
        {
            return new GeneralAppSettings()
            {
                SaveDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Macros"),
                ApplicationTheme = ApplicationTheme.Light,
                MonitorSize = SystemInformation.PrimaryMonitorSize
            };
        }

        public bool Equals(GeneralAppSettings? other) // TODO может и не надо, просто сделать модель record, а не class
        {
            if (other is null) return false;

            return this.SaveDirectory == other.SaveDirectory &&
                   this.ApplicationTheme == other.ApplicationTheme &&
                   this.MonitorSize == other.MonitorSize;
        }

        // TODO может и не надо, просто сделать модель record, а не class
        public object Clone() => new GeneralAppSettings(this.SaveDirectory, this.ApplicationTheme, this.MonitorSize);
    }
}
