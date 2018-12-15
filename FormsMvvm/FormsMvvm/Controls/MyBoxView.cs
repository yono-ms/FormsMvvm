using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace FormsMvvm
{
    /// <summary>
    /// カスタムコントロールへのコマンド追加
    /// </summary>
    public class MyBoxView : BoxView
    {
        public MyBoxView()
        {
            UpdateColor();
            GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    // 何らかのタイミングで活性判定を行い実行する
                    if (Command.CanExecute(CommandParameter))
                    {
                        Command.Execute(CommandParameter);
                    }
                })
            });
        }

        private void UpdateColor()
        {
            Color = IsEnabled ? Color.Green : Color.Red;
        }

        public Command Command
        {
            get { return (Command)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create("Command", typeof(Command), typeof(MyBoxView));

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create("CommandParameter", typeof(object), typeof(MyBoxView),
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    Trace.WriteLine($"CommandParameterProperty propertyChanged old={oldValue} new={newValue}");

                    // プロパティを更新する
                    var view = bindable as MyBoxView;
                    view.IsEnabled = view.Command.CanExecute(newValue);
                    // 活性変化をフックしていなければここで見た目を更新する
                    view.UpdateColor();
                });
    }
}
