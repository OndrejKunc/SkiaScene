using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using SkiaScene.TouchTracking;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace SkiaScene.FormsSample
{
    public partial class MainPage : ContentPage
    {
        //Prevent glitch when only one finger is released after pinch and pan is immediatelly started
        private ISKScene _scene;
        private ITouchManipulationManager _touchManipulationManger;
        private TimeSpan _minGestureDuration = TimeSpan.FromMilliseconds(60);
        private Dictionary<long, DateTime> _lastGestureTime = new Dictionary<long, DateTime>();

        List<long> touchIds = new List<long>();

        public MainPage()
        {
            InitializeComponent();
        }
        
        private void OnTouchEffectAction(object sender, TouchActionEventArgs args)
        {
            SKPoint viewPoint = args.Location;
            SKPoint point =
                new SKPoint((float)(canvasView.CanvasSize.Width * viewPoint.X / canvasView.Width),
                            (float)(canvasView.CanvasSize.Height * viewPoint.Y / canvasView.Height));

            var actionType = args.Type;

            switch (actionType)
            {
                case TouchActionType.Pressed:
                    touchIds.Add(args.Id);
                    _touchManipulationManger.ProcessTouchEvent(args.Id, actionType, point);
                    _lastGestureTime[args.Id] = DateTime.Now;
                    break;
                case TouchActionType.Moved:
                    if (touchIds.Contains(args.Id))
                    {
                        _touchManipulationManger.ProcessTouchEvent(args.Id, actionType, point);
                        var now = DateTime.Now;
                        if (now - _lastGestureTime[args.Id] < _minGestureDuration)
                        {
                            return;
                        }
                        _lastGestureTime[args.Id] = now;
                        canvasView.InvalidateSurface();
                    }
                    break;

                case TouchActionType.Released:
                case TouchActionType.Cancelled:
                    if (touchIds.Contains(args.Id))
                    {
                        _lastGestureTime[args.Id] = DateTime.MinValue;
                        _touchManipulationManger.ProcessTouchEvent(args.Id, actionType, point);
                        touchIds.Remove(args.Id);
                        canvasView.InvalidateSurface();
                    }
                    break;
            }
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
    }
}
