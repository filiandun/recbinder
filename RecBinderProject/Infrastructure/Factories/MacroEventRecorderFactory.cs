using Microsoft.Extensions.DependencyInjection;

using RecBinderProject.ApplicationLayer.MacroExecution.Abstractions;
using RecBinderProject.ApplicationLayer.MacroExecution.Recording;

using RecBinderProject.Domain;

using RecBinderProject.Infrastructure.EventExecution;


namespace RecBinderProject.Infrastructure.Factories
{
    public class MacroEventRecorderFactory : IMacroEventRecorderFactory
    {
        private readonly IServiceProvider _serviceProvider;


        public MacroEventRecorderFactory(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }


        public IMacroEventRecorder Create(MacroRecordingOptions options, Macro macro)
        {
            return ActivatorUtilities.CreateInstance<MacroEventRecorder>(this._serviceProvider, options, macro);
        }
    }
}
