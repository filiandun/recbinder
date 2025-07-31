
namespace RecBinderProject
{
    public enum AppTheme
    {
        Dark,
        Light
    }

    public class AppSettings
    {
        public string SaveDirectory { get; set; }
        public AppTheme Theme { get; set; } // TO DO применение темы не совсем работает, так как тема подтягивается сначала с файла настроек,
                                            // а потом, когда уже main window запускается, то оно подтягивается как в системе

        public AppSettings() 
        {
            this.SaveDirectory = "C:\\Repositories\\RecBinder\\RecBinderProject\\Recordings\\" /*AppDomain.CurrentDomain.BaseDirectory*/;
            this.Theme = AppTheme.Dark;
        }
    }
}
