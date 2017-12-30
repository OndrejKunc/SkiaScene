using SkiaSharp;
using System;

namespace SkiaScene
{
    public static class SKPointExtensions
    {
        public static float GetMagnitude(this SKPoint point)
        {
            return (float)Math.Sqrt(Math.Pow(point.X, 2) + Math.Pow(point.Y, 2));
        }
    }
}
