using SkiaScene.TouchTracking;
using SkiaSharp;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace SkiaScene.UWP
{
    public class TouchHandler : TouchHandlerBase<FrameworkElement>
    {
        private FrameworkElement _frameworkElement;

        public override void RegisterEvents(FrameworkElement frameworkElement)
        {
            _frameworkElement = frameworkElement;
            if (_frameworkElement != null)
            {
                // Set event handlers on FrameworkElement
                _frameworkElement.PointerEntered += OnPointerEntered;
                _frameworkElement.PointerPressed += OnPointerPressed;
                _frameworkElement.PointerMoved += OnPointerMoved;
                _frameworkElement.PointerReleased += OnPointerReleased;
                _frameworkElement.PointerExited += OnPointerExited;
                _frameworkElement.PointerCanceled += OnPointerCancelled;
            }
        }
        

        public override void UnregisterEvents(FrameworkElement frameworkElement)
        {
            if (frameworkElement != null)
            {
                // Release event handlers on FrameworkElement
                frameworkElement.PointerEntered -= OnPointerEntered;
                frameworkElement.PointerPressed -= OnPointerPressed;
                frameworkElement.PointerMoved -= OnPointerMoved;
                frameworkElement.PointerReleased -= OnPointerReleased;
                frameworkElement.PointerExited -= OnPointerEntered;
                frameworkElement.PointerCanceled -= OnPointerCancelled;
            }
        }

        private void OnPointerEntered(object sender, PointerRoutedEventArgs args)
        {
            CommonHandler(sender, TouchActionType.Entered, args);
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs args)
        {
            CommonHandler(sender, TouchActionType.Pressed, args);

            // Check setting of Capture property
            if (Capture)
            {
                (sender as FrameworkElement).CapturePointer(args.Pointer);
            }
        }

        private void OnPointerMoved(object sender, PointerRoutedEventArgs args)
        {
            CommonHandler(sender, TouchActionType.Moved, args);
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs args)
        {
            CommonHandler(sender, TouchActionType.Released, args);
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs args)
        {
            CommonHandler(sender, TouchActionType.Exited, args);
        }

        private void OnPointerCancelled(object sender, PointerRoutedEventArgs args)
        {
            CommonHandler(sender, TouchActionType.Cancelled, args);
        }

        private void CommonHandler(object sender, TouchActionType touchActionType, PointerRoutedEventArgs args)
        {
            PointerPoint pointerPoint = args.GetCurrentPoint(sender as UIElement);
            Windows.Foundation.Point windowsPoint = pointerPoint.Position;

            OnTouchAction(_frameworkElement, new TouchActionEventArgs(args.Pointer.PointerId,
                                                            touchActionType,
                                                            new SKPoint((float)windowsPoint.X, (float)windowsPoint.Y),
                                                            args.Pointer.IsInContact));
        }
    }
}
