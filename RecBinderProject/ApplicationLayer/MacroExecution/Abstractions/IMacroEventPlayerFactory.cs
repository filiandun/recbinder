using RecBinderProject.ApplicationLayer.MacroExecution.Playback;

using RecBinderProject.Domain;


namespace RecBinderProject.ApplicationLayer.MacroExecution.Abstractions
{
    public interface IMacroEventPlayerFactory
    {
        public IMacroEventPlayer Create(MacroPlaybackOptions options, Macro macro);
    }
}
