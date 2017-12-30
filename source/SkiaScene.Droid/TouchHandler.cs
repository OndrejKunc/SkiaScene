using Android.Views;
using System;
using System.Collections.Generic;

namespace TouchTracking.Droid
{
    public class TouchHandler : TouchHandlerBase<View>
    {
        View _view;
        bool _capture;
        Func<double, double> _fromPixels;
        int[] _twoIntArray = new int[2];

        static Dictionary<View, TouchHandler> _viewDictionary =
            new Dictionary<View, TouchHandler>();

        static Dictionary<int, TouchHandler> _idToTouchHandlerDictionary =
            new Dictionary<int, TouchHandler>();

        public override void RegisterEvents(View view)
        {
            _view = view;
            if (view != null)
            {
                _viewDictionary.Add(view, this);

                _fromPixels = view.Context.FromPixels;

                // Set event handler on View
                view.Touch += OnTouch;
            }
        }

        public override void UnregisterEvents(View view)
        {
            if (_viewDictionary.ContainsKey(view))
            {
                _viewDictionary.Remove(view);
                view.Touch -= OnTouch;
            }
        }

        private void OnTouch(object sender, View.TouchEventArgs args)
        {
            // Two object common to all the events
            View senderView = sender as View;
            MotionEvent motionEvent = args.Event;

            // Get the pointer index
            int pointerIndex = motionEvent.ActionIndex;

            // Get the id that identifies a finger over the course of its progress
            int id = motionEvent.GetPointerId(pointerIndex);


            senderView.GetLocationOnScreen(_twoIntArray);
            TouchTrackingPoint screenPointerCoords = new TouchTrackingPoint(_twoIntArray[0] + motionEvent.GetX(pointerIndex),
                                                  _twoIntArray[1] + motionEvent.GetY(pointerIndex));


            // Use ActionMasked here rather than Action to reduce the number of possibilities
            switch (args.Event.ActionMasked)
            {
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:
                    FireEvent(this, id, TouchActionType.Pressed, screenPointerCoords, true);

                    _idToTouchHandlerDictionary.Add(id, this);

                    _capture = Capture;
                    break;

                case MotionEventActions.Move:
                    // Multiple Move events are bundled, so handle them in a loop
                    for (pointerIndex = 0; pointerIndex < motionEvent.PointerCount; pointerIndex++)
                    {
                        id = motionEvent.GetPointerId(pointerIndex);

                        if (_capture)
                        {
                            senderView.GetLocationOnScreen(_twoIntArray);

                            screenPointerCoords = new TouchTrackingPoint(_twoIntArray[0] + motionEvent.GetX(pointerIndex),
                                                            _twoIntArray[1] + motionEvent.GetY(pointerIndex));

                            FireEvent(this, id, TouchActionType.Moved, screenPointerCoords, true);
                        }
                        else
                        {
                            CheckForBoundaryHop(id, screenPointerCoords);

                            if (_idToTouchHandlerDictionary[id] != null)
                            {
                                FireEvent(_idToTouchHandlerDictionary[id], id, TouchActionType.Moved, screenPointerCoords, true);
                            }
                        }
                    }
                    break;

                case MotionEventActions.Up:
                case MotionEventActions.Pointer1Up:
                    if (_capture)
                    {
                        FireEvent(this, id, TouchActionType.Released, screenPointerCoords, false);
                    }
                    else
                    {
                        CheckForBoundaryHop(id, screenPointerCoords);

                        if (_idToTouchHandlerDictionary[id] != null)
                        {
                            FireEvent(_idToTouchHandlerDictionary[id], id, TouchActionType.Released, screenPointerCoords, false);
                        }
                    }
                    _idToTouchHandlerDictionary.Remove(id);
                    break;

                case MotionEventActions.Cancel:
                    if (_capture)
                    {
                        FireEvent(this, id, TouchActionType.Cancelled, screenPointerCoords, false);
                    }
                    else
                    {
                        if (_idToTouchHandlerDictionary[id] != null)
                        {
                            FireEvent(_idToTouchHandlerDictionary[id], id, TouchActionType.Cancelled, screenPointerCoords, false);
                        }
                    }
                    _idToTouchHandlerDictionary.Remove(id);
                    break;
            }
        }

        private void CheckForBoundaryHop(int id, TouchTrackingPoint pointerLocation)
        {
            TouchHandler touchEffectHit = null;

            foreach (View view in _viewDictionary.Keys)
            {
                // Get the view rectangle
                try
                {
                    view.GetLocationOnScreen(_twoIntArray);
                }
                catch // System.ObjectDisposedException: Cannot access a disposed object.
                {
                    continue;
                }
                TouchTrackingRect viewRect = new TouchTrackingRect(_twoIntArray[0], _twoIntArray[1], view.Width, view.Height);

                if (viewRect.Contains(pointerLocation))
                {
                    touchEffectHit = _viewDictionary[view];
                }
            }

            if (touchEffectHit != _idToTouchHandlerDictionary[id])
            {
                if (_idToTouchHandlerDictionary[id] != null)
                {
                    FireEvent(_idToTouchHandlerDictionary[id], id, TouchActionType.Exited, pointerLocation, true);
                }
                if (touchEffectHit != null)
                {
                    FireEvent(touchEffectHit, id, TouchActionType.Entered, pointerLocation, true);
                }
                _idToTouchHandlerDictionary[id] = touchEffectHit;
            }
        }

        private void FireEvent(TouchHandler touchEffect, int id, TouchActionType actionType, TouchTrackingPoint pointerLocation, bool isInContact)
        {
            // Get the location of the pointer within the view
            touchEffect._view.GetLocationOnScreen(_twoIntArray);
            double x = pointerLocation.X - _twoIntArray[0];
            double y = pointerLocation.Y - _twoIntArray[1];
            TouchTrackingPoint point = new TouchTrackingPoint((float)_fromPixels(x), (float)_fromPixels(y));

            // Call the method
            OnTouchAction(touchEffect._view,
                new TouchActionEventArgs(id, actionType, point, isInContact));
        }
    }
}
