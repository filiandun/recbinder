using Microsoft.Extensions.DependencyInjection;

using RecBinderProject.ApplicationLayer.MacroExecutionService.Common;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Engine;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Options;

using RecBinderProject.Domain;


namespace RecBinderProject.Infrastructure.Factories
{
    public class MacroEventPlayerFactory : IMacroEventPlayerFactory
    {
        private readonly IServiceProvider _serviceProvider;


        public MacroEventPlayerFactory(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }


        public MacroEventPlayer Create(MacroPlaybackOptions options, Macro macro)
        {
            return ActivatorUtilities.CreateInstance<MacroEventPlayer>(this._serviceProvider, options, macro);
        }
    }
}
