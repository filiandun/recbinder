namespace RecBinderProject.ApplicationLayer.MacroExecution.Abstractions
{
    public interface IMacroEventRecorder : IDisposable
    {
        public TimeSpan GetElapsedTime { get; }

        public void Start();
        public void Resume();
        public void Pause();
        public void Stop();
    }
}
