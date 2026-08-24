using RecBinderProject.Presentation.ViewModels.AppSettings;
using RecBinderProject.Presentation.ViewModels.MacroExecution;


namespace RecBinderProject.Presentation.Factories
{
    public interface IMainWindowViewModelFactory
    {
        public MacroLibraryViewModel CreateLibrary();

        public MacroRecordingViewModel CreateRecording();

        //public AppHelpViewModel CreateHelp();

        public AppSettingsViewModel CreateAppSettings();
    }
}
