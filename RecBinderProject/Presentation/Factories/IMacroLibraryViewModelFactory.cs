using RecBinderProject.Domain;
using RecBinderProject.Presentation.ViewModels.MacroExecution;


namespace RecBinderProject.Presentation.Factories
{
    public interface IMacroLibraryViewModelFactory
    {
        public MacroPlaybackViewModel CreatePlayback(Macro macro);
    }
}
