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
        public bool EnableTwoFingersPanInIsotropicScaleMode { get; set; }
        public float DoubleTapScaleFactor { get; set; } = 2f;

        public void StartResponding()
        {
            _touchGestureRecognizer.OnPan += TouchGestureRecognizerOnPan;
            _touchGestureRecognizer.OnPinch += TouchGestureRecognizerOnPinch;
            _touchGestureRecognizer.OnDoubleTap += TouchGestureRecognizerOnDoubleTap;
        }
        public void StopResponding()
        {
            _touchGestureRecognizer.OnPan -= TouchGestureRecognizerOnPan;
            _touchGestureRecognizer.OnPinch -= TouchGestureRecognizerOnPinch;
            _touchGestureRecognizer.OnDoubleTap -= TouchGestureRecognizerOnDoubleTap;
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
                float angle = GetAngleBetweenVectors(oldVector, newVector);

                _skScene.RotateByRadiansDelta(pivotPoint, angle);

                // Effectively rotate the old vector
                float magnitudeRatio = oldVector.GetMagnitude() / newVector.GetMagnitude();
                oldVector.X = magnitudeRatio * newVector.X;
                oldVector.Y = magnitudeRatio * newVector.Y;
            }
            else if (TouchManipulationMode == TouchManipulationMode.IsotropicScale 
                && EnableTwoFingersPanInIsotropicScaleMode)
            {
                float angle = GetAngleBetweenVectors(oldVector, newVector);

                // Calculate the movement as a distance between old vector and a new vector
                // but in orthogonal direction to the old vector.

                float oldVectorOriginPoint = newVector.GetMagnitude() * (float) Math.Cos(angle);
                float magnitudeRatio = oldVectorOriginPoint / oldVector.GetMagnitude();
                SKPoint oldVectorOrigin = new SKPoint(oldVector.X * magnitudeRatio, oldVector.Y * magnitudeRatio);
                SKPoint moveVector = newVector - oldVectorOrigin;

                _skScene.MoveByVector(moveVector);
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

        protected virtual void TouchGestureRecognizerOnDoubleTap(object sender, TapEventArgs args)
        {
            SKPoint scenePoint = _skScene.GetCanvasPointFromViewPoint(args.ViewPoint);
            _skScene.ZoomByScaleFactor(scenePoint, DoubleTapScaleFactor);
        }

        private float GetAngleBetweenVectors(SKPoint oldVector, SKPoint newVector)
        {
            // Find angles from pivot point to touch points
            float oldAngle = (float)Math.Atan2(oldVector.Y, oldVector.X);
            float newAngle = (float)Math.Atan2(newVector.Y, newVector.X);

            float angle = newAngle - oldAngle;
            return angle;
        }
    }
}