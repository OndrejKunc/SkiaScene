using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using SkiaScene.TouchTracking;
using Xamarin.Forms;

namespace SkiaScene.FormsSample
{
    public partial class MainPage : ContentPage
    {
        //Prevent glitch when only one finger is released after pinch and pan is immediatelly started
        private DateTime lastPinchCompleted = DateTime.MinValue;
        private ISKScene _scene;
        private ITouchManipulationManager _touchManipulationManger;
        private double previousPanX;
        private double previousPanY;

        List<long> touchIds = new List<long>();

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnTouchEffectAction(object sender, TouchActionEventArgs args)
        {
            SKPoint pt = args.Location;
            SKPoint point =
                new SKPoint((float)(canvasView.CanvasSize.Width * pt.X / canvasView.Width),
                            (float)(canvasView.CanvasSize.Height * pt.Y / canvasView.Height));

            var actionType = args.Type;

            switch (actionType)
            {
                case TouchActionType.Pressed:
                    touchIds.Add(args.Id);
                    _touchManipulationManger.ProcessTouchEvent(args.Id, actionType, point);
                    break;
                case TouchActionType.Moved:
                    if (touchIds.Contains(args.Id))
                    {
                        _touchManipulationManger.ProcessTouchEvent(args.Id, actionType, point);
                        canvasView.InvalidateSurface();
                    }
                    break;

                case TouchActionType.Released:
                case TouchActionType.Cancelled:
                    if (touchIds.Contains(args.Id))
                    {
                        _touchManipulationManger.ProcessTouchEvent(args.Id, actionType, point);
                        touchIds.Remove(args.Id);
                        canvasView.InvalidateSurface();
                    }
                    break;
            }
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
                    var vector = new SKPoint(currentX * scale, currentY * scale);
                    _scene.MoveByVector(vector);
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
                _touchManipulationManger = new TouchManipulationManager(_scene);
                _touchManipulationManger.TouchManipulationMode = TouchManipulationMode.ScaleRotate;

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
}
