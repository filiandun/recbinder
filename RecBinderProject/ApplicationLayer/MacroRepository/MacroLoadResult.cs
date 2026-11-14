using RecBinderProject.Domain;


namespace RecBinderProject.ApplicationLayer.MacroRepository
{
    public class MacroLoadResult
    {
        public List<Macro> Macros { get; init; } = new();
        public List<string> FailedFiles { get; init; } = new();
    }
}
