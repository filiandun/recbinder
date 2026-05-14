using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Runtime.InteropServices;


namespace RecBinderProject.Infrastructure.GlobalHotkeyService
{
    public class GlobalHotkeyService
    {
        public event Action<HotkeyAction>? HotkeyPressed;

        private const int WM_HOTKEY = 0x0312;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private readonly Dictionary<HotkeyAction, int> _actionToId;
        private readonly Dictionary<int, HotkeyAction> _idToAction;
        private readonly Dictionary<HotkeyAction, HotkeyBinding> _bindings;

        private HwndSource? _source;
        private IntPtr _hwnd;
        private int _nextId;


        public GlobalHotkeyService()
        {
            this._actionToId = new();
            this._idToAction = new();
            this._bindings = new();

            this._hwnd = IntPtr.Zero;
            this._nextId = 1;
        }


        public void Initialize(Window window)
        {
            this._hwnd = new WindowInteropHelper(window).Handle;
            this._source = HwndSource.FromHwnd(_hwnd);
            this._source?.AddHook(WndProc);
        }

        public bool Register(HotkeyBinding binding)
        {
            if (this._hwnd == IntPtr.Zero) throw new InvalidOperationException("HotkeyService is not initialized.");

            Unregister(binding.Action);

            int id = this._nextId++;
            uint modifiers = (uint)binding.Modifiers;
            uint vk = (uint)KeyInterop.VirtualKeyFromKey(binding.Key);

            bool ok = RegisterHotKey(this._hwnd, id, modifiers, vk);
            if (!ok) return false;

            this._actionToId[binding.Action] = id;
            this._idToAction[id] = binding.Action;
            this._bindings[binding.Action] = binding;

            return true;
        }

        public void UpdateBindings(IEnumerable<HotkeyBinding> bindings)
        {
            this.UnregisterAll();

            foreach (var binding in bindings)
            {
                this.Register(binding);
            }
        }

        public void Unregister(HotkeyAction action)
        {
            if (this._actionToId.TryGetValue(action, out int id))
            {
                UnregisterHotKey(this._hwnd, id);
                this._actionToId.Remove(action);
                this._idToAction.Remove(id);
                this._bindings.Remove(action);
            }
        }

        public void UnregisterAll()
        {
            foreach (var id in this._actionToId.Values)
            {
                UnregisterHotKey(_hwnd, id);
            }

            this._actionToId.Clear();
            this._idToAction.Clear();
            this._bindings.Clear();
        }

        public void Dispose()
        {
            UnregisterAll();

            if (this._source is not null)
            {
                this._source.RemoveHook(WndProc);
                this._source = null;
            }

            this._hwnd = IntPtr.Zero;
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY)
            {
                int id = wParam.ToInt32();

                if (this._idToAction.TryGetValue(id, out var action))
                {
                    this.HotkeyPressed?.Invoke(action);
                    handled = true;
                }
            }
            return IntPtr.Zero;
        }
    }
}
