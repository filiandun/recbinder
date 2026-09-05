
namespace RecBinderProject.Domain.MacroEvents
{
    public struct MouseWheelEvent
    {
        public int Delta { get; set; }

        public MouseWheelEvent(int delta)
        {
            this.Delta = delta;
        }
    }
}
