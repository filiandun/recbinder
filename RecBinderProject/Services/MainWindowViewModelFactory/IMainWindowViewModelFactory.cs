using RecBinderProject.Models;

using RecBinderProject.ViewModels.AppSettingsViewModels;
using RecBinderProject.ViewModels.MacroExecutionViewModels;


namespace RecBinderProject.Services.MainWindowViewModelFactory
{
    public interface IMainWindowViewModelFactory
    {
        public MacroPlaybackViewModel CreatePlayback(Macro macro);
        public MacroRecordingViewModel CreateRecording();

        public AppSettingsViewModel GetAppSettings();
    }
}
