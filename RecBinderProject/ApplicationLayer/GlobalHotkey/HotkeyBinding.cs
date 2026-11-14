using System.Windows.Input;


namespace RecBinderProject.ApplicationLayer.GlobalHotkey
{
    public sealed class HotkeyBinding
    {
        public HotkeyAction Action { get; }
        public ModifierKeys Modifiers { get; }
        public Key Key { get; }


        public HotkeyBinding(HotkeyAction action, ModifierKeys modifiers, Key key)
        {
            this.Action = action;
            this.Modifiers = modifiers;
            this.Key = key;
        }
    }
}
