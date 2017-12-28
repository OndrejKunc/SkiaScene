using SkiaSharp;
using System;

namespace SkiaScene.TouchTracking
{
    public class TouchActionEventArgs : EventArgs
    {
        public TouchActionEventArgs(long id, TouchActionType type, SKPoint location, bool isInContact)
        {
            Id = id;
            Type = type;
            Location = location;
            IsInContact = isInContact;
        }

        public long Id { private set; get; }

        public TouchActionType Type { private set; get; }

        public SKPoint Location { private set; get; }

        public bool IsInContact { private set; get; }
    }
}
