using SkiaScene.TouchTracking;
using Xamarin.Forms;

namespace SkiaScene.Forms.NetStandard
{
    public class TouchEffect : RoutingEffect
    {
        public event TouchActionEventHandler TouchAction;

        public TouchEffect() : base("SkiaScene.TouchEffect")
        {
        }

        public bool Capture { set; get; }

        public void OnTouchAction(object element, TouchActionEventArgs args)
        {
            TouchAction?.Invoke(element, args);
        }
    }
}
