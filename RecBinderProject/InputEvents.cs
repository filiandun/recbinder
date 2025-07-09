using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;

namespace RecBinderProject
{
    public class InputEvents : ICloneable
    {
        private LinkedList<IInputEvent> inputEventsList;

        public string Name { get; private set; }
        public double Duration { get; private set; }
        public DateTime dateTime { get; private set; }
        public string Description { get; private set; }


        public InputEvents()
        {
            this.inputEventsList = new LinkedList<IInputEvent>();
        }

        private InputEvents(LinkedList<IInputEvent> recordedEvents)
        {
            this.inputEventsList = recordedEvents;
        }

        public void AddEvents(IInputEvent userEvent)
        {
            this.inputEventsList.AddLast(userEvent);
        }

        public void ClearEvents()
        {
            this.inputEventsList.Clear();
        }

        public object Clone()
        {
            LinkedList<IInputEvent> recordedEventsCopy = new LinkedList<IInputEvent>(this.inputEventsList);
            return new InputEvents(recordedEventsCopy);
        }

        public async Task PlayEventsAsync()
        {
            InputSimulator inputSimulator = new InputSimulator();
            Stopwatch stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < this.inputEventsList.Count; i++)
            {
                IInputEvent recordedEvent = this.inputEventsList.ElementAt(i);

                double targetTime = recordedEvent.TimeOffset;
                double elapsed = stopwatch.ElapsedMilliseconds;
                double delay = targetTime - elapsed;

                if (delay > 0)
                {
                    await Task.Delay((int)delay);
                }

                if (recordedEvent is MouseMoveEvent mouseMove)
                {
                    var startPosition = System.Windows.Forms.Cursor.Position;

                    int targetX = /*startPosition.X +*/ (int)mouseMove.X;
                    int targetY = /*startPosition.Y +*/ (int)mouseMove.Y;

                    int scaledX = targetX * 65535 / Screen.PrimaryScreen.Bounds.Width;
                    int scaledY = targetY * 65535 / Screen.PrimaryScreen.Bounds.Height;

                    inputSimulator.Mouse.MoveMouseTo(scaledX, scaledY);
                }
                else if (recordedEvent is MousePressEvent mousePressEvent) // TO DO расскидать по методам, а то слишком много условий
                {
                    if (mousePressEvent.KeyState == KeyState.Down)
                    {
                        switch (mousePressEvent.MouseButton)
                        {
                            case MouseButtons.Left: inputSimulator.Mouse.LeftButtonDown(); break;
                            case MouseButtons.Right: inputSimulator.Mouse.RightButtonDown(); break;

                            case MouseButtons.XButton1: inputSimulator.Mouse.XButtonDown(1); break;
                            case MouseButtons.XButton2: inputSimulator.Mouse.XButtonDown(2); break;
                        }
                    }
                    else if (mousePressEvent.KeyState == KeyState.Up)
                    {
                        switch (mousePressEvent.MouseButton)
                        {
                            case MouseButtons.Left: inputSimulator.Mouse.LeftButtonUp(); break;
                            case MouseButtons.Right: inputSimulator.Mouse.RightButtonUp(); break;

                            case MouseButtons.XButton1: inputSimulator.Mouse.XButtonUp(1); break;
                            case MouseButtons.XButton2: inputSimulator.Mouse.XButtonUp(2); break;
                        }
                    }
                }
                else if (recordedEvent is KeyboardPressEvent keyboardPressEvent)
                {
                    if (keyboardPressEvent.KeyState == KeyState.Down)
                    {
                        inputSimulator.Keyboard.KeyDown((WindowsInput.Native.VirtualKeyCode) keyboardPressEvent.KeyCode);
                    }
                    else if (keyboardPressEvent.KeyState == KeyState.Up)
                    {
                        inputSimulator.Keyboard.KeyUp((WindowsInput.Native.VirtualKeyCode)keyboardPressEvent.KeyCode);
                    }
                }
                else
                {
                    MessageBox.Show("Error. Event undefind");
                }
            }
        }
    }
}
