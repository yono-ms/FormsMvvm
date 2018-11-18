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

            // 復元用
            background = editText.Background;

#if ENTRY_COLOR_WIDTH

            // 色と太さを変えるバージョン
            var gradientDrawableFocused = new GradientDrawable();
            gradientDrawableFocused.SetColor(0);
            gradientDrawableFocused.SetStroke(8, Android.Graphics.Color.Green);
            gradientDrawableFocused.SetCornerRadius(16);

            var gradientDrawableDefault = new GradientDrawable();
            gradientDrawableDefault.SetColor(0);
            gradientDrawableDefault.SetStroke(2, Android.Graphics.Color.Gray);
            gradientDrawableDefault.SetCornerRadius(4);

            var stateListDrawable = new StateListDrawable();
            stateListDrawable.AddState(new int[] { Android.Resource.Attribute.StateFocused }, gradientDrawableFocused);
            stateListDrawable.AddState(Android.Util.StateSet.WildCard.ToArray(), gradientDrawableDefault);
            editText.SetBackground(stateListDrawable);

#else

            // 色だけ変えるバージョン
            var colors = new int[] { Android.Graphics.Color.Blue, Android.Graphics.Color.Gray };
            var states = new int[][] { new int[] { Android.Resource.Attribute.StateFocused }, Android.Util.StateSet.WildCard.ToArray() };
            var colorStateList = new ColorStateList(states, colors);

            var gradientDrawable = new GradientDrawable();
            gradientDrawable.SetColor(0);
            gradientDrawable.SetStroke(2, colorStateList);
            gradientDrawable.SetCornerRadius(4);
            editText.SetBackground(gradientDrawable);

#endif
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