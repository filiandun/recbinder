using Microsoft.Extensions.DependencyInjection;

using RecBinderProject.ApplicationLayer.MacroExecutionService.Common;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Engine;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Options;

using RecBinderProject.Domain;


namespace RecBinderProject.Infrastructure.Factories
{
    public class MacroEventRecorderFactory : IMacroEventRecorderFactory
    {
        private readonly IServiceProvider _serviceProvider;


        public MacroEventRecorderFactory(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }


        public MacroEventRecorder Create(MacroRecordingOptions options, Macro macro)
        {
            return ActivatorUtilities.CreateInstance<MacroEventRecorder>(this._serviceProvider, options, macro);
        }
    }
}
