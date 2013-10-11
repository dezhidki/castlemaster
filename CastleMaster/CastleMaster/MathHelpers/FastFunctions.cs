
using Microsoft.Xna.Framework;
using System;
namespace CastleMaster.MathHelpers
{
    public static class FastFunctions
    {
        public static uint ToABGR(uint argb)
        {
            uint ag = argb & 0xFF00FF00;
            uint r = (argb & 0x00FF0000) >> 16;
            uint b = (argb & 0x000000FF) << 16;

            return ag | r | b;
        }

        public static Rectangle RectangleFromPoints(Point p0, Point p1)
        {
            int width = Math.Abs(p1.X - p0.X);
            int height = Math.Abs(p1.Y - p0.Y);

            int x = p0.X;
            int y = p0.Y;

            if (p1.X < p0.X)
                x = p1.X;
            if (p1.Y < p0.Y)
                y = p1.Y;

            return new Rectangle(x, y, width, height);
        }
    }
}
