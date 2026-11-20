using Microsoft.Extensions.DependencyInjection;

using RecBinderProject.ApplicationLayer.MacroExecution.Abstractions;
using RecBinderProject.ApplicationLayer.MacroExecution.Playback;

using RecBinderProject.Domain;

using RecBinderProject.Infrastructure.EventExecution;


namespace RecBinderProject.Infrastructure.Factories
{
    public class MacroEventPlayerFactory : IMacroEventPlayerFactory
    {
        private readonly IServiceProvider _serviceProvider;


        public MacroEventPlayerFactory(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }


        public IMacroEventPlayer Create(MacroPlaybackOptions options, Macro macro)
        {
            return ActivatorUtilities.CreateInstance<MacroEventPlayer>(this._serviceProvider, options, macro);
        }
    }
}
