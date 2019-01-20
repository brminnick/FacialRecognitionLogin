using System;
using System.Windows.Input;
using System.Threading.Tasks;

using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;

namespace FacialRecognitionLogin
{
    public class LoginViewModel : BaseViewModel
    {
        #region Constant Fields
        readonly WeakEventManager _loginApprovedEventManager = new WeakEventManager();
        readonly WeakEventManager<LoginFailedEventArgs> _loginFailedEventManager = new WeakEventManager<LoginFailedEventArgs>();
        #endregion

        #region Fields
        string _usernameEntryText, _passwordEntryText;
        ICommand _loginButtonTappedCommand;
        #endregion

        #region Event
        public event EventHandler LoginApproved
        {
            add => _loginApprovedEventManager.AddEventHandler(value);
            remove => _loginApprovedEventManager.RemoveEventHandler(value);
        }

        public event EventHandler<LoginFailedEventArgs> LoginFailed
        {
            add => _loginFailedEventManager.AddEventHandler(value);
            remove => _loginFailedEventManager.RemoveEventHandler(value);
        }
        #endregion

        #region Properties
        public ICommand LoginButtonTappedCommand => _loginButtonTappedCommand ??
            (_loginButtonTappedCommand = new AsyncCommand(() => ExecuteLoginButtonTappedCommand(UsernameEntryText, PasswordEntryText), continueOnCapturedContext: false));

        public string UsernameEntryText
        {
            get => _usernameEntryText;
            set => SetProperty(ref _usernameEntryText, value);
        }

        public string PasswordEntryText
        {
            get => _passwordEntryText;
            set => SetProperty(ref _passwordEntryText, value);
        }
        #endregion

        #region Methods
        async Task ExecuteLoginButtonTappedCommand(string usernameEntryText, string passwordEntryText)
        {
            if (string.IsNullOrWhiteSpace(usernameEntryText) || string.IsNullOrWhiteSpace(passwordEntryText))
            {
                OnLoginFailed("Username and Password cannot be empty", false);
                return;
            }

            var isUsernamePasswordCorrect = await Xamarin.Forms.DependencyService.Get<ILogin>().CheckLogin(usernameEntryText, passwordEntryText);

            if (!isUsernamePasswordCorrect)
            {
                OnLoginFailed("Username / Password Invalid", true);
                return;
            }

            var photoStream = await PhotoService.GetPhotoStreamFromCamera();

            if (photoStream is null)
            {
                OnLoginFailed("Facial Recognition Required", false);
                return;
            }

            var isFaceRecognized = await FacialRecognitionService.IsFaceIdentified(usernameEntryText, photoStream);

            if (isFaceRecognized)
                OnLoginApproved();
            else
                OnLoginFailed("Face not recognized", true);
        }

        void OnLoginFailed(string errorMessage, bool shouldDisplaySignUpPrompt) =>
            _loginFailedEventManager?.HandleEvent(this, new LoginFailedEventArgs(errorMessage, shouldDisplaySignUpPrompt), nameof(LoginFailed));

        void OnLoginApproved() =>
            _loginApprovedEventManager?.HandleEvent(this, EventArgs.Empty, nameof(LoginApproved));
        #endregion
    }
}
