using RecBinderProject.Domain.MacroEvents;


namespace RecBinderProject.Infrastructure.Services.Input
{
    public interface IInputListener : IDisposable
    {
        event Action<MouseDeltaEvent>? MouseDeltaReceived;

        void Initialize(IntPtr hwnd);

        void Register();
        void Unregister();
    }
}
