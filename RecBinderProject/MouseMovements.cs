using System.Windows.Forms;
using WindowsInput;

namespace RecBinderProject
{
    public class MouseMovements
    {
        private List<MouseMove> mouseMovementsList;

        public MouseMovements()
        {
            this.mouseMovementsList = new List<MouseMove>();
        }

        public void AddMouseMovement(MouseMove mouseMove)
        {
            if (this.mouseMovementsList != null)
            {
                this.mouseMovementsList.Add(mouseMove);
            }
        }

        public void PlayMouseMovements()
        {
            InputSimulator inputSimulator = new InputSimulator();
            var startPosition = System.Windows.Forms.Cursor.Position;

            foreach (MouseMove point in this.mouseMovementsList)
            {
                int targetX = startPosition.X + (int)point.X - (int)this.mouseMovementsList[0].X;
                int targetY = startPosition.Y + (int)point.Y - (int)this.mouseMovementsList[0].Y;

                int scaledX = targetX * 65535 / Screen.PrimaryScreen.Bounds.Width;
                int scaledY = targetY * 65535 / Screen.PrimaryScreen.Bounds.Height;

                inputSimulator.Mouse.MoveMouseTo(scaledX, scaledY);
                Thread.Sleep((int)(point.DelayMs));
            }
        }
    }
}
