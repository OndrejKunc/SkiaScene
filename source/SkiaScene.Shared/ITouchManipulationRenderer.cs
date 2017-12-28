using SkiaScene.TouchTracking;
using SkiaSharp;

namespace SkiaScene
{
    public interface ITouchManipulationRenderer
    {
        void Render(SKPoint canvasPoint, TouchActionType touchActionType, long touchId);
        int MaxFramesPerSecond { get; set; }
    }
}
