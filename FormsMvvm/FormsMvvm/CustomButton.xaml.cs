using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FormsMvvm
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CustomButton : ContentView
	{
        public bool IsPressed { get; set; }

        public CustomButton ()
		{
			InitializeComponent ();
		}

        private void SKCanvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            Trace.WriteLine($"{GetType().Name} SKCanvasView_PaintSurface");
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            var paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = IsPressed ? Color.Red.ToSKColor() : Color.Pink.ToSKColor(),
                StrokeWidth = 25
            };
            canvas.DrawCircle(info.Width / 2, info.Height / 2, info.Height / 2, paint);
        }

        private void SKCanvasView_Touch(object sender, SKTouchEventArgs e)
        {
            // Androidでは動作しない
            // UWPではPresseは発生するがReleaseが発生しない
            Trace.WriteLine($"{GetType().Name} SKCanvasView_Touch {e.ActionType}");
        }

        private void TouchEffect_TouchAction(object sender, TouchActionEventArgs args)
        {
            // 超高コストタッチイベント
            Trace.WriteLine($"{GetType().Name} TouchEffect_TouchAction {args.Type}");
            switch (args.Type)
            {
                case TouchActionType.Pressed:
                    IsPressed = true;
                    canvasView.InvalidateSurface();
                    break;

                case TouchActionType.Released:
                    IsPressed = false;
                    canvasView.InvalidateSurface();
                    break;

                default:
                    break;
            }
        }
    }
}