using SkiaSharp;
using SkiaSharp.Views.Forms;
using SkiaScene.TouchTracking;
using Xamarin.Forms;

namespace SkiaScene.FormsSample
{
    public partial class MainPage : ContentPage
    {
        private ISKScene _scene;
        private ITouchManipulationManager _touchManipulationManger;
        private ITouchManipulationRenderer _touchManipulationRenderer;

        public MainPage()
        {
            InitializeComponent();
        }


        private void InitSceneObjects()
        {
            _scene = new SKScene(new TestScenereRenderer(), canvasView.CanvasSize);
            _touchManipulationManger = new TouchManipulationManager(_scene)
            {
                TouchManipulationMode = TouchManipulationMode.ScaleRotate
            };
            _touchManipulationRenderer = new TouchManipulationRenderer(_touchManipulationManger, () => canvasView.InvalidateSurface())
            {
                MaxFramesPerSecond = 100,
            };
        }

        private void OnTouchEffectAction(object sender, TouchActionEventArgs args)
        {
            SKPoint viewPoint = args.Location;
            SKPoint point =
                new SKPoint((float)(canvasView.CanvasSize.Width * viewPoint.X / canvasView.Width),
                            (float)(canvasView.CanvasSize.Height * viewPoint.Y / canvasView.Height));

            var actionType = args.Type;
            _touchManipulationRenderer.Render(point, actionType, args.Id);
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
