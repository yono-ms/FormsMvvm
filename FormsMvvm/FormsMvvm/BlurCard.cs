using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FormsMvvm
{
    /// <summary>
    /// ブラーカード
    /// </summary>
    class BlurCard : SKCanvasView
    {
        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            Trace.WriteLine($"{GetType().Name} OnPaintSurface");
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            using (var snap = surface.Snapshot())
            {
                using (var paint = new SKPaint())
                {
                    Debug.WriteLine($"Source={Source.ToString()}");
                    if(Source is StreamImageSource)
                    {
                        // 一部だけ切り出そうにも土台のサイズがわからない
                        StreamImageSource streamImageSource = (StreamImageSource)Source;
                        CancellationToken cancellationToken = CancellationToken.None;
                        Task<Stream> task = streamImageSource.Stream(cancellationToken);
                        Stream stream = task.Result;
                        var sKBitmap = SKBitmap.Decode(stream);
                        paint.ImageFilter = SKImageFilter.CreateBlur(1, 1);
                        canvas.DrawBitmap(sKBitmap, info.Rect, paint);
                    }
                }
            }
        }

        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly BindableProperty SourceProperty = BindableProperty.Create("Source", typeof(ImageSource), typeof(BlurCard), null);

        public double Padding
        {
            get { return (double)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        public static readonly BindableProperty PaddingProperty = BindableProperty.Create("Padding", typeof(double), typeof(BlurCard), 10d);
    }
}
