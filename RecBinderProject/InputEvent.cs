using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecBinderProject
{
    public enum KeyState
    {
        Up,
        Down
    }

    public interface IInputEvent
    {
        public int TimeOffset { get; set; }
    }
}
