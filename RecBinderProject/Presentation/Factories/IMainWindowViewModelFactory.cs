using RecBinderProject.Domain;

using RecBinderProject.Presentation.ViewModels.AppSettings;
using RecBinderProject.Presentation.ViewModels.MacroExecution;


namespace RecBinderProject.Presentation.Factories
{
    public interface IMainWindowViewModelFactory
    {
        public MacroLibraryViewModel CreateLibrary();

        public MacroPlaybackViewModel CreatePlayback(Macro macro);

        public MacroRecordingViewModel CreateRecording();

        public AppSettingsViewModel GetAppSettings();
    }
}
