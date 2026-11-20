using RecBinderProject.ApplicationLayer.MacroExecution.Recording;

using RecBinderProject.Domain;


namespace RecBinderProject.ApplicationLayer.MacroExecution.Abstractions
{
    public interface IMacroEventRecorderFactory
    {
        public IMacroEventRecorder Create(MacroRecordingOptions options, Macro macro);

    }
}
