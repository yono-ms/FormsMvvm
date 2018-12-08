using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FormsMvvm
{
    /// <summary>
    /// 放射グラデーションボタン
    /// </summary>
    public class RadialButton : SKCanvasView
    {
        public bool InProgress { get; set; }

        public float RadiusDefault { get; set; }
        public float Radius { get; set; }

        private bool _IsPressed;

        public bool IsPressed
        {
            get { return _IsPressed; }
            set { _IsPressed = value; InvalidateSurface(); }
        }


        public RadialButton()
        {
            // 継承するとAndroidでもタッチイベントを使えるらしい
            EnableTouchEvents = true;
        }

        protected override void OnTouch(SKTouchEventArgs e)
        {
            switch (e.ActionType)
            {
                case SKTouchAction.Pressed:
                    Trace.WriteLine($"OnTouch {e.ActionType}");
                    IsPressed = true;
                    InvalidateSurface();
                    break;

                case SKTouchAction.Released:
                    Trace.WriteLine($"OnTouch {e.ActionType}");
                    if (IsPressed)
                    {
                        IsPressed = false;
                        StartAnimationAsync();
                    }
                    break;

                case SKTouchAction.Exited:
                    Trace.WriteLine($"OnTouch {e.ActionType}");
                    if (IsPressed)
                    {
                        IsPressed = false;
                        InvalidateSurface();
                    }
                    break;

                default:
                    break;
            }

            e.Handled = true;
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            Trace.WriteLine($"{GetType().Name} OnPaintSurface");
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            RadiusDefault = Math.Max(info.Width, info.Height) / 2;
            if (Radius == 0)
            {
                Radius = RadiusDefault;
            }

            // 押下状態では影を大きくする
            var seed = Math.Min(info.Rect.Width, info.Rect.Height);
            int bodyDelta = seed / 10;
            var bodyRect = new SKRectI(info.Rect.Left + bodyDelta, info.Rect.Top + bodyDelta, info.Rect.Right - bodyDelta, info.Rect.Bottom - bodyDelta);
            int shadowDelta = IsPressed ? 0 : seed / 20;
            var shadowRect = new SKRectI(info.Rect.Left + shadowDelta, info.Rect.Top + shadowDelta, info.Rect.Right - shadowDelta, info.Rect.Bottom - shadowDelta);
            var shadowColor = SKColors.DarkGray;
            var sunnyColor = SKColors.Transparent;

            canvas.Clear();

            // 上下の影
            using (var paint = new SKPaint())
            {
                float start = (float)(bodyRect.Top - shadowRect.Top) / shadowRect.Height;
                float end = 1 - start;
                var rect = new SKRectI(bodyRect.Left, shadowRect.Top, bodyRect.Right, shadowRect.Bottom);

                paint.Shader = SKShader.CreateLinearGradient(
                    new SKPoint(rect.MidX, rect.Top),
                    new SKPoint(rect.MidX, rect.Bottom),
                    new SKColor[] { sunnyColor, shadowColor, shadowColor, sunnyColor },
                    new float[] { 0, start, end, 1 },
                    SKShaderTileMode.Clamp);
                canvas.DrawRect(rect, paint);
            }

            // 左右の影
            using (var paint = new SKPaint())
            {
                float start = (float)(bodyRect.Left - shadowRect.Left) / shadowRect.Width;
                float end = 1 - start;
                var rect = new SKRectI(shadowRect.Left, bodyRect.Top, shadowRect.Right, bodyRect.Bottom);

                paint.Shader = SKShader.CreateLinearGradient(
                    new SKPoint(rect.Left, rect.MidY),
                    new SKPoint(rect.Right, rect.MidY),
                    new SKColor[] { sunnyColor, shadowColor, shadowColor, sunnyColor },
                    new float[] { 0, start, end, 1 },
                    SKShaderTileMode.Clamp);
                canvas.DrawRect(rect, paint);
            }

            // 四隅 左上
            using (var paint = new SKPaint())
            {
                var rect = new SKRectI(shadowRect.Left, shadowRect.Top, bodyRect.Left, bodyRect.Top);

                paint.Shader = SKShader.CreateRadialGradient(
                    new SKPoint(rect.Right, rect.Bottom),
                    rect.Width,
                    new SKColor[] { shadowColor, sunnyColor },
                    null,
                    SKShaderTileMode.Clamp);
                canvas.DrawRect(rect, paint);
            }
            // 四隅 右上
            using (var paint = new SKPaint())
            {
                var rect = new SKRectI(bodyRect.Right, shadowRect.Top, shadowRect.Right, bodyRect.Top);

                paint.Shader = SKShader.CreateRadialGradient(
                    new SKPoint(rect.Left, rect.Bottom),
                    rect.Width,
                    new SKColor[] { shadowColor, sunnyColor },
                    null,
                    SKShaderTileMode.Clamp);
                canvas.DrawRect(rect, paint);
            }
            // 四隅 左下
            using (var paint = new SKPaint())
            {
                var rect = new SKRectI(shadowRect.Left, bodyRect.Bottom, bodyRect.Left, shadowRect.Bottom);

                paint.Shader = SKShader.CreateRadialGradient(
                    new SKPoint(rect.Right, rect.Top),
                    rect.Width,
                    new SKColor[] { shadowColor, sunnyColor },
                    null,
                    SKShaderTileMode.Clamp);
                canvas.DrawRect(rect, paint);
            }
            // 四隅 右下
            using (var paint = new SKPaint())
            {
                var rect = new SKRectI(bodyRect.Right, bodyRect.Bottom, shadowRect.Right, shadowRect.Bottom);

                paint.Shader = SKShader.CreateRadialGradient(
                    new SKPoint(rect.Left, rect.Top),
                    rect.Width,
                    new SKColor[] { shadowColor, sunnyColor },
                    null,
                    SKShaderTileMode.Clamp);
                canvas.DrawRect(rect, paint);
            }

            // 本体
            using (var paint = new SKPaint())
            {
                // 非活性状態ではグレー
                var centerColor = IsEnabled ? CenterColor.ToSKColor() : SKColors.WhiteSmoke;
                var edgeColor = IsEnabled ? EdgeColor.ToSKColor() : SKColors.LightGray;

                // プラットフォームにより影のサイズが異なるため切れてしまう
                //var sigma = IsPressed ? 4 : 1;
                //paint.ImageFilter = SKImageFilter.CreateDropShadow(0, 0, sigma, sigma, SKColors.Black, SKDropShadowImageFilterShadowMode.DrawShadowAndForeground);

                paint.Shader = SKShader.CreateRadialGradient(
                    new SKPoint(info.Rect.MidX, info.Rect.MidY),
                    Radius,
                    new SKColor[] { centerColor, edgeColor },
                    null,
                    SKShaderTileMode.Clamp);
                canvas.DrawRect(bodyRect, paint);
            }

            // 文字
            using (var paint = new SKPaint())
            {
                paint.Color = IsEnabled ? SKColors.Black : SKColors.Gray;
                paint.Typeface = SKTypeface.FromFamilyName("Arial", SKTypefaceStyle.Bold);
                paint.IsAntialias = true;

                // 見やすくならない
                //paint.ImageFilter = SKImageFilter.CreateDropShadow(2, 2, 2, 2, SKColors.Black, SKDropShadowImageFilterShadowMode.DrawShadowAndForeground);

                var textBounds = new SKRect();
                paint.MeasureText(Text, ref textBounds);

                float height = textBounds.Height;
                paint.TextSize = 0.4f * bodyRect.Height * paint.TextSize / height;
                paint.MeasureText(Text, ref textBounds);

                float xText = bodyRect.Width / 2 - textBounds.MidX + bodyDelta;
                float yText = bodyRect.Height / 2 - textBounds.MidY + bodyDelta;

                canvas.DrawText(Text, xText, yText, paint);
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
                    InvalidateSurface();
                    await Task.Delay(TimeSpan.FromMilliseconds(20));
                    Trace.WriteLine($" wake up. {Radius}");
                }
                Radius = RadiusDefault;
                InvalidateSurface();

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
        public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(RadialButton), null);
        /// <summary>
        /// 中心の色
        /// </summary>
        public Color CenterColor
        {
            get { return (Color)GetValue(CenterColorProperty); }
            set { SetValue(CenterColorProperty, value); }
        }
        /// <summary>
        /// 中心の色
        /// </summary>
        public static readonly BindableProperty CenterColorProperty = BindableProperty.Create("CenterColor", typeof(Color), typeof(RadialButton), Color.White);
        /// <summary>
        /// 外側の色
        /// </summary>
        public Color EdgeColor
        {
            get { return (Color)GetValue(EdgeColorProperty); }
            set { SetValue(EdgeColorProperty, value); }
        }
        /// <summary>
        /// 外側の色
        /// </summary>
        public static readonly BindableProperty EdgeColorProperty = BindableProperty.Create("EdgeColor", typeof(Color), typeof(RadialButton), Color.Black);
    }
}
