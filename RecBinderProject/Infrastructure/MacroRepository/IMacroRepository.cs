using RecBinderProject.Domain;


namespace RecBinderProject.Infrastructure.MacroRepository
{
    public interface IMacroRepository
    {
        public MacroLoadResult LoadAll();

        public Macro Read(string recordName);
        public void Save(Macro recordingInput, bool allowOverwrite = false);

        public void Delete(Macro recordingInput);
    }
}
