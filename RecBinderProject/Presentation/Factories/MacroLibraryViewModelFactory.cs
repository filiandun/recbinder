using Microsoft.Extensions.DependencyInjection;

using RecBinderProject.Domain;

using RecBinderProject.Presentation.ViewModels.MacroExecution;
using RecBinderProject.Presentation.ViewModels.MacroMetadataExecution;


namespace RecBinderProject.Presentation.Factories
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
            MacroMetadataViewerViewModel metadataViewerViewModel = ActivatorUtilities.CreateInstance<MacroMetadataViewerViewModel>(this._serviceProvider, macro.Metadata);

            MacroPlaybackViewModel playbackViewModel = ActivatorUtilities.CreateInstance<MacroPlaybackViewModel>(this._serviceProvider, metadataViewerViewModel, macro);

            return playbackViewModel;
        }
    }
}
