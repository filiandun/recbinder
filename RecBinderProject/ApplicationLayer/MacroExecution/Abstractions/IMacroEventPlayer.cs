namespace RecBinderProject.ApplicationLayer.MacroExecution.Abstractions
{
    public interface IMacroEventPlayer
    {
        public TimeSpan EffectiveDuration { get; }
        public TimeSpan ElapsedTime { get; }

        public Task Start(CancellationToken cancellationToken);

        public void Resume();
        public void Pause();
    }
}
