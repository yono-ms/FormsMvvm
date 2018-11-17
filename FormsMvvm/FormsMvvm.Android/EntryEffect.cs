using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly:ResolutionGroupName("FormsMvvm")]
[assembly:ExportEffect(typeof(FormsMvvm.Droid.EntryEffect), "EntryEffect")]
namespace FormsMvvm.Droid
{
    public class EntryEffect : PlatformEffect
    {
        Drawable background;

        protected override void OnAttached()
        {
            var editText = Control as EditText;
            if (editText == null)
            {
                return;
            }

            background = editText.Background;
            var gradientDrawable = new GradientDrawable();
            gradientDrawable.SetColor(0);
            gradientDrawable.SetStroke(2, Android.Graphics.Color.Gray);
            gradientDrawable.SetCornerRadius(4);
            editText.SetBackground(gradientDrawable);
        }

        protected override void OnDetached()
        {
            var editText = Control as EditText;
            if (editText == null)
            {
                return;
            }

            editText.SetBackground(background);
        }
    }
}