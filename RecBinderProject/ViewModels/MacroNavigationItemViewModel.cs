using RecBinderProject.Models;


namespace RecBinderProject.ViewModels
{
    public class MacroNavigationItemViewModel
    {
        public Macro Macro { get; init; }

        public string MacroName => this.Macro.Metadata.Name;
        public string MacroDescription => this.Macro.Metadata.Description;
        public string MacroDuration => this.Macro.Metadata.Duration.ToString();

        public MacroNavigationItemViewModel(Macro macro)
        {
            this.Macro = macro;
        }
    }
}
