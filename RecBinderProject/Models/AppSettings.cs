using ModernWpf;
using System.Drawing;
using System.Windows.Forms;

namespace RecBinderProject.Models
{
    public class AppSettings
    {
        public string SaveDirectory { get; set; }
        public ApplicationTheme Theme { get; set; } // TO DO применение темы не совсем работает, так как тема подтягивается сначала с файла настроек,
                                            // а потом, когда уже main window запускается, то оно подтягивается как в системе
        public Size MonitorSize { get; set; }

        public AppSettings() 
        {
            this.SaveDirectory = "C:\\Repositories\\RecBinder\\RecBinderProject\\Recordings\\" /*AppDomain.CurrentDomain.BaseDirectory*/;
            this.Theme = ApplicationTheme.Light;
            this.MonitorSize = SystemInformation.PrimaryMonitorSize;
        }

        public AppSettings(string saveDirectory, ApplicationTheme theme, Size monitorSize)
        {
            this.SaveDirectory = saveDirectory;
            this.Theme = theme;
            this.MonitorSize = monitorSize;
        }

        public AppSettings(string saveDirectory, ApplicationTheme theme, int monitorWidth, int monitorHeight)
        {
            this.SaveDirectory = saveDirectory;
            this.Theme = theme;
            this.MonitorSize = new Size(monitorWidth, monitorHeight);
        }
    }
}
