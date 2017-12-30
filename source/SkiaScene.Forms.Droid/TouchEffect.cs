using System.Linq;
using TouchTracking.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("TouchTracking")]
[assembly: ExportEffect(typeof(TouchTracking.Forms.Droid.TouchEffect), "TouchEffect")]
namespace TouchTracking.Forms.Droid
{
    public class TouchEffect : PlatformEffect
    {
        private TouchHandler _touchHandler;
        private Android.Views.View _view;
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

        private void TouchHandlerOnTouch(object sender, TouchActionEventArgs args)
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
