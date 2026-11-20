using System.Windows.Interop;
using System.Runtime.InteropServices;

using RecBinderProject.Domain.MacroEvents;


namespace RecBinderProject.Infrastructure.Input
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RAWINPUTDEVICE
    {
        public ushort usUsagePage;
        public ushort usUsage;
        public uint dwFlags;
        public IntPtr hwndTarget;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RAWINPUTHEADER
    {
        public uint dwType;
        public uint dwSize;
        public IntPtr hDevice;
        public IntPtr wParam;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RAWMOUSE
    {
        public ushort usFlags;
        public uint ulButtons;
        public uint ulRawButtons;
        public int lLastX;
        public int lLastY;
        public uint ulExtraInformation;
    }


    public class RawInputListener : IInputListener
    {
        public event Action<MouseDeltaEvent>? MouseDeltaReceived;

        private const int WM_INPUT = 0x00FF;
        private const int RID_INPUT = 0x10000003;

        private const ushort HID_USAGE_PAGE_GENERIC = 0x01;
        private const ushort HID_USAGE_MOUSE = 0x02;

        private const uint RIDEV_INPUTSINK = 0x00000100; // background listen
        private const uint RIDEV_REMOVE = 0x00000001;

        private const uint RIM_TYPEMOUSE = 0;
        private const ushort MOUSE_MOVE_ABSOLUTE = 1;

        private static readonly uint RawInputHeaderSize = (uint)Marshal.SizeOf<RAWINPUTHEADER>();


        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterRawInputDevices([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] RAWINPUTDEVICE[] pRawInputDevices, uint uiNumDevices, uint cbSize);

        [DllImport("user32.dll")]
        public static extern uint GetRawInputData(IntPtr hRawInput, uint uiCommand, IntPtr pData, ref uint pcbSize, uint cbSizeHeader);


        private readonly IntPtr _buffer = Marshal.AllocHGlobal(64);
        private readonly uint _bufferCapacity = 64;

        private HwndSource? _source;
        private IntPtr _hwnd;


        public void Initialize(IntPtr hwnd)
        {
            this._hwnd = hwnd;

            this._source = HwndSource.FromHwnd(this._hwnd);
            this._source?.AddHook(HwndMessageHook);
        }

        public void Register() // TODO сделать без исключений
        {
            if (this._hwnd == IntPtr.Zero) throw new InvalidOperationException("RawInputListener is not initialized.");

            RAWINPUTDEVICE[] devices = new RAWINPUTDEVICE[1];
            devices[0].usUsagePage = HID_USAGE_PAGE_GENERIC;
            devices[0].usUsage = HID_USAGE_MOUSE;
            devices[0].dwFlags = RIDEV_INPUTSINK;
            devices[0].hwndTarget = this._hwnd;

            bool success = RegisterRawInputDevices(devices, (uint)devices.Length, (uint)Marshal.SizeOf<RAWINPUTDEVICE>());
            if (!success) throw new InvalidOperationException($"RawInputListener error start a Raw Input: {Marshal.GetLastWin32Error()}");
        }

        public void Unregister()
        {
            RAWINPUTDEVICE[] devices = new RAWINPUTDEVICE[1];
            devices[0].usUsagePage = HID_USAGE_PAGE_GENERIC;
            devices[0].usUsage = HID_USAGE_MOUSE;
            devices[0].dwFlags = RIDEV_REMOVE;
            devices[0].hwndTarget = IntPtr.Zero;

            RegisterRawInputDevices(devices, (uint)devices.Length, (uint)Marshal.SizeOf<RAWINPUTDEVICE>());
        }

        private IntPtr HwndMessageHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg != WM_INPUT)
            {
                return IntPtr.Zero;
            }

            uint dwSize = this._bufferCapacity;

            if (GetRawInputData(lParam, RID_INPUT, this._buffer, ref dwSize, RawInputHeaderSize) != uint.MaxValue)
            {
                RAWINPUTHEADER header = Marshal.PtrToStructure<RAWINPUTHEADER>(this._buffer);

                if (header.dwType == RIM_TYPEMOUSE)
                {
                    IntPtr mousePtr = IntPtr.Add(this._buffer, (int)RawInputHeaderSize);
                    RAWMOUSE mouse = Marshal.PtrToStructure<RAWMOUSE>(mousePtr);

                    bool isAbsolute = (mouse.usFlags & MOUSE_MOVE_ABSOLUTE) != 0;

                    if (!isAbsolute && (mouse.lLastX != 0 || mouse.lLastY != 0))
                    {
                        this.MouseDeltaReceived?.Invoke(new MouseDeltaEvent(mouse.lLastX, mouse.lLastY));
                    }
                }
            }

            return IntPtr.Zero;
        }


        public void Dispose()
        {
            this.Unregister();

            Marshal.FreeHGlobal(this._buffer);

            if (this._source is not null)
            {
                this._source.RemoveHook(this.HwndMessageHook);
                this._source = null;
            }
        }
    }
}
