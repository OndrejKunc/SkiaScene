using SkiaScene.TouchTracking;
using SkiaSharp;

namespace SkiaScene
{
    public interface ITouchManipulationManager
    {
        TouchManipulationMode TouchManipulationMode { get; set; }
        void ProcessTouchEvent(long id, TouchActionType type, SKPoint location);
    }
}
