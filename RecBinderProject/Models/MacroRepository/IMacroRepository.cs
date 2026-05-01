namespace RecBinderProject.Models.MacroRepository
{
    public interface IMacroRepository
    {
        public MacroLoadResult LoadAll();

        public Macro Read(string recordName);
        public void Save(Macro recordingInput, bool allowOverwrite = false);

        public void Delete(Macro recordingInput);
    }
}
