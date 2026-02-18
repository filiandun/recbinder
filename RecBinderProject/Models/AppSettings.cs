using ModernWpf;
using System.Drawing;
using System.Windows.Forms;


namespace RecBinderProject.Models
{
    public class AppSettings : IEquatable<AppSettings>, ICloneable
    {
        public string SaveDirectory { get; set; }
        public ApplicationTheme ApplicationTheme { get; set; } // TO DO применение темы не совсем работает, так как тема подтягивается сначала с файла настроек,
                                            // а потом, когда уже main window запускается, то оно подтягивается как в системе из-за UI библиотеки
        public Size MonitorSize { get; set; }

        public AppSettings() 
        {
            this.SaveDirectory = "C:\\Repositories\\RecBinder\\RecBinderProject\\Recordings\\" /*AppDomain.CurrentDomain.BaseDirectory*/;
            this.ApplicationTheme = ApplicationTheme.Light;
            this.MonitorSize = SystemInformation.PrimaryMonitorSize;
        }

        public AppSettings(string saveDirectory, ApplicationTheme theme, Size monitorSize)
        {
            this.SaveDirectory = saveDirectory;
            this.ApplicationTheme = theme;
            this.MonitorSize = monitorSize;
        }

        public AppSettings(string saveDirectory, ApplicationTheme theme, int monitorWidth, int monitorHeight)
        {
            this.SaveDirectory = saveDirectory;
            this.ApplicationTheme = theme;
            this.MonitorSize = new Size(monitorWidth, monitorHeight);
        }

        public bool Equals(AppSettings? other) // TODO может и не надо, просто сделать модель record, а не class
        {
            if (other is null) return false;

            return this.SaveDirectory == other.SaveDirectory &&
                   this.ApplicationTheme == other.ApplicationTheme &&
                   this.MonitorSize == other.MonitorSize;
        }

        // TODO может и не надо, просто сделать модель record, а не class
        public object Clone() => new AppSettings(this.SaveDirectory, this.ApplicationTheme, this.MonitorSize);
    }
}
