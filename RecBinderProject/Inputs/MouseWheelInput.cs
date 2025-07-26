using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RecBinderProject.Inputs
{
    public struct MouseWheelInput
    {
        public int Delta { get; set; }
        public int TimeOffset { get; set; }

        public MouseWheelInput(int delta, int timeOffset)
        {
            this.Delta = delta;
            this.TimeOffset = timeOffset;
        }
    }
}
