using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using TouchTracking;

namespace SkiaScene.TouchManipulation
{
    public class TapEventArgs : EventArgs
    {
        public SKPoint ViewPoint { get; }
        public SKPoint ScenePoint { get; }

        public TapEventArgs(SKPoint viewPoint, SKPoint scenePoint)
        {
            ViewPoint = viewPoint;
            ScenePoint = scenePoint;
        }
    }
}
