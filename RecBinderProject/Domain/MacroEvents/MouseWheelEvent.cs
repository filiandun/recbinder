
namespace RecBinderProject.Domain.MacroEvents
{
    public struct MouseWheelEvent
    {
        public int WheelClicks { get; set; }

        public MouseWheelEvent(int wheelClicks)
        {
            this.WheelClicks = wheelClicks;
        }
    }
}
