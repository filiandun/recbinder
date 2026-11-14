namespace RecBinderProject.ApplicationLayer.GlobalHotkey
{
    public interface IGlobalHotkeyService : IDisposable
    {
        public event Action<HotkeyAction>? HotkeyPressed;

        public void Initialize(IntPtr hwnd);

        public void UpdateBindings(IEnumerable<HotkeyBinding> bindings);
    }
}