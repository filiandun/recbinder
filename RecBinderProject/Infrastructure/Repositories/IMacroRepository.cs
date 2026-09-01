using RecBinderProject.Domain;


namespace RecBinderProject.Infrastructure.Repositories
{
    public interface IMacroRepository
    {
        public event Action? MacroSaved;

        public MacroLoadResult LoadAll();

        public Macro Read(string recordName);
        public void Save(Macro recordingInput, bool allowOverwrite = false);
        public void Delete(Macro recordingInput);
    }
}
