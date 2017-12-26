using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace SkiaScene.FormsSample
{
    public partial class MainPage : ContentPage
    {
        //Prevent glitch when only one finger is released after pinch and pan is immediatelly started
        private DateTime lastPinchCompleted = DateTime.MinValue;
        private SKScene _scene;
        private double previousPanX;
        private double previousPanY;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnPan(object sender, PanUpdatedEventArgs args)
        {
            Debug.WriteLine($"Pan - X:{args.TotalX} Y:{args.TotalY} Status:{args.StatusType}");
            if (DateTime.Now - lastPinchCompleted < TimeSpan.FromMilliseconds(200))
            {
                Debug.WriteLine($"Pan - Ignoring");
                previousPanX = args.TotalX;
                previousPanY = args.TotalY;
                return;
            }
            var scale = canvasView.CanvasSize.Width / (float)canvasView.Width;
            switch (args.StatusType)
            {
                case GestureStatus.Started:
                    previousPanX = 0;
                    previousPanY = 0;
                    break;
                case GestureStatus.Running:
                    var currentX = (float)(args.TotalX - previousPanX);
                    var currentY = (float)(args.TotalY - previousPanY);
                    previousPanX = args.TotalX;
                    previousPanY = args.TotalY;
                    var size = new SKSize(currentX * scale, currentY * scale);
                    _scene.MoveBySize(size);
                    canvasView.InvalidateSurface();
                    break;
                case GestureStatus.Completed:
                    previousPanX = 0;
                    previousPanY = 0;
                    break;
                case GestureStatus.Canceled:
                    previousPanX = 0;
                    previousPanY = 0;
                    break;

            }
        }

        private void OnPinch(object sender, PinchGestureUpdatedEventArgs args)
        {
            Debug.WriteLine($"Pinch - Scale:{args.Scale} Point:{args.ScaleOrigin} Status:{args.Status}");
            var viewPoint = GetCanvasPointFromScalePoint(args.ScaleOrigin);
            var canvasPoint = _scene.GetCanvasPointFromViewPoint(viewPoint);
            _scene.ZoomByScaleFactor(canvasPoint, (float)args.Scale);
            canvasView.InvalidateSurface();
            if (args.Status == GestureStatus.Completed)
            {
                lastPinchCompleted = DateTime.Now;
            }
        }

        private void OnTap(object sender, EventArgs e)
        {
            
        }

        private void OnPaint(object sender, SKPaintSurfaceEventArgs args)
        {
            if (_scene == null)
            {
                _scene = new SKScene(new TestScenereRenderer(), canvasView.CanvasSize);

            }
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;
            _scene.Render(canvas);
        }

        private SKPoint GetCanvasPointFromScalePoint(Point scalePoint)
        {
            return new SKPoint((float)(scalePoint.X * canvasView.CanvasSize.Width), (float)(scalePoint.Y * canvasView.CanvasSize.Height));
        }
    }

    public enum GestureState
    {
        Started,
        Running,
        Completed,
        Cancelled
    }

}
