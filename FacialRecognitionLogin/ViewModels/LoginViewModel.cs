using System;
using System.IO;
using System.Windows.Input;
using System.Threading.Tasks;

using Plugin.Media;
using Plugin.Media.Abstractions;

using Xamarin.Forms;
namespace FacialRecognitionLogin
{
    public class LoginViewModel : BaseViewModel
    {
        #region Fields
        string _usernameEntryText, _passwordEntryText;
        ICommand _loginButtonTappedCommand;
        #endregion

        #region Event
        public event EventHandler<string> LoginFailed;
        public event EventHandler LoginApproved;
        #endregion

        #region Properties
        public ICommand LoginButtonTappedCommand => _loginButtonTappedCommand ??
            (_loginButtonTappedCommand = new Command(async () => await ExecuteLoginButtonTappedCommand()));

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
        async Task ExecuteLoginButtonTappedCommand()
        {
            if (string.IsNullOrWhiteSpace(UsernameEntryText) || string.IsNullOrWhiteSpace(PasswordEntryText))
            {
                OnLoginFailed("Username and Password cannot be empty");
                return;
            }

            var isUsernamePasswordCorrect = await DependencyService.Get<ILogin>().CheckLogin(UsernameEntryText, PasswordEntryText);

            var photoStream = await PhotoHelpers.GetPhotoStreamFromCamera();

            var isFaceRecognized = await FacialRecognitionHelpers.IsFaceIdentified(UsernameEntryText, photoStream);

            if (isFaceRecognized)
                OnLoginApproved();
            else
                OnLoginFailed("Face not recognized");
        }

        void OnLoginFailed(string errorMessage) =>
            LoginFailed?.Invoke(this, errorMessage);

        void OnLoginApproved() =>
            LoginApproved?.Invoke(this, EventArgs.Empty);
        #endregion
    }
}
