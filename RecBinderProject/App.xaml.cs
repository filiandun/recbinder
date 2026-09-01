using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using RecBinderProject.Infrastructure.Extenstions;


namespace RecBinderProject
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; } = null!;


        protected override void OnStartup(StartupEventArgs e)
        {
            ServiceCollection services = new();

            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Debug);
            });

            services.AddApplicationComponents(); // add components
                    
            this.ServiceProvider = services.BuildServiceProvider();

            this.ServiceProvider.InitApplicationState(); // init state  

            var mainWindow = ActivatorUtilities.CreateInstance<MainWindow>(this.ServiceProvider);
            mainWindow.Show();

            base.OnStartup(e);
        }


        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("error: " + e.Exception.Message);
            e.Handled = true;
        }
    }
}
