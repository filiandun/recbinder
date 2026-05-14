namespace RecBinderProject.Domain.MacroEvents
{
    public struct MouseWheelEvent
    {
        public int Delta { get; set; }
        public int TimeOffset { get; set; }

        public MouseWheelEvent(int delta, int timeOffset)
        {
            this.Delta = delta;
            this.TimeOffset = timeOffset;
        }
    }
}
