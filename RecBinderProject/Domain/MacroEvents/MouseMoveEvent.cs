namespace RecBinderProject.Domain.MacroEvents
{
    public struct MouseMoveEvent
    {
        public double X { get; set; }
        public double Y { get; set; }
        public int TimeOffset { get; set; }

        public MouseMoveEvent(double X, double Y, int timeOffset)
        {
            this.X = X;
            this.Y = Y;
            this.TimeOffset = timeOffset;
        }
    }
}
