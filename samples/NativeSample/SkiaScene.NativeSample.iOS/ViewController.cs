using SkiaScene.TouchManipulation;
using SkiaSharp;
using SkiaSharp.Views.iOS;
using System;
using Foundation;
using TouchTracking.iOS;
using UIKit;

namespace SkiaScene.NativeSample.iOS
{
    public partial class ViewController : UIViewController
    {
        private ISKScene _scene;
        private ITouchGestureRecognizer _touchGestureRecognizer;
        private ISceneGestureResponder _sceneGestureResponder;
        private SKCanvasView _canvasView;
        private TouchHandler _touchHandler;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            
            _canvasView = MainView;

            _canvasView.PaintSurface += OnPaint;
            _touchHandler = new TouchHandler();
            _touchHandler.RegisterEvents(_canvasView);
            _touchHandler.TouchAction += OnTouch;

            Foundation.NSNotificationCenter.DefaultCenter.AddObserver(
                new NSString("UIDeviceOrientationDidChangeNotification"), OnDeviceRotation);
        }

        private void OnDeviceRotation(NSNotification notification)
        {
            SetSceneCenter();
        }


        private void OnTouch(object sender, TouchTracking.TouchActionEventArgs args)
        {
            var viewPoint = args.Location;
            SKPoint point =
                new SKPoint((float)(_canvasView.CanvasSize.Width * viewPoint.X / _canvasView.Frame.Width),
                            (float)(_canvasView.CanvasSize.Height * viewPoint.Y / _canvasView.Frame.Height));

            var actionType = args.Type;
            _touchGestureRecognizer.ProcessTouchEvent(args.Id, actionType, point);
        }

        private void SetSceneCenter()
        {
            if (_scene == null)
            {
                return;
            }
            var centerPoint = new SKPoint(_canvasView.CanvasSize.Width / 2, _canvasView.CanvasSize.Height / 2);
            _scene.ScreenCenter = centerPoint;
        }

        private void InitSceneObjects()
        {
            _scene = new SKScene(new SvgSceneRenderer())
            {
                MaxScale = 1000,
                MinScale = 0.001f,
            };
            SetSceneCenter();
            _touchGestureRecognizer = new TouchGestureRecognizer();
            _sceneGestureResponder = new SceneGestureRenderingResponder(() => _canvasView.SetNeedsDisplay(), _scene, _touchGestureRecognizer)
            {
                TouchManipulationMode = TouchManipulationMode.ScaleRotate,
                MaxFramesPerSecond = 100,
            };
            _sceneGestureResponder.StartResponding();
        }

        private void OnPaint(object sender, SKPaintSurfaceEventArgs args)
        {
            if (_scene == null)
            {
                InitSceneObjects();

            }
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;
            _scene.Render(canvas);
        }
    }
}
