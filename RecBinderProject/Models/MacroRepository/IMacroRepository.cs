namespace RecBinderProject.Models.MacroRepository
{
    public interface IMacroRepository
    {
        public List<Macro> LoadAll();

        public Macro Read(string recordName);
        public void Save(Macro recordingInput);

        public void Delete(Macro recordingInput);
    }
}
