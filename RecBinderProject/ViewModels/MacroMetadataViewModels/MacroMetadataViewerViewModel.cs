using RecBinderProject.Models;


namespace RecBinderProject.ViewModels.MacroMetadataViewModels
{
    public class MacroMetadataViewerViewModel
    {
        public string MacroName { get; init; }
        public string MacroDescription { get; init; }

        public string MacroDuration { get; init; }
        public string MacroCreationDateTime { get; init; }


        public MacroMetadataViewerViewModel(MacroMetadata metadata)
        {
            this.MacroName = metadata.Name;
            this.MacroDescription = metadata.Description;
            this.MacroDuration = metadata.Duration.ToString(@"hh\:mm\:ss\:ff"); ;
            this.MacroCreationDateTime = metadata.CreationDateTime.ToString("G");
        }
    }
}
