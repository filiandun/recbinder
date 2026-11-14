using RecBinderProject.ApplicationLayer.MacroExecutionService.Engine;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Options;

using RecBinderProject.Domain;


namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Common
{
    public interface IMacroEventPlayerFactory
    {
        public MacroEventPlayer Create(MacroPlaybackOptions options, Macro macro);
    }
}
