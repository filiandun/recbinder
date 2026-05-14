using RecBinderProject.Domain;


namespace RecBinderProject.Presentation.ViewModels.MainWindow
{
    public class MacroListItemViewModel
    {
        public Macro Macro { get; init; }

        public string MacroName => this.Macro.Metadata.Name;
        public string MacroDescription => this.Macro.Metadata.Description;
        public string MacroDuration => this.Macro.Metadata.Duration.ToString();

        public MacroListItemViewModel(Macro macro)
        {
            this.Macro = macro;
        }
    }
}
