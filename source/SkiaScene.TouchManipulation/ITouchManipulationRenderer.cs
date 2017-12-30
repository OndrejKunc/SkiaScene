using TouchTracking;
using SkiaSharp;

namespace SkiaScene.TouchManipulation
{
    public interface ITouchManipulationRenderer
    {
        void Render(SKPoint canvasPoint, TouchActionType touchActionType, long touchId);
        int MaxFramesPerSecond { get; set; }
    }
}
