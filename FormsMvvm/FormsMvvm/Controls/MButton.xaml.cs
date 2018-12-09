using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FormsMvvm
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MButton : ContentView
	{
        public bool IsTrace { get; private set; } = true;

        private bool _IsPressed;

        public bool IsPressed
        {
            get { return _IsPressed; }
            set { _IsPressed = value; }
        }

        public MButton ()
		{
			InitializeComponent ();
		}

        private void SKCanvasView_Touch(object sender, SKTouchEventArgs e)
        {
            var view = sender as SKCanvasView;

            switch (e.ActionType)
            {
                case SKTouchAction.Pressed:
                    IsPressed = true;
                    view.InvalidateSurface();
                    break;

                case SKTouchAction.Released:
                    if (IsPressed)
                    {
                        Clicked?.Invoke(this, new EventArgs());
                    }
                    IsPressed = false;
                    view.InvalidateSurface();
                    break;

                case SKTouchAction.Cancelled:
                case SKTouchAction.Exited:
                    if (IsPressed)
                    {
                        IsPressed = false;
                        view.InvalidateSurface();
                    }
                    break;

                default:
                    Trace.WriteLineIf(IsTrace, $"{GetType().Name} {MethodBase.GetCurrentMethod().Name} {e.ActionType}");
                    break;
            }

            e.Handled = true;
        }

        private void SKCanvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            Trace.WriteLineIf(IsTrace, $"{GetType().Name} {MethodBase.GetCurrentMethod().Name} {e.Info.Width} {e.Info.Height}");
            var rect = e.Info.Rect;
            var surface = e.Surface;
            var canvas = surface.Canvas;

            canvas.Clear();
            using (var paint = new SKPaint()
            {
                Color = IsPressed ? SKColors.MediumVioletRed : SKColors.AliceBlue
            })
            {
                canvas.DrawRect(rect, paint);
            }
        }
        /// <summary>
        /// ボタン押下イベント
        /// </summary>
        public event EventHandler Clicked;
        /// <summary>
        /// ボタンテキスト
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        /// <summary>
        /// ボタンテキスト
        /// </summary>
        public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(MButton), null);
    }
}