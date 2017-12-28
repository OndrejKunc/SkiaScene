using SkiaScene.TouchTracking;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace SkiaScene
{
    //Inspiration from https://developer.xamarin.com/guides/xamarin-forms/advanced/skiasharp/transforms/touch/
    public class TouchManipulationManager : ITouchManipulationManager
    {
        private readonly ISKScene _skScene;
        private Dictionary<long, TouchManipulationInfo> touchDictionary = new Dictionary<long, TouchManipulationInfo>();

        public TouchManipulationManager(ISKScene skScene)
        {
            _skScene = skScene;
        }

        public TouchManipulationMode TouchManipulationMode { get; set; } = TouchManipulationMode.IsotropicScale;

        public void ProcessTouchEvent(long id, TouchActionType type, SKPoint location)
        {
            switch (type)
            {
                case TouchActionType.Pressed:
                    touchDictionary.Add(id, new TouchManipulationInfo
                    {
                        PreviousPoint = location,
                        NewPoint = location
                    });
                    break;

                case TouchActionType.Moved:
                    TouchManipulationInfo info = touchDictionary[id];
                    info.NewPoint = location;
                    Manipulate();
                    info.PreviousPoint = info.NewPoint;
                    break;

                case TouchActionType.Released:
                    touchDictionary[id].NewPoint = location;
                    Manipulate();
                    touchDictionary.Remove(id);
                    break;

                case TouchActionType.Cancelled:
                    touchDictionary.Remove(id);
                    break;
            }
        }

        private void Manipulate()
        {
            TouchManipulationInfo[] infos = new TouchManipulationInfo[touchDictionary.Count];
            touchDictionary.Values.CopyTo(infos, 0);

            if (infos.Length == 1)
            {
                SKPoint prevPoint = infos[0].PreviousPoint;
                SKPoint newPoint = infos[0].NewPoint;
                SKPoint resultVector = newPoint - prevPoint;
                _skScene.MoveByVector(resultVector);
            }
            else if (infos.Length >= 2)
            {
                int pivotIndex = infos[0].NewPoint == infos[0].PreviousPoint ? 0 : 1;
                SKPoint pivotPoint = infos[pivotIndex].NewPoint;
                SKPoint newPoint = infos[1 - pivotIndex].NewPoint;
                SKPoint previousPoint = infos[1 - pivotIndex].PreviousPoint;
                ManipulateTwoFingers(previousPoint, newPoint, pivotPoint);
            }
        }

        private void ManipulateTwoFingers(SKPoint previousPoint, SKPoint newPoint, SKPoint pivotPoint)
        {
            SKMatrix touchMatrix = SKMatrix.MakeIdentity();
            SKPoint oldVector = previousPoint - pivotPoint;
            SKPoint newVector = newPoint - pivotPoint;

            if (TouchManipulationMode == TouchManipulationMode.ScaleRotate)
            {
                // Find angles from pivot point to touch points
                float oldAngle = (float)Math.Atan2(oldVector.Y, oldVector.X);
                float newAngle = (float)Math.Atan2(newVector.Y, newVector.X);

                // Calculate rotation matrix
                float angle = newAngle - oldAngle;
                _skScene.RotateByRadiansDelta(pivotPoint, angle);

                // Effectively rotate the old vector
                float magnitudeRatio = oldVector.GetMagnitude() / newVector.GetMagnitude();
                oldVector.X = magnitudeRatio * newVector.X;
                oldVector.Y = magnitudeRatio * newVector.Y;
            }

            var scale = newVector.GetMagnitude() / oldVector.GetMagnitude();

            if (!float.IsNaN(scale) && !float.IsInfinity(scale))
            {
                _skScene.ZoomByScaleFactor(pivotPoint, scale);
            }
        }
    }
}
