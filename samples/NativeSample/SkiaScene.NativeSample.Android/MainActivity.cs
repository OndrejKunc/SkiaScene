using Android.App;
using Android.OS;
using SkiaSharp.Views.Android;
using SkiaSharp;
using SkiaScene.TouchManipulation;
using TouchTracking.Droid;

namespace SkiaScene.NativeSample.Droid
{
    [Activity(Label = "SkiaScene.NativeSample", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        private ISKScene _scene;
        private ITouchGestureRecognizer _touchGestureRecognizer;
        private ISceneGestureResponder _sceneGestureResponder;
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
            var viewPoint = args.Location;
            var pixelPoint = new SKPoint(this.ToPixels(viewPoint.X), this.ToPixels(viewPoint.Y));
            SKPoint point =
                new SKPoint((float)(_canvasView.CanvasSize.Width * pixelPoint.X / _canvasView.Width),
                            (float)(_canvasView.CanvasSize.Height * pixelPoint.Y / _canvasView.Height));

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
            _sceneGestureResponder = new SceneGestureRenderingResponder(() => _canvasView.Invalidate(), _scene, _touchGestureRecognizer)
            {
                TouchManipulationMode = TouchManipulationMode.IsotropicScale,
                EnableTwoFingersPanInIsotropicScaleMode = true,
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
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;
            _scene.Render(canvas);
        }
    }
}

