using RecBinderProject.ApplicationLayer.MacroExecutionService.Engine;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Options;

using RecBinderProject.Domain;


namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Common
{
    public interface IMacroEventRecorderFactory
    {
        public MacroEventRecorder Create(MacroRecordingOptions options, Macro macro);

    }
}
