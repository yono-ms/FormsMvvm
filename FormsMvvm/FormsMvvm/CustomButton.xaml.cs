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
        public bool InProgress { get; set; }

        public float RadiusDefault { get; set; }
        public float Radius { get; set; }

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

            RadiusDefault = Math.Max(info.Width, info.Height) / 2;
            if (Radius == 0)
            {
                Radius = RadiusDefault;
            }

            canvas.Clear();

            using (var paint = new SKPaint())
            {
                paint.Shader = SKShader.CreateRadialGradient(
                                new SKPoint(info.Rect.MidX, info.Rect.MidY),
                                Radius,
                                new SKColor[] { SKColors.LightBlue, IsEnabled ? SKColors.CadetBlue : SKColors.LightGray },
                                null,
                                SKShaderTileMode.Clamp);
                canvas.DrawRect(info.Rect, paint);
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Trace.WriteLine($"{GetType().Name} TapGestureRecognizer_Tapped");

            if(IsEnabled)
            {
                StartAnimationAsync();
            }
            else
            {
                Trace.WriteLine($" ignore. enabled={IsEnabled}");
            }
        }

        private async void StartAnimationAsync()
        {
            Trace.WriteLine($"{GetType().Name} StartAnimationAsync");

            if (InProgress)
            {
                Trace.WriteLine($" is busy.");
            }

            try
            {
                InProgress = true;

                var quotient = RadiusDefault / 10;

                Radius = 0;
                while (Radius < (quotient * 10))
                {
                    Radius += quotient;
                    canvasView.InvalidateSurface();
                    await Task.Delay(TimeSpan.FromMilliseconds(20));
                    Trace.WriteLine($" wake up. {Radius}");
                }
                Radius = RadiusDefault;
                canvasView.InvalidateSurface();

                Clicked?.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"{GetType().Name} TapGestureRecognizer_Tapped : {ex.ToString()}");
            }
            finally
            {
                InProgress = false;
            }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(CustomButton), null);

        public event EventHandler Clicked;
    }
}