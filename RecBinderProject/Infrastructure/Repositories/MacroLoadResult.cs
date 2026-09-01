using RecBinderProject.Domain;


namespace RecBinderProject.Infrastructure.Repositories
{
    public class MacroLoadResult
    {
        public List<Macro> Macros { get; init; } = new();
        public List<string> FailedFiles { get; init; } = new();
    }
}
