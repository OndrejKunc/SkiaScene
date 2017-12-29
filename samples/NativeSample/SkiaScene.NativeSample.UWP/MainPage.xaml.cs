using SkiaScene.UWP;
using SkiaSharp;
using SkiaSharp.Views.UWP;
using System;
using Windows.UI.Xaml.Controls;

namespace SkiaScene.NativeSample.UWP
{
    public sealed partial class MainPage : Page
    {
        private ISKScene _scene;
        private ITouchManipulationManager _touchManipulationManager;
        private ITouchManipulationRenderer _touchManipulationRenderer;
        private TouchHandler _touchHandler;


        public MainPage()
        {
            InitializeComponent();

            CanvasView.PaintSurface += OnPaint;
            _touchHandler = new TouchHandler();
            _touchHandler.RegisterEvents(CanvasView);
            _touchHandler.TouchAction += OnTouch;
            CanvasView.PointerWheelChanged += OnPointerWheelChanged;
        }

        private void OnTouch(object sender, TouchTracking.TouchActionEventArgs args)
        {
            SKPoint viewPoint = args.Location;
            SKPoint point =
                new SKPoint((float)(CanvasView.CanvasSize.Width * viewPoint.X / CanvasView.ActualWidth),
                            (float)(CanvasView.CanvasSize.Height * viewPoint.Y / CanvasView.ActualHeight));

            var actionType = args.Type;
            _touchManipulationRenderer.Render(point, actionType, args.Id);
        }

        private void InitSceneObjects()
        {
            _scene = new SKScene(new TestScenereRenderer(), CanvasView.CanvasSize);
            _touchManipulationManager = new TouchManipulationManager(_scene)
            {
                TouchManipulationMode = TouchManipulationMode.ScaleRotate,
            };
            _touchManipulationRenderer = new TouchManipulationRenderer(_touchManipulationManager, () => CanvasView.Invalidate())
            {
                MaxFramesPerSecond = 30,
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

        private void OnPointerWheelChanged(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            const float zoomFactor = 1.1f;
            var wheelDelta = e.GetCurrentPoint(CanvasView).Properties.MouseWheelDelta;
            float currentZoomFactor = wheelDelta > 0 ? zoomFactor : 1 / zoomFactor;

            _scene.ZoomByScaleFactor(_scene.GetCenter(), currentZoomFactor);
            CanvasView.Invalidate();
        }

    }
}