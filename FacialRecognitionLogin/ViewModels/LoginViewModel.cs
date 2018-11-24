using System;
using System.Windows.Input;
using System.Threading.Tasks;

using AsyncAwaitBestPractices.MVVM;

namespace FacialRecognitionLogin
{
    public class LoginViewModel : BaseViewModel
    {
        #region Fields
        string _usernameEntryText, _passwordEntryText;
        ICommand _loginButtonTappedCommand;
        #endregion

        #region Event
        public event EventHandler<LoginFailedEventArgs> LoginFailed;
        public event EventHandler LoginApproved;
        #endregion

        #region Properties
        public ICommand LoginButtonTappedCommand => _loginButtonTappedCommand ??
            (_loginButtonTappedCommand = new AsyncCommand(() => ExecuteLoginButtonTappedCommand(UsernameEntryText, PasswordEntryText), false));

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
            LoginFailed?.Invoke(this, new LoginFailedEventArgs(errorMessage, shouldDisplaySignUpPrompt));

        void OnLoginApproved() =>
            LoginApproved?.Invoke(this, EventArgs.Empty);
        #endregion
    }
}
