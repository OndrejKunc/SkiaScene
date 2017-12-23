using SkiaSharp;

namespace SkiaScene
{
    public interface ISKScene
    {
        void Render(SKCanvas canvas);
        void Zoom(SKPoint point, float scale);
        void ZoomByScaleFactor(SKPoint point, float scaleDelta);
        void MoveToPoint(SKPoint point);
        void MoveBySize(SKSize size);
        void Rotate(SKPoint point, float radians);
        void RotateByRadiansDelta(SKPoint point, float radiansDelta);
        SKPoint Center { get; }
        float Scale { get; }
        float AngleInRadians { get; }
    }
}
