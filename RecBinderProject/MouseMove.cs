namespace RecBinderProject
{
    public struct MouseMove
    {
        public double X { get; set; }
        public double Y { get; set; }
        public int DelayMs { get; set; }

        public MouseMove(double X, double Y, int delayMs)
        {
            this.X = X;
            this.Y = Y;
            this.DelayMs = delayMs;
        }
    }
}
