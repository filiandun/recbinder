using RecBinderProject.Models.Inputs;

namespace RecBinderProject.Models.Inputs
{
    public struct MouseMoveInput
    {
        public double X { get; set; }
        public double Y { get; set; }
        public int TimeOffset { get; set; }

        public MouseMoveInput(double X, double Y, int timeOffset)
        {
            this.X = X;
            this.Y = Y;
            TimeOffset = timeOffset;
        }
    }
}
