using RecBinderProject.Domain;


namespace RecBinderProject.Presentation.ViewModels.MacroLibrary
{
    public class MacroLibraryListItemViewModel
    {
        public Macro Macro { get; private set; }

        public string MacroName => this.Macro.Metadata.Name;
        public string MacroDescription => this.Macro.Metadata.Description;
        public string MacroDuration => this.Macro.Metadata.Duration.ToString(@"hh\:mm\:ss\:ff");
        public string MacroCreationDateTime => this.Macro.Metadata.CreationDateTime.ToString("G");

        public MacroLibraryListItemViewModel(Macro macro)
        {
            this.Macro = macro;
        }
    }
}
