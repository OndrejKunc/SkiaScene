using SkiaScene.iOS;
using SkiaSharp;
using SkiaSharp.Views.iOS;
using System;

using UIKit;

namespace SkiaScene.NativeSample.iOS
{
    public partial class ViewController : UIViewController
    {
        private ISKScene _scene;
        private ITouchManipulationManager _touchManipulationManager;
        private ITouchManipulationRenderer _touchManipulationRenderer;
        private SKCanvasView _canvasView;
        private TouchHandler _touchHandler;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //TODO test this
            _canvasView = new SKCanvasView();
            View.Add(_canvasView);

            _canvasView.PaintSurface += OnPaint;
            _touchHandler = new TouchHandler();
            _touchHandler.RegisterEvents(_canvasView);
            _touchHandler.TouchAction += OnTouch;
        }


        private void OnTouch(object sender, TouchTracking.TouchActionEventArgs args)
        {
            SKPoint viewPoint = args.Location;
            SKPoint point =
                new SKPoint((float)(_canvasView.CanvasSize.Width * viewPoint.X / _canvasView.Frame.Width),
                            (float)(_canvasView.CanvasSize.Height * viewPoint.Y / _canvasView.Frame.Height));

            var actionType = args.Type;
            _touchManipulationRenderer.Render(point, actionType, args.Id);
        }

        private void InitSceneObjects()
        {
            _scene = new SKScene(new TestScenereRenderer(), _canvasView.CanvasSize);
            _touchManipulationManager = new TouchManipulationManager(_scene)
            {
                TouchManipulationMode = TouchManipulationMode.ScaleRotate,
            };
            _touchManipulationRenderer = new TouchManipulationRenderer(_touchManipulationManager, () => _canvasView.SetNeedsDisplay())
            {
                MaxFramesPerSecond = 100,
            };
        }

        private void OnPaint(object sender, SKPaintSurfaceEventArgs args)
        {
            if (_scene == null)
            {
                InitSceneObjects();

            }
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;
            _scene.Render(canvas);
        }
    }
}
