using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FormsMvvm
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            try
            {
                ApiKey = Preferences.Get(nameof(ApiKey), string.Empty);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"{GetType()} ctor {ex.ToString()}");
            }

            MyBoxCommand4.CanExecuteChanged += (s, e) =>
            {
                // ここは永久に発生しない
                Trace.WriteLine($"{GetType().Name} CanExecuteChanged");
            };
        }

        protected override void OnAppearing()
        {
            Trace.WriteLine($"{GetType().Name} {MethodBase.GetCurrentMethod().Name}");
        }

        protected override void OnDisappearing()
        {
            Trace.WriteLine($"{GetType().Name} {MethodBase.GetCurrentMethod().Name}");
        }

        private string GetPropertyError(object value, [CallerMemberName] string propertyName = null)
        {
            var context = new ValidationContext(this) { MemberName = propertyName };
            var result = new List<ValidationResult>();
            var isValid = Validator.TryValidateProperty(value, context, result);
            return isValid ? string.Empty : string.Join("\n", result.Select(e => e.ErrorMessage));
        }

        private string _ApiKey;
        [Required]
        [RegularExpression(@"[0-9a-z]+")]
        [MaxLength(16)]
        public string ApiKey
        {
            get { return _ApiKey; }
            set
            {
                _ApiKey = value;
                OnPropertyChanged();
                ApiKeyError = GetPropertyError(value);
            }
        }
        private string _ApiKeyError;

        public string ApiKeyError
        {
            get { return _ApiKeyError; }
            set
            {
                _ApiKeyError = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanCommit));
                TestCommand.ChangeCanExecute();
                // 123は意味がない
                //MyBoxCommand1.ChangeCanExecute();
                //MyBoxCommand2.ChangeCanExecute();
                //MyBoxCommand3.ChangeCanExecute();
                // 4だけ意味を持つが実際には不要
                // CanExecuteChangedは使わずCommandParameterのchangedを使っているらしい
                //MyBoxCommand4.ChangeCanExecute();
            }
        }

        public bool CanCommit => string.IsNullOrEmpty(ApiKeyError);

        public string CommitLabel { get; } = "START";
        public string Description { get; } = "サーバーを利用するにはAPIキーが必要です。";
        public string ApiKeyPlaceholder { get; } = "APIキー";

        private async void Button_ClickedAsync(object sender, EventArgs e)
        {
            Preferences.Set(nameof(ApiKey), ApiKey);
            await Navigation.PushAsync(new ListPage(), true);
        }

        public ImageSource Source { get; } = ImageSource.FromResource("FormsMvvm.img1.jpg");

        public Command TestCommand => new Command((arg) =>
        {
            Button_ClickedAsync(this, new EventArgs());
        }, (arg) =>
        {
            var errors = arg as string;
            return string.IsNullOrEmpty(errors);
        });

        public Command MyBoxCommand1 => new Command(() =>
        {
            // 常に活性単品ボタン
            Trace.WriteLine($"{GetType().Name} MyBoxCommand1 Execute");
        });

        public Command MyBoxCommand2 => new Command((arg) =>
        {
            // 常に活性ボタンのボタン区別
            Trace.WriteLine($"{GetType().Name} MyBoxCommand2 Execute {arg}");
        });

        public Command MyBoxCommand3 => new Command(() =>
        {
            Trace.WriteLine($"{GetType().Name} MyBoxCommand3 Execute");
        }, ()=>
        {
            // CommandParameterを指定しない場合ここは永久に来ない
            // つまり使えない
            Trace.WriteLine($"{GetType().Name} MyBoxCommand3 CanExecute");
            return CanCommit;
        });

        public Command MyBoxCommand4 => new Command((arg) =>
        {
            Trace.WriteLine($"{GetType().Name} MyBoxCommand4 Execute {arg}");
        }, (arg) =>
        {
            // CommandParameterが固定値の場合一度だけ発生する
            // CommandParameterがCanExecuteに関連するバインドの場合は都度発生する
            Trace.WriteLine($"{GetType().Name} MyBoxCommand4 CanExecute {arg}");
            // argを無視しても良い
            return CanCommit;
        });
    }
}
