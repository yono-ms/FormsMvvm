# FormsMvvm

## 概要

XamarinFormsのMVVM実装を検討する。

- ライフサイクル考えなくていいかも
- ただしバックグラウンドサービス要素は除外してあくまでUIのみ
- 不揮発保存はXamarin.EssentialsのPreferences使えばいい
- デザインはEffectsだけでうまくいくかも
- それでもだめならカスタムコントロールをまじめに作る
- 最悪でもSkiaで何でも描ける
- 大量データのリストはRealmを使えるか
- リサイクラーみたいなメモリを食わない機構はどうする

## VM

xamlページを作成するとペアで作られるContentPageは、
コードビハインドであるが実はViewModelだったという話。
BindableObjectの派生であり、
INotificationPropertyChangedを持っている。
IsBusyのBindingというあたりで気づく。

### DataContext

しかしPageでもあるため自分をコンテキストにできない。
直下のコンテントのコンテキストに設定する。

    <ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:local="clr-namespace:FormsMvvm"
             x:Class="FormsMvvm.MainPage"
             x:Name="contentPage"
             Title="FormsMvvm">

        <Grid BindingContext="{x:Reference contentPage}">

XamarinFormsではBindingContextという名前となる。

### Property

普通のVMと同じくpropfullで作る。
OnPropertyChangedが用意されているので使う。

        private string _ApiKeyError;

        public string ApiKeyError
        {
            get { return _ApiKeyError; }
            set { _ApiKeyError = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanCommit)); }
        }

        public bool CanCommit => string.IsNullOrEmpty(ApiKeyError);

### Validation

ここは便利機能はないらしいので普通のVMと同じく作る。

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

## Binding

### View内部のバインド

VMを使用しない場合はSourceでx:Referenceできるが、
BindingContextを使うとBindingContextを上書きして使用する。
VMを使うなら完全にVM駆動方式にした方がわかりやすい。

### Convertor

コンバーターは作って使える。
複数ページにまたがる場合はApp.xamlにインスタンスを作る。

### BindableProperty

カスタムコントロールのプロパティは、
xamlで直値を設定もしくはコードで値設定する際には、
通常のpropfullで作っておけば良い。
PageでカスタムコントロールのプロパティにPageのプロパティを
バインドするケースでは、propdpみたいなForms方言を使用する。

## ライフサイクル

### 画面遷移時の挙動

Navigation.PushAsyncを使用する場合、
元のページは廃棄されない。
Backボタン時にはOnAppearingだけ呼ばれる。
ContentPageの実体はFragmentであるが、
画面遷移はreplaceにはなっていない。
PopAsyncされたときに廃棄される。
今までの経路をたどって戻るため、
スパゲッティ化するにはスタック管理が必要。

## アラート

DisplayAlertをawaitしてOK/CANCEL分岐できる。
コードビハインド兼VMの強み。
しかもOnPauseは考えなくてよい。

## ワーカースレッド

async/awaitを使用していればUIに戻る。
しかもOnPauseは考えなくてよい。

## Preferences

awaitしないのでコンストラクタでも使える。
ただしコンストラクタでGetするとデザイナが例外になる。
これはtry/catchしておけば回避できる。

## ContentPageコンストラクタ

デフォルトコンストラクタだけで動くようにしなければならない。
デザイナに怒られることで気づく。
前述のライフサイクルがらみで、
自動でメンバ変数を保存するような仕組みが
存在するかもしれない。
引数は基本不揮発で渡す。
揮発引数を使用したければ、
コンストラクタ後にプロパティをセットしてからPushAsyncする。

## カスタムコントロール

### Effect

プラットフォーム固有の描画を調整することができる。
Entryの場合iOS/UWPは枠線でAndroidは下線となっている。
全く別の形状を実現するにはカスタムコントロールを作る。
この場合、固有枠線や固有下線が消えない。
消すためにはプラットフォーム個別にEffectを作る。
枠線に合わせたい場合はAndroidだけEffectを作る。
Effectが存在しないiOSは無視されるため枠線表示のままとなる。

- 消す方向はEffect
- 描く方向はカスタムコントロール

と割り切るのも良いが、
Androidの下線を枠線に切り替える場合は、
Effectの中でコードで描画することもできる。
プラットフォームリソースでxmlを使用することも可能。

## タッチイベント

### 吊るしのイベント

#### GestureRecognizer Class

https://docs.microsoft.com/ja-jp/dotnet/api/xamarin.forms.gesturerecognizer?view=xamarin-forms

- Xamarin.Forms.ClickGestureRecognizer
- Xamarin.Forms.PanGestureRecognizer
- Xamarin.Forms.PinchGestureRecognizer
- Xamarin.Forms.SwipeGestureRecognizer
- Xamarin.Forms.TapGestureRecognizer

#### Press/Release

5種類のイベントがあるが、
よく見るとPress/Releaseが無い。
Button以外のコントロールにPress/Releaseが無い理由はこれか。

(追記)

SKCanvasViewのTouchイベントで全種類判定可能。

### Press/Releaseを検出する方法

#### 効果からのイベントの呼び出し

https://docs.microsoft.com/ja-jp/xamarin/xamarin-forms/app-fundamentals/effects/touch-tracking

#### 製造コスト

上記サンプルを見ると、
これは提供されるべき機能であって自分で作る機能ではない、
ということがわかる。
こんなものを書いているようではFormsを選択した意味がない。

### Press/Releaseの必要性とは

発端は押下時イメージを通常時イメージとは切り替えることだった。
よく考えてみるとイベントはClick(Tap)で良くてアニメーションさえ実現できれば問題ない。

### 吊るし以外のアニメーション

吊るしのアニメーションが用意されている。

https://docs.microsoft.com/ja-jp/xamarin/xamarin-forms/user-interface/animation/simple

汎用的なプロパティアニメーションを実現する方法は、
手作りということになるらしい。

https://docs.microsoft.com/ja-jp/xamarin/xamarin-forms/user-interface/graphics/skiasharp/basics/animation

おなじみのTaskDelayとInvalidateSurfaceで実現する。
カスタムコントロールのアニメ化するプロパティの
テンポラリ値を用意して描画させるとアニメーションになる。

## フルカスタムコントロール

skiaでcanvasにdrawする形式でコントロールを作れる。
SKCanvasを継承してcsだけで作るとタッチイベントも正常動作するらしい。
contentViewから作るよりも、
こっちの方が逆にコストがかからないような気がする。

### blur

UWPでおなじみのアクリルを実現する方法を考える。
検索するとsurface.Snapshot()の話題が見つかる。
しかしこれは自分のView内部のスナップしか取れない。
よって背景をぼかしつつカードの背景として利用するような目的には使えない。
つまり土台からすべてをcanvasに一筆書きするような作りでのみ有効な手法だった。
部品としてアクリルパネルを作る仕組みとして考えられる例は、
以下のようなものとなる。

1. ページの土台の背景を設定する
2. 同一背景をコントロールにも渡す
3. コントロール内で背景画像をクリップして自分の土台にする
4. その上に目的の描画を行う

これでいけるならSourceをバインドするだけで使える。
と思ったらとんでもない。

- パネルのOnPaintで初めてRectが決まる
- パネルが自分の土台のRectを知ることはない

つまり土台の背景に関するRectが手に入らない。
切り出せない。

- パネルのぼかし元絵柄は土台側が握っている
  - 土台がパネルのRectを判定して絵を切り出す
  - 土台からパネルに絵を渡す
- パネルが渡された絵を加工して背景設定

こうじゃないと動かない。
この構成では土台とパネルは一体のコントロールということになる。
やはり一筆書き方式となる。

さらに、
パネルの乗った状態でアラートを出すときに
iosみたいにアラート以外ぼかしたい場合はどうなるか。

- 背景をぼかす
- パネルをスナップしてぼかす

これをやらないといけない。
しかもパネルをスナップした時点で背景がぼけて、
みたいなダブルボケはどうなるのか計画しないといけない。

ここまででコストを使い果たしたので続きはまた次回。

