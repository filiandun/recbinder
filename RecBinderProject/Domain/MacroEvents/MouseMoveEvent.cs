namespace RecBinderProject.Domain.MacroEvents
{
    public struct MouseMoveEvent
    {
        public double X { get; set; }
        public double Y { get; set; }

        public MouseMoveEvent(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
