using Microsoft.Extensions.DependencyInjection;

using RecBinderProject.Domain;
using RecBinderProject.Presentation.Factories;
using RecBinderProject.Presentation.ViewModels.MacroExecution;
using RecBinderProject.Presentation.ViewModels.MacroMetadataExecution;


namespace RecBinderProject.Infrastructure.Factories
{
    public class MacroLibraryViewModelFactory : IMacroLibraryViewModelFactory
    {
        private readonly IServiceProvider _serviceProvider;


        public MacroLibraryViewModelFactory(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }


        public MacroPlaybackViewModel CreatePlayback(Macro macro)
        {
            MacroPlaybackViewModel playbackViewModel = ActivatorUtilities.CreateInstance<MacroPlaybackViewModel>(this._serviceProvider, macro);

            return playbackViewModel;
        }
    }
}
