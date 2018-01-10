using TouchTracking;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SkiaScene.TouchManipulation
{
    //Inspiration from https://developer.xamarin.com/guides/xamarin-forms/advanced/skiasharp/transforms/touch/
    public class TouchManipulationManager : ITouchManipulationManager
    {
        private readonly ISKScene _skScene;

        private readonly Dictionary<long, TouchManipulationInfo> touchDictionary =
            new Dictionary<long, TouchManipulationInfo>();
        protected DateTime LastTapTime = DateTime.MinValue;
        protected DateTime LastDoubleTapTime = DateTime.MinValue;
        protected TimeSpan DoubleTapDelay = TimeSpan.FromMilliseconds(320);
        private Timer _timer;

        public event TapEventHandler OnTap;
        public event TapEventHandler OnDoubleTap;
        public event TapEventHandler OnSingleTap;

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
                        NewPoint = location,
                        MoveCounter = 0
                    });
                    break;

                case TouchActionType.Moved:
                    TouchManipulationInfo info = touchDictionary[id];
                    info.NewPoint = location;
                    info.MoveCounter = info.MoveCounter + 1;
                    Manipulate();
                    info.PreviousPoint = info.NewPoint;
                    break;

                case TouchActionType.Released:
                    touchDictionary[id].NewPoint = location;
                    ProcessTap();
                    Manipulate();
                    touchDictionary.Remove(id);
                    break;

                case TouchActionType.Cancelled:
                    touchDictionary.Remove(id);
                    break;
            }
        }

        private void ProcessTap()
        {
            TouchManipulationInfo[] infos = new TouchManipulationInfo[touchDictionary.Count];
            touchDictionary.Values.CopyTo(infos, 0);
            if (infos.Length != 1)
            {
                return;
            }
            SKPoint point = infos[0].PreviousPoint;
            if (infos[0].MoveCounter <= 1)
            {
                return;
            }
            var scenePoint = _skScene.GetCanvasPointFromViewPoint(point);
            var tapEventArgs = new TapEventArgs(point, scenePoint);
            
            var now = DateTime.Now;
            var lastTapTime = LastTapTime;
            LastTapTime = now;

            System.Diagnostics.Debug.WriteLine("Invoking tap");
            OnTap?.Invoke(this, tapEventArgs);
            if (now - lastTapTime < DoubleTapDelay)
            {
                System.Diagnostics.Debug.WriteLine("Invoking double tap");
                OnDoubleTap?.Invoke(this, tapEventArgs);
                LastDoubleTapTime = now;
                LastTapTime = DateTime.MinValue; //Reset double tap timer
            }
            else
            {
                _timer = new Timer(_ =>
                {
                    System.Diagnostics.Debug.WriteLine("On Timer");
                    if (DateTime.Now - LastDoubleTapTime < DoubleTapDelay)
                    {
                        return;
                    }
                    System.Diagnostics.Debug.WriteLine("Invoking single tap");
                    OnSingleTap?.Invoke(this, tapEventArgs);
                }, null, DoubleTapDelay.Milliseconds, Timeout.Infinite);
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
            previousPoint = _skScene.GetCanvasPointFromViewPoint(previousPoint);
            newPoint = _skScene.GetCanvasPointFromViewPoint(newPoint);
            pivotPoint = _skScene.GetCanvasPointFromViewPoint(pivotPoint);


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
