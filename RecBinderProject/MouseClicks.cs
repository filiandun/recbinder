using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;

namespace RecBinderProject
{
    public class MouseClicks
    {
        private List<MouseClick> mouseClicks;

        public MouseClicks()
        {
            this.mouseClicks = new List<MouseClick>();
        }

        public void AddMouseClick(MouseClick mouseClick)
        {
            if (this.mouseClicks != null)
            {
                this.mouseClicks.Add(mouseClick);
            }
        }

        public void PlayMouseClicks()
        {
            InputSimulator inputSimulator = new InputSimulator();

            foreach (MouseClick mouseClick in this.mouseClicks)
            {
                if (mouseClick.KeyState == KeyState.Down)
                {
                    //inputSimulator.Mouse.XButtonDown(((int)mouseClick.MouseButton));
                    inputSimulator.Mouse.LeftButtonDown();
                }
                else if (mouseClick.KeyState == KeyState.Up)
                {
                    //inputSimulator.Mouse.XButtonUp(((int)mouseClick.MouseButton));
                    inputSimulator.Mouse.LeftButtonUp();

                }

                Thread.Sleep((int)(mouseClick.DelayMs));
            }
        }
    }
}
