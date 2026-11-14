
namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Engine
{
    public interface IInputSimulator
    {
        public void KeyboardDown(int keyCode);
        public void KeyboardUp(int keyCode);

        public void MouseDown(int button);
        public void MouseUp(int button);


        public void MouseMoveTo(double x, double y);
        public void MouseMoveBy(int deltaX, int deltaY);

        public void MouseVerticalScroll(int delta);

        public void ReleaseAll();
    }
}
