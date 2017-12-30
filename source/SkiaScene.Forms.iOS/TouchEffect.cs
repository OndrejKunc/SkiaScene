using System.Linq;
using TouchTracking.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("TouchTracking")]
[assembly: ExportEffect(typeof(TouchTracking.Forms.iOS.TouchEffect), "TouchEffect")]
namespace TouchTracking.Forms.iOS
{
    public class TouchEffect : PlatformEffect
    {
        private TouchHandler _touchHandler;
        private UIView _view;
        private TouchTracking.Forms.TouchEffect _touchEffect;

        protected override void OnAttached()
        {
            _view = Control == null ? Container : Control;

            // Get access to the TouchEffect class in the PCL
            _touchEffect =
                (TouchTracking.Forms.TouchEffect)Element.Effects.FirstOrDefault(e => e is TouchTracking.Forms.TouchEffect);

            if (_touchEffect == null)
            {
                return;
            }

            _touchHandler = new TouchHandler();
            _touchHandler.TouchAction += TouchHandlerOnTouch;
            _touchHandler.Capture = _touchEffect.Capture;
            _touchHandler.RegisterEvents(_view);

        }

        private void TouchHandlerOnTouch(object sender, TouchTracking.TouchActionEventArgs args)
        {
            _touchEffect.OnTouchAction(sender, args);
        }

        protected override void OnDetached()
        {
            if (_touchHandler == null)
            {
                return;
            }
            _touchHandler.TouchAction -= TouchHandlerOnTouch;
            _touchHandler.UnregisterEvents(_view);
        }
    }
}
