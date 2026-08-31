using RecBinderProject.Domain;


namespace RecBinderProject.Presentation.ViewModels.MainWindow
{
    public class MacroListItemViewModel
    {
        public Macro Macro { get; private set; }

        public string MacroName => this.Macro.Metadata.Name;
        public string MacroDescription => this.Macro.Metadata.Description;
        public string MacroDuration => this.Macro.Metadata.Duration.ToString(@"hh\:mm\:ss\:ff");
        public string MacroCreationDateTime => this.Macro.Metadata.CreationDateTime.ToString("G");

        public MacroListItemViewModel(Macro macro)
        {
            this.Macro = macro;
        }
    }
}
