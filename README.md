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

## ContentPage�R���X�g���N�^

�f�t�H���g�R���X�g���N�^�����œ����悤�ɂ��Ȃ���΂Ȃ�Ȃ��B
�f�U�C�i�ɓ{���邱�ƂŋC�Â��B
�O�q�̃��C�t�T�C�N������݂ŁA
�����Ń����o�ϐ���ۑ�����悤�Ȏd�g�݂�
���݂��邩������Ȃ��B
�����͊�{�s�����œn���B
�����������g�p��������΁A
�R���X�g���N�^��Ƀv���p�e�B���Z�b�g���Ă���PushAsync����B

## Effect

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
