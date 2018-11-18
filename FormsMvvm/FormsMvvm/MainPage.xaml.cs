using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
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
            set { _ApiKeyError = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanCommit)); }
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
    }
}
