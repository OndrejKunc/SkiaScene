using TouchTracking;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace SkiaScene.TouchManipulation
{
    public class TouchManipulationRenderer : ITouchManipulationRenderer
    {
        private const int DefaultFramesPerSecond = 30;

        private readonly ITouchManipulationManager _touchManipulationManager;
        private readonly Action _invalidateViewAction;
        private TimeSpan _minGestureDuration = TimeSpan.FromMilliseconds(60);
        private Dictionary<long, DateTime> _lastGestureTime = new Dictionary<long, DateTime>();
        private List<long> _touchIds = new List<long>();
        private int _maxFramesPerSecond;

        public TouchManipulationRenderer(ITouchManipulationManager touchManipulationManager, Action invalidateViewAction)
        {
            _touchManipulationManager = touchManipulationManager;
            _invalidateViewAction = invalidateViewAction;
            MaxFramesPerSecond = DefaultFramesPerSecond;
        }

        public int MaxFramesPerSecond
        {
            get { return _maxFramesPerSecond; }
            set
            {
                _maxFramesPerSecond = value;
                _minGestureDuration = TimeSpan.FromMilliseconds(1000d / _maxFramesPerSecond);

            }
        }

        public void Render(SKPoint canvasPoint, TouchActionType touchActionType, long touchId)
        {
            switch (touchActionType)
            {
                case TouchActionType.Pressed:
                    _touchIds.Add(touchId);
                    _touchManipulationManager.ProcessTouchEvent(touchId, touchActionType, canvasPoint);
                    _lastGestureTime[touchId] = DateTime.Now;
                    break;
                case TouchActionType.Moved:
                    if (_touchIds.Contains(touchId))
                    {
                        _touchManipulationManager.ProcessTouchEvent(touchId, touchActionType, canvasPoint);
                        var now = DateTime.Now;
                        if (now - _lastGestureTime[touchId] < _minGestureDuration)
                        {
                            return;
                        }
                        _lastGestureTime[touchId] = now;
                        _invalidateViewAction();
                    }
                    break;

                case TouchActionType.Released:
                case TouchActionType.Cancelled:
                    if (_touchIds.Contains(touchId))
                    {
                        _lastGestureTime[touchId] = DateTime.MinValue;
                        _touchManipulationManager.ProcessTouchEvent(touchId, touchActionType, canvasPoint);
                        _touchIds.Remove(touchId);
                        _invalidateViewAction();
                    }
                    break;
            }
        }
    }
}
