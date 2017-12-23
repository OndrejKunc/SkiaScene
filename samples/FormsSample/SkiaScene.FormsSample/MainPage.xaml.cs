using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SkiaScene.FormsSample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void OnPan(object sender, PanUpdatedEventArgs e)
        {
            var scale = canvas.CanvasSize.Width / (float)canvas.Width;

        }

        private void OnPinch(object sender, PinchGestureUpdatedEventArgs e)
        {

        }

        private void OnTap(object sender, EventArgs e)
        {
            
        }

        private void OnPaint(object sender, SKPaintSurfaceEventArgs e)
        {

        }

    }
}
