using Android.App;
using Android.Widget;
using Android.OS;
using SkiaSharp.Views.Android;
using SkiaSharp;
using SkiaScene.Droid;

namespace SkiaScene.NativeSample.Droid
{
    [Activity(Label = "SkiaScene.NativeSample", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        private ISKScene _scene;
        private ITouchManipulationManager _touchManipulationManager;
        private ITouchManipulationRenderer _touchManipulationRenderer;
        private SKCanvasView _canvasView;
        private TouchHandler _touchHandler;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            _canvasView = FindViewById<SKCanvasView>(Resource.Id.canvasView);

            _canvasView.PaintSurface += OnPaint;
            _touchHandler = new TouchHandler();
            _touchHandler.RegisterEvents(_canvasView);
            _touchHandler.TouchAction += OnTouch;
        }

        private void OnTouch(object sender, TouchTracking.TouchActionEventArgs args)
        {
            SKPoint viewPoint = args.Location;
            var pixelPoint = new SKPoint(this.ToPixels(viewPoint.X), this.ToPixels(viewPoint.Y));
            SKPoint point =
                new SKPoint((float)(_canvasView.CanvasSize.Width * pixelPoint.X / _canvasView.Width),
                            (float)(_canvasView.CanvasSize.Height * pixelPoint.Y / _canvasView.Height));

            var actionType = args.Type;
            _touchManipulationRenderer.Render(point, actionType, args.Id);
        }

        private void InitSceneObjects()
        {
            _scene = new SKScene(new SvgSceneRenderer(), _canvasView.CanvasSize);
            _touchManipulationManager = new TouchManipulationManager(_scene)
            {
                TouchManipulationMode = TouchManipulationMode.ScaleRotate,
            };
            _touchManipulationRenderer = new TouchManipulationRenderer(_touchManipulationManager, () => _canvasView.Invalidate())
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

