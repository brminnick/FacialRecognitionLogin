using System;
using System.Windows.Input;
using System.Threading.Tasks;

using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;

namespace FacialRecognitionLogin
{
    public class LoginViewModel : BaseViewModel
    {
        readonly WeakEventManager _loginApprovedEventManager = new WeakEventManager();
        readonly WeakEventManager<LoginFailedEventArgs> _loginFailedEventManager = new WeakEventManager<LoginFailedEventArgs>();

        ICommand? _loginButtonTappedCommand;

        string _usernameEntryText = string.Empty,
            _passwordEntryText = string.Empty;

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

        public ICommand LoginButtonTappedCommand => _loginButtonTappedCommand ??= new AsyncCommand(() => ExecuteLoginButtonTappedCommand(UsernameEntryText, PasswordEntryText));

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

        async Task ExecuteLoginButtonTappedCommand(string usernameEntryText, string passwordEntryText)
        {
            if (string.IsNullOrWhiteSpace(usernameEntryText) || string.IsNullOrWhiteSpace(passwordEntryText))
            {
                OnLoginFailed("Username and Password cannot be empty", false);
                return;
            }

            var isLoginValid = await SecureStorageService.IsLoginCorrect(usernameEntryText, passwordEntryText).ConfigureAwait(false);

            if (isLoginValid)
            {
                var mediaFile = await MediaService.GetMediaFileFromCamera().ConfigureAwait(false);

                if (mediaFile is null)
                {
                    OnLoginFailed("Facial Recognition Required", false);
                }
                else
                {
                    var isFaceRecognized = await FacialRecognitionService.IsFaceIdentified(usernameEntryText, mediaFile.GetStream()).ConfigureAwait(false);

                    if (isFaceRecognized)
                        OnLoginApproved();
                    else
                        OnLoginFailed("Face not recognized", true);
                }
            }
            else
            {
                OnLoginFailed("Username / Password Invalid", true);
            }
        }

        void OnLoginFailed(string errorMessage, bool shouldDisplaySignUpPrompt) =>
            _loginFailedEventManager.HandleEvent(this, new LoginFailedEventArgs(errorMessage, shouldDisplaySignUpPrompt), nameof(LoginFailed));

        void OnLoginApproved() =>
            _loginApprovedEventManager.HandleEvent(this, EventArgs.Empty, nameof(LoginApproved));
    }
}
