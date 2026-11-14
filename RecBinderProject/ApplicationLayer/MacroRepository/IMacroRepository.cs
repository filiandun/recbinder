using RecBinderProject.Domain;


namespace RecBinderProject.ApplicationLayer.MacroRepository
{
    public interface IMacroRepository
    {
        public event Action<Macro>? MacroSaved;

        public MacroLoadResult LoadAll();

        public Macro Read(string recordName);
        public Task SaveAsync(Macro recordingInput);
        public void Delete(Macro recordingInput);
    }
}
