using System;
using SkiaSharp;

namespace SkiaScene
{
    public class SKScene : ISKScene
    {
        protected readonly ISKSceneRenderer _sceneRenderer;
        protected readonly Action invalidateSceneAction;
        protected SKMatrix Matrix = SKMatrix.MakeIdentity();
        protected readonly SKPoint DefaultCenter;

        public SKScene(ISKSceneRenderer sceneRenderer, Action invalidateSceneAction, SKSize canvasSize)
        {
            _sceneRenderer = sceneRenderer;
            this.invalidateSceneAction = invalidateSceneAction;
            DefaultCenter = new SKPoint(canvasSize.Width / 2, canvasSize.Height / 2);
            Center = DefaultCenter;
        }

        public SKPoint Center { get; private set; }

        public float Scale { get; private set; } = 1;

        public float AngleInRadians { get; private set; }

        public void Render(SKCanvas canvas)
        {
            canvas.SetMatrix(Matrix);
            _sceneRenderer.Render(canvas, AngleInRadians, Center, Scale);
        }

        public void MoveBySize(SKSize size)
        {
            SKMatrix invertedMatrix;
            if (!Matrix.TryInvert(out invertedMatrix))
            { 
                return;
            }
            var resultPoint = invertedMatrix.MapVector(size.Width, size.Height);
            SKMatrix.PreConcat(ref Matrix, SKMatrix.MakeTranslation(resultPoint.X, resultPoint.Y));
            RecalculateCenter();
            invalidateSceneAction();
        }
        
        public void MoveToPoint(SKPoint point)
        {
            SKPoint diff = Center - point;
            Center = point;
            SKMatrix.PreConcat(ref Matrix, SKMatrix.MakeTranslation(diff.X, diff.Y));
            RecalculateCenter();
            invalidateSceneAction();
        }
        
        public void Rotate(SKPoint point, float radians)
        {
            var angleDiff = radians - AngleInRadians;
            AngleInRadians = radians;
            SKMatrix.PreConcat(ref Matrix, SKMatrix.MakeRotation(angleDiff, point.X, point.Y));
            RecalculateCenter();
            invalidateSceneAction();
        }

        public void RotateByRadiansDelta(SKPoint point, float radiansDelta)
        {
            AngleInRadians += radiansDelta;
            SKMatrix.PreConcat(ref Matrix, SKMatrix.MakeRotation(radiansDelta, point.X, point.Y));
            RecalculateCenter();
            invalidateSceneAction();
        }

        public void Zoom(SKPoint point, float scale)
        {
            var scaleFactor = scale / Scale;
            Scale = scale;
            SKMatrix.PreConcat(ref Matrix, SKMatrix.MakeScale(scaleFactor, scaleFactor, point.X, point.Y));
            RecalculateCenter();
            invalidateSceneAction();
        }
        
        public void ZoomByScaleFactor(SKPoint point, float scaleFactor)
        {
            Scale *= scaleFactor;
            SKMatrix.PreConcat(ref Matrix, SKMatrix.MakeScale(scaleFactor, scaleFactor, point.X, point.Y));
            RecalculateCenter();
            invalidateSceneAction();
        }


        public SKPoint GetCanvasPointFromViewPoint(SKPoint viewPoint)
        {

            SKMatrix invertedMatrix;
            if (!Matrix.TryInvert(out invertedMatrix))
            {
                return SKPoint.Empty;
            }
            return invertedMatrix.MapPoint(viewPoint);
        }

        private void RecalculateCenter()
        {
            Center = GetCanvasPointFromViewPoint(DefaultCenter);
        }
    }
}
