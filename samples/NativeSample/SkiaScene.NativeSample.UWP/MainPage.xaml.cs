using SkiaScene.TouchManipulation;
using SkiaSharp;
using SkiaSharp.Views.UWP;
using System;
using Windows.UI.Xaml;
using TouchTracking.UWP;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Window.Current.SizeChanged += OnWindowSizeChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Window.Current.SizeChanged -= OnWindowSizeChanged;
        }

        private void OnWindowSizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            SetSceneCenter();
        }

        private void OnTouch(object sender, TouchTracking.TouchActionEventArgs args)
        {
            var viewPoint = args.Location;
            SKPoint point =
                new SKPoint((float)(CanvasView.CanvasSize.Width * viewPoint.X / CanvasView.ActualWidth),
                            (float)(CanvasView.CanvasSize.Height * viewPoint.Y / CanvasView.ActualHeight));

            var actionType = args.Type;
            _touchManipulationRenderer.Render(point, actionType, args.Id);
        }

        private void SetSceneCenter()
        {
            if (_scene == null)
            {
                return;
            }
            var centerPoint = new SKPoint(CanvasView.CanvasSize.Width / 2, CanvasView.CanvasSize.Height / 2);
            _scene.ScreenCenter = centerPoint;
        }

        private void InitSceneObjects()
        {
            _scene = new SKScene(new SvgSceneRenderer());
            SetSceneCenter();
            _touchManipulationManager = new TouchManipulationManager(_scene)
            {
                TouchManipulationMode = TouchManipulationMode.IsotropicScale,
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