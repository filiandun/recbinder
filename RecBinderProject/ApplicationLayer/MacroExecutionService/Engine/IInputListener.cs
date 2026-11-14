using RecBinderProject.Domain.MacroEvents;


namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Engine
{
    public interface IInputListener : IDisposable
    {
        event Action<MouseDeltaEvent>? MouseDeltaReceived;

        void Initialize(IntPtr hwnd);

        void Register();
        void Unregister();
    }
}
