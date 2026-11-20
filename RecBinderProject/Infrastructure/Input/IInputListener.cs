using RecBinderProject.Domain.MacroEvents;


namespace RecBinderProject.Infrastructure.Input
{
    public interface IInputListener : IDisposable
    {
        event Action<MouseDeltaEvent>? MouseDeltaReceived;

        void Initialize(IntPtr hwnd);

        void Register();
        void Unregister();
    }
}
