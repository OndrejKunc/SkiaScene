using Android.Content;
using Android.Util;

namespace SkiaScene.Droid
{
    public static class ContextExtensions
    {
        static float _displayDensity = float.MinValue;
        
        public static double FromPixels(this Context self, double pixels)
        {
            SetupMetrics(self);
            return pixels / _displayDensity;
        }
        
        static void SetupMetrics(Context context)
        {
            if (_displayDensity != float.MinValue)
            {
                return;
            }

            using (DisplayMetrics metrics = context.Resources.DisplayMetrics)
            {
                _displayDensity = metrics.Density;
            }
        }
    }
}