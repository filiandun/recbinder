namespace RecBinderProject.Domain.MacroEvents
{
    public struct MouseMoveEvent
    {
        public double X { get; set; }
        public double Y { get; set; }

        public MouseMoveEvent(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }
}
