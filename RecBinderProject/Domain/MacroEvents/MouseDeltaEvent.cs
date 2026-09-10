
namespace RecBinderProject.Domain.MacroEvents
{
    public struct MouseDeltaEvent
    {
        public int DeltaX { get; set; }
        public int DeltaY { get; set; }

        public MouseDeltaEvent(int deltaX, int deltaY)
        {
            this.DeltaX = deltaX;
            this.DeltaY = deltaY;
        }
    }
}
