using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FormsMvvm
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ListPage : ContentPage
	{
		public ListPage ()
		{
			InitializeComponent ();

            ApiKey = Preferences.Get(nameof(ApiKey), string.Empty);
		}

        protected override void OnAppearing()
        {
            Trace.WriteLine($"{GetType().Name} {MethodBase.GetCurrentMethod().Name}");

            InitAsync();
        }

        protected override void OnDisappearing()
        {
            Trace.WriteLine($"{GetType().Name} {MethodBase.GetCurrentMethod().Name}");
        }

        private async void InitAsync()
        {
            Trace.WriteLine($"{GetType().Name} Init");
            if (IsBusy)
            {
                Trace.WriteLine($"is busy.");
                return;
            }

            try
            {
                IsBusy = true;

                using (var httpClient = new HttpClient())
                {
                    var keyValuePairs = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("key", ApiKey),
                        new KeyValuePair<string, string>("format", "json"),
                        new KeyValuePair<string, string>("keyword", "ラーメン")
                    };
                    var urlString = $"{Constants.API_GOURMET}?{string.Join(@"&", keyValuePairs.Select(e => $"{e.Key}={e.Value}"))}";
                    Trace.WriteLine($"-- SEND -- {urlString}");
                    var httpResponseMessage = await httpClient.GetAsync(urlString);
                    Trace.WriteLine($"{httpResponseMessage.StatusCode}");
                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        var content = await httpResponseMessage.Content.ReadAsStringAsync();
                        Trace.WriteLine($"-- RECV -- {content}");
                        var apiResponseGourmet = JsonConvert.DeserializeObject<ApiResponseGourmet>(content);

                        if (apiResponseGourmet == null)
                        {
                            ErrorMessage = $"response is null.";
                            return;
                        }

                        var results = apiResponseGourmet.results;
                        if (results == null)
                        {
                            ErrorMessage = $"results is null.";
                            return;
                        }

                        var error = results.error;
                        if (error != null)
                        {
                            ErrorMessage = $"{error.message}({error.code})";
                        }

                        if (results.shop == null)
                        {
                            ErrorMessage = $"shop is null.";
                            return;
                        }

                        ResultsAvailable = results.results_available;
                        ResultsReturned = results.results_returned;
                        ResultsStart = results.results_start;

                        results.shop.ForEach(async e =>
                        {
                            Items.Add(e);
                            await Task.Delay(TimeSpan.FromMilliseconds(200));
                        });
                    }
                    else
                    {
                        ErrorMessage = $"{httpResponseMessage.StatusCode.ToString()}({(int)httpResponseMessage.StatusCode})";
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"{GetType().Name} Init : {ex.ToString()}");
                ErrorMessage = ex.ToString();
            }
            finally
            {
                IsBusy = false;
            }
        }

        public string ApiKey { get; set; }

        public ObservableCollection<ApiResponseGourmet.Shop> Items { get; } = new ObservableCollection<ApiResponseGourmet.Shop>();

        private string _ErrorMessage;

        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set { _ErrorMessage = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsError)); }
        }

        public bool IsError => !string.IsNullOrEmpty(ErrorMessage);

        private string _ResultsAvailable;

        public string ResultsAvailable
        {
            get { return _ResultsAvailable; }
            set { _ResultsAvailable = value; OnPropertyChanged(); }
        }

        private string _ResultsReturned;

        public string ResultsReturned
        {
            get { return _ResultsReturned; }
            set { _ResultsReturned = value; OnPropertyChanged(); }
        }

        private string _ResultsStart;

        public string ResultsStart
        {
            get { return _ResultsStart; }
            set { _ResultsStart = value; OnPropertyChanged(); }
        }
    }
}