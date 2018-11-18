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

            try
            {
                ApiKey = Preferences.Get(nameof(ApiKey), string.Empty);
                Keyword = Preferences.Get(nameof(Keyword), @"横浜");
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"{GetType()} ctor {ex.ToString()}");
            }
        }

        protected override void OnAppearing()
        {
            Trace.WriteLine($"{GetType().Name} {MethodBase.GetCurrentMethod().Name}");

            KickGetApi();
        }

        protected override void OnDisappearing()
        {
            Trace.WriteLine($"{GetType().Name} {MethodBase.GetCurrentMethod().Name}");
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Trace.WriteLine($"{GetType().Name} Button_Clicked");

            KickGetApi();
        }

        private async void ListView_RefreshingAsync(object sender, EventArgs e)
        {
            Trace.WriteLine($"{GetType().Name} ListView_RefreshingAsync");

            if (IsBusy)
            {
                Trace.WriteLine($"is busy.");
                return;
            }

            try
            {
                IsBusy = true;
                IsRefreshing = true;

                ClearItems();

                await GetApi();

            }
            catch (Exception ex)
            {
                Trace.WriteLine($"{GetType().Name} ListView_RefreshingAsync : {ex.ToString()}");
                ErrorMessage = ex.ToString();
            }
            finally
            {
                IsRefreshing = false;
                IsBusy = false;
            }
        }

        /// <summary>
        /// BUSY管理してGetAPIを実行する
        /// </summary>
        private async void KickGetApi()
        {
            Trace.WriteLine($"{GetType().Name} KickGetApi");
            if (IsBusy)
            {
                Trace.WriteLine($"is busy.");
                return;
            }

            try
            {
                IsBusy = true;

                await GetApi();
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"{GetType().Name} KickGetApi : {ex.ToString()}");
                ErrorMessage = ex.ToString();
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// 更新必要か判定してAPIを実行する
        /// </summary>
        /// <returns></returns>
        private async Task GetApi()
        {
            Trace.WriteLine($"{GetType().Name} GetApi");

            if (IsReceivedAll())
            {
                var update = await DisplayAlert(string.Empty, "全データ取得済みです。", "更新する", "OK");
                if (update)
                {
                    ClearItems();
                }
                else
                {
                    return;
                }
            }

            var keyValuePairs = GetDefaultParameters();

            var urlString = BuildUrlString(Constants.API_GOURMET, keyValuePairs);

            using (var httpClient = new HttpClient())
            {
                Trace.WriteLine($"-- SEND -- {urlString}");
                var httpResponseMessage = await httpClient.GetAsync(urlString);
                Trace.WriteLine($"{httpResponseMessage.StatusCode}");
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var content = await httpResponseMessage.Content.ReadAsStringAsync();
                    Trace.WriteLine($"-- RECV -- {content}");
                    var apiResponseGourmet = JsonConvert.DeserializeObject<ApiResponseGourmet>(content);

                    var result = IsResponseValid(apiResponseGourmet);
                    if (result.isValidResponse)
                    {
                        var results = apiResponseGourmet.results;
                        ResultsAvailable = results.results_available;
                        ResultsReturned = results.results_returned;
                        ResultsStart = results.results_start;

                        foreach (var item in results.shop)
                        {
                            Items.Add(item);

                            // 画像ロードと分散させるためスレッドを止めながら追加する
                            await Task.Delay(TimeSpan.FromMilliseconds(200));
                        }
                    }
                    else
                    {
                        ErrorMessage = result.errorMessage;
                    }
                }
                else
                {
                    var statusCode = httpResponseMessage.StatusCode;
                    ErrorMessage = $"{statusCode.ToString()}({(int)statusCode})";
                }
            }
        }

        /// <summary>
        /// 取得済みアイテムを全削除する
        /// </summary>
        private void ClearItems()
        {
            ResultsAvailable = string.Empty;
            ResultsReturned = string.Empty;
            ResultsStart = string.Empty;

            Items.Clear();
        }

        /// <summary>
        /// 全件取得しているかチェック
        /// </summary>
        /// <returns>取得済みは真</returns>
        private bool IsReceivedAll()
        {
            if (string.IsNullOrEmpty(ResultsAvailable))
            {
                Trace.WriteLine($"{GetType().Name} IsReceivedAll 一度も受信していない");
                return false;
            }

            var available = int.Parse(ResultsAvailable);
            if (available == 0)
            {
                Trace.WriteLine($"{GetType().Name} IsReceivedAll 検索結果が0件");
                return false;
            }

            if (available > Items.Count)
            {
                return false;
            }
            else
            {
                Trace.WriteLine($"{GetType().Name} IsReceivedAll 全件受信完了 {Items.Count} / {available}");
                return true;
            }
        }

        /// <summary>
        /// URL文字列を組み立てる
        /// </summary>
        /// <param name="apiUrlBase">APIのURL</param>
        /// <param name="keyValuePairs">パラメータ</param>
        /// <returns>URL文字列</returns>
        private string BuildUrlString(string apiUrlBase, Dictionary<string, string> keyValuePairs)
        {
            var parameters = keyValuePairs.Select(e => $"{e.Key}={e.Value}");
            var parametersString = string.Join(@"&", parameters);
            return $"{apiUrlBase}?{parametersString}";
        }

        /// <summary>
        /// デフォルトパラメータを取得する
        /// </summary>
        /// <returns>パラメータ</returns>
        private Dictionary<string, string> GetDefaultParameters()
        {
            return new Dictionary<string, string>
            {
                { "key", ApiKey },
                { "keyword", Keyword},
                { "format", "json" },
                { "lat", "35.4657858" },
                { "lng", "139.6201245," },
                { "start", (Items.Count + 1).ToString() },
                { "count", "10" }
            };
        }

        /// <summary>
        /// レスポンスバリデーションチェック
        /// </summary>
        /// <param name="apiResponseGourmet">レスポンス</param>
        /// <returns>正常の場合は真</returns>
        (bool isValidResponse, string errorMessage) IsResponseValid(ApiResponseGourmet apiResponseGourmet)
        {
            if (apiResponseGourmet == null)
            {
                return (false, $"response is null.");
            }

            var results = apiResponseGourmet.results;
            if (results == null)
            {
                return (false, $"results is null.");
            }

            var error = results.error;
            if (error != null)
            {
                return (false, $"{error.message}({error.code})");
            }

            if (results.shop == null)
            {
                return (false, $"shop is null.");
            }

            return (true, string.Empty);
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

        private string _Keyword;

        public string Keyword
        {
            get { return _Keyword; }
            set { _Keyword = value; }
        }

        private bool _IsRefreshing;

        public bool IsRefreshing
        {
            get { return _IsRefreshing; }
            set { _IsRefreshing = value; OnPropertyChanged(); }
        }

    }
}