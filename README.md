# FormsMvvm

## �T�v

XamarinForms��MVVM��������������B

- ���C�t�T�C�N���l���Ȃ��Ă�������
- �������o�b�N�O���E���h�T�[�r�X�v�f�͏��O���Ă����܂�UI�̂�
- �s�����ۑ���Xamarin.Essentials��Preferences�g���΂���
- �f�U�C����Effects�����ł��܂���������
- ����ł����߂Ȃ�J�X�^���R���g���[�����܂��߂ɍ��
- �ň��ł�Skia�ŉ��ł��`����
- ��ʃf�[�^�̃��X�g��Realm���g���邩
- ���T�C�N���[�݂����ȃ�������H��Ȃ��@�\�͂ǂ�����

## VM

xaml�y�[�W���쐬����ƃy�A�ō����ContentPage�́A
�R�[�h�r�n�C���h�ł��邪����ViewModel�������Ƃ����b�B
BindableObject�̔h���ł���A
INotificationPropertyChanged�������Ă���B
IsBusy��Binding�Ƃ���������ŋC�Â��B

### DataContext

������Page�ł����邽�ߎ������R���e�L�X�g�ɂł��Ȃ��B
�����̃R���e���g�̃R���e�L�X�g�ɐݒ肷��B

    <ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:local="clr-namespace:FormsMvvm"
             x:Class="FormsMvvm.MainPage"
             x:Name="contentPage"
             Title="FormsMvvm">

        <Grid BindingContext="{x:Reference contentPage}">

XamarinForms�ł�BindingContext�Ƃ������O�ƂȂ�B

### Property

���ʂ�VM�Ɠ�����propfull�ō��B
OnPropertyChanged���p�ӂ���Ă���̂Ŏg���B

        private string _ApiKeyError;

        public string ApiKeyError
        {
            get { return _ApiKeyError; }
            set { _ApiKeyError = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanCommit)); }
        }

        public bool CanCommit => string.IsNullOrEmpty(ApiKeyError);

### Validation

�����͕֗��@�\�͂Ȃ��炵���̂ŕ��ʂ�VM�Ɠ��������B

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

### View�����̃o�C���h

VM���g�p���Ȃ��ꍇ��Source��x:Reference�ł��邪�A
BindingContext���g����BindingContext���㏑�����Ďg�p����B
VM���g���Ȃ犮�S��VM�쓮�����ɂ��������킩��₷���B

### Convertor

�R���o�[�^�[�͍���Ďg����B
�����y�[�W�ɂ܂�����ꍇ��App.xaml�ɃC���X�^���X�����B

### BindableProperty

�J�X�^���R���g���[���̃v���p�e�B�́A
xaml�Œ��l��ݒ�������̓R�[�h�Œl�ݒ肷��ۂɂ́A
�ʏ��propfull�ō���Ă����Ηǂ��B
Page�ŃJ�X�^���R���g���[���̃v���p�e�B��Page�̃v���p�e�B��
�o�C���h����P�[�X�ł́Apropdp�݂�����Forms�������g�p����B

## ���C�t�T�C�N��

### ��ʑJ�ڎ��̋���

Navigation.PushAsync���g�p����ꍇ�A
���̃y�[�W�͔p������Ȃ��B
Back�{�^�����ɂ�OnAppearing�����Ă΂��B
ContentPage�̎��̂�Fragment�ł��邪�A
��ʑJ�ڂ�replace�ɂ͂Ȃ��Ă��Ȃ��B
PopAsync���ꂽ�Ƃ��ɔp�������B
���܂ł̌o�H�����ǂ��Ė߂邽�߁A
�X�p�Q�b�e�B������ɂ̓X�^�b�N�Ǘ����K�v�B

## �A���[�g

DisplayAlert��await����OK/CANCEL����ł���B
�R�[�h�r�n�C���h��VM�̋��݁B
������OnPause�͍l���Ȃ��Ă悢�B

## ���[�J�[�X���b�h

async/await���g�p���Ă����UI�ɖ߂�B
������OnPause�͍l���Ȃ��Ă悢�B

## Preferences

await���Ȃ��̂ŃR���X�g���N�^�ł��g����B
�������R���X�g���N�^��Get����ƃf�U�C�i����O�ɂȂ�B
�����try/catch���Ă����Ή���ł���B

## ContentPage�R���X�g���N�^

�f�t�H���g�R���X�g���N�^�����œ����悤�ɂ��Ȃ���΂Ȃ�Ȃ��B
�f�U�C�i�ɓ{���邱�ƂŋC�Â��B
�O�q�̃��C�t�T�C�N������݂ŁA
�����Ń����o�ϐ���ۑ�����悤�Ȏd�g�݂�
���݂��邩������Ȃ��B
�����͊�{�s�����œn���B
�����������g�p��������΁A
�R���X�g���N�^��Ƀv���p�e�B���Z�b�g���Ă���PushAsync����B

## �J�X�^���R���g���[��

### Effect

�v���b�g�t�H�[���ŗL�̕`��𒲐����邱�Ƃ��ł���B
Entry�̏ꍇiOS/UWP�͘g����Android�͉����ƂȂ��Ă���B
�S���ʂ̌`�����������ɂ̓J�X�^���R���g���[�������B
���̏ꍇ�A�ŗL�g����ŗL�����������Ȃ��B
�������߂ɂ̓v���b�g�t�H�[���ʂ�Effect�����B
�g���ɍ��킹�����ꍇ��Android����Effect�����B
Effect�����݂��Ȃ�iOS�͖�������邽�ߘg���\���̂܂܂ƂȂ�B

- ����������Effect
- �`�������̓J�X�^���R���g���[��

�Ɗ���؂�̂��ǂ����A
Android�̉�����g���ɐ؂�ւ���ꍇ�́A
Effect�̒��ŃR�[�h�ŕ`�悷�邱�Ƃ��ł���B
�v���b�g�t�H�[�����\�[�X��xml���g�p���邱�Ƃ��\�B

## �^�b�`�C�x���g

### �݂邵�̃C�x���g

#### GestureRecognizer Class

https://docs.microsoft.com/ja-jp/dotnet/api/xamarin.forms.gesturerecognizer?view=xamarin-forms

- Xamarin.Forms.ClickGestureRecognizer
- Xamarin.Forms.PanGestureRecognizer
- Xamarin.Forms.PinchGestureRecognizer
- Xamarin.Forms.SwipeGestureRecognizer
- Xamarin.Forms.TapGestureRecognizer

#### Press/Release

5��ނ̃C�x���g�����邪�A
�悭�����Press/Release�������B
Button�ȊO�̃R���g���[����Press/Release���������R�͂��ꂩ�B

(�ǋL)

SKCanvasView��Touch�C�x���g�őS��ޔ���\�B

### Press/Release�����o������@

#### ���ʂ���̃C�x���g�̌Ăяo��

https://docs.microsoft.com/ja-jp/xamarin/xamarin-forms/app-fundamentals/effects/touch-tracking

#### �����R�X�g

��L�T���v��������ƁA
����͒񋟂����ׂ��@�\�ł����Ď����ō��@�\�ł͂Ȃ��A
�Ƃ������Ƃ��킩��B
����Ȃ��̂������Ă���悤�ł�Forms��I�������Ӗ����Ȃ��B

### Press/Release�̕K�v���Ƃ�

���[�͉������C���[�W��ʏ펞�C���[�W�Ƃ͐؂�ւ��邱�Ƃ������B
�悭�l���Ă݂�ƃC�x���g��Click(Tap)�ŗǂ��ăA�j���[�V�������������ł���Ζ��Ȃ��B

### �݂邵�ȊO�̃A�j���[�V����

�݂邵�̃A�j���[�V�������p�ӂ���Ă���B

https://docs.microsoft.com/ja-jp/xamarin/xamarin-forms/user-interface/animation/simple

�ėp�I�ȃv���p�e�B�A�j���[�V����������������@�́A
����Ƃ������ƂɂȂ�炵���B

https://docs.microsoft.com/ja-jp/xamarin/xamarin-forms/user-interface/graphics/skiasharp/basics/animation

���Ȃ��݂�TaskDelay��InvalidateSurface�Ŏ�������B
�J�X�^���R���g���[���̃A�j��������v���p�e�B��
�e���|�����l��p�ӂ��ĕ`�悳����ƃA�j���[�V�����ɂȂ�B

## �t���J�X�^���R���g���[��

skia��canvas��draw����`���ŃR���g���[��������B
SKCanvas���p������cs�����ō��ƃ^�b�`�C�x���g�����퓮�삷��炵���B
contentView����������A
�������̕����t�ɃR�X�g��������Ȃ��悤�ȋC������B

### blur

UWP�ł��Ȃ��݂̃A�N����������������@���l����B
���������surface.Snapshot()�̘b�肪������B
����������͎�����View�����̃X�i�b�v�������Ȃ��B
����Ĕw�i���ڂ����J�[�h�̔w�i�Ƃ��ė��p����悤�ȖړI�ɂ͎g���Ȃ��B
�܂�y�䂩�炷�ׂĂ�canvas�Ɉ�M��������悤�ȍ��ł̂ݗL���Ȏ�@�������B
���i�Ƃ��ăA�N�����p�l�������d�g�݂Ƃ��čl�������́A
�ȉ��̂悤�Ȃ��̂ƂȂ�B

1. �y�[�W�̓y��̔w�i��ݒ肷��
2. ����w�i���R���g���[���ɂ��n��
3. �R���g���[�����Ŕw�i�摜���N���b�v���Ď����̓y��ɂ���
4. ���̏�ɖړI�̕`����s��

����ł�����Ȃ�Source���o�C���h���邾���Ŏg����B
�Ǝv������Ƃ�ł��Ȃ��B

- �p�l����OnPaint�ŏ��߂�Rect�����܂�
- �p�l���������̓y���Rect��m�邱�Ƃ͂Ȃ�

�܂�y��̔w�i�Ɋւ���Rect����ɓ���Ȃ��B
�؂�o���Ȃ��B

- �p�l���̂ڂ������G���͓y�䑤�������Ă���
  - �y�䂪�p�l����Rect�𔻒肵�ĊG��؂�o��
  - �y�䂩��p�l���ɊG��n��
- �p�l�����n���ꂽ�G�����H���Ĕw�i�ݒ�

��������Ȃ��Ɠ����Ȃ��B
���̍\���ł͓y��ƃp�l���͈�̂̃R���g���[���Ƃ������ƂɂȂ�B
��͂��M���������ƂȂ�B

����ɁA
�p�l���̏������ԂŃA���[�g���o���Ƃ���
ios�݂����ɃA���[�g�ȊO�ڂ��������ꍇ�͂ǂ��Ȃ邩�B

- �w�i���ڂ���
- �p�l�����X�i�b�v���Ăڂ���

��������Ȃ��Ƃ����Ȃ��B
�������p�l�����X�i�b�v�������_�Ŕw�i���ڂ��āA
�݂����ȃ_�u���{�P�͂ǂ��Ȃ�̂��v�悵�Ȃ��Ƃ����Ȃ��B

�����܂łŃR�X�g���g���ʂ������̂ő����͂܂�����B

