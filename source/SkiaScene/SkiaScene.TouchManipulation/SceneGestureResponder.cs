using System;
using SkiaSharp;

namespace SkiaScene.TouchManipulation
{
    public class SceneGestureResponder : ISceneGestureResponder
    {
        private readonly ISKScene _skScene;
        private readonly ITouchGestureRecognizer _touchGestureRecognizer;

        public SceneGestureResponder(ISKScene skScene, ITouchGestureRecognizer touchGestureRecognizer)
        {
            _skScene = skScene;
            _touchGestureRecognizer = touchGestureRecognizer;
        }

        public TouchManipulationMode TouchManipulationMode { get; set; }

        public void StartResponding()
        {
            _touchGestureRecognizer.OnPan += TouchGestureRecognizerOnPan;
            _touchGestureRecognizer.OnPinch += TouchGestureRecognizerOnPinch;
        }

        public void StopResponding()
        {
            _touchGestureRecognizer.OnPan -= TouchGestureRecognizerOnPan;
            _touchGestureRecognizer.OnPinch -= TouchGestureRecognizerOnPinch;
        }


        protected virtual void TouchGestureRecognizerOnPinch(object sender, PinchEventArgs args)
        {
            var previousPoint = _skScene.GetCanvasPointFromViewPoint(args.PreviousPoint);
            var newPoint = _skScene.GetCanvasPointFromViewPoint(args.NewPoint);
            var pivotPoint = _skScene.GetCanvasPointFromViewPoint(args.PivotPoint);

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

        protected virtual void TouchGestureRecognizerOnPan(object sender, PanEventArgs args)
        {
            SKPoint resultVector = args.NewPoint - args.PreviousPoint;
            _skScene.MoveByVector(resultVector);
        }
    }
}