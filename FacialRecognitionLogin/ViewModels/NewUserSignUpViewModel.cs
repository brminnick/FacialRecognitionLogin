using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;

using Xamarin.Forms;

namespace FacialRecognitionLogin
{
    public class NewUserSignUpViewModel : BaseViewModel
    {
        #region Constant Fields
        readonly WeakEventManager _saveSuccessfullyCompletedEventManager = new WeakEventManager();
        readonly WeakEventManager<string> _takePhotoFailedEventManager = new WeakEventManager<string>();
        readonly WeakEventManager<string> _saveFailedEventManager = new WeakEventManager<string>();
        #endregion

        #region Fields
        Guid _facialRecognitionUserGUID;
        string _usernameEntryText, _passwordEntryText, _fontAwesomeLabelText = FontAwesomeIcon.EmptyBox.ToString();
        ICommand _takePhotoButtonCommand, _saveButtonCommand, _cancelButtonCommand;
        #endregion

        #region Events
        public event EventHandler SaveSuccessfullyCompleted
        {
            add => _saveSuccessfullyCompletedEventManager.AddEventHandler(value);
            remove => _saveSuccessfullyCompletedEventManager.RemoveEventHandler(value);
        }

        public event EventHandler<string> TakePhotoFailed
        {
            add => _takePhotoFailedEventManager.AddEventHandler(value);
            remove => _takePhotoFailedEventManager.RemoveEventHandler(value);
        }

        public event EventHandler<string> SaveFailed
        {
            add => _saveFailedEventManager.AddEventHandler(value);
            remove => _saveFailedEventManager.RemoveEventHandler(value);
        }
        #endregion

        #region Properties
        public ICommand CancelButtonCommand => _cancelButtonCommand ??
            (_cancelButtonCommand = new AsyncCommand(ExecuteCancelButtonCommand, continueOnCapturedContext: false));

        public ICommand TakePhotoButtonCommand => _takePhotoButtonCommand ??
            (_takePhotoButtonCommand = new AsyncCommand(() => ExecuteTakePhotoButtonCommand(UsernameEntryText), continueOnCapturedContext: false));

        public ICommand SaveButtonCommand => _saveButtonCommand ??
            (_saveButtonCommand = new AsyncCommand(() => ExecuteSaveButtonCommand(UsernameEntryText, PasswordEntryText), continueOnCapturedContext: false));

        public string UsernameEntryText
        {
            get => _usernameEntryText;
            set => SetProperty(ref _usernameEntryText, value, () => FontAwesomeLabelText = FontAwesomeIcon.EmptyBox.ToString());
        }

        public string PasswordEntryText
        {
            get => _passwordEntryText;
            set => SetProperty(ref _passwordEntryText, value);
        }

        public string FontAwesomeLabelText
        {
            get => _fontAwesomeLabelText;
            set => SetProperty(ref _fontAwesomeLabelText, value);
        }

        bool IsUsernamePasswordValid => string.IsNullOrWhiteSpace(UsernameEntryText) || string.IsNullOrWhiteSpace(PasswordEntryText);
        #endregion

        #region Methods
        async Task ExecuteTakePhotoButtonCommand(string username)
        {
            if (IsUsernamePasswordValid)
            {
                OnTakePhotoFailed("Username / Password Empty");
                return;
            }

            var photoStream = await PhotoService.GetPhotoStreamFromCamera().ConfigureAwait(false);
            if (photoStream is null)
                return;

            try
            {
                _facialRecognitionUserGUID = await FacialRecognitionService.AddNewFace(username, photoStream).ConfigureAwait(false);
                FontAwesomeLabelText = FontAwesomeIcon.CheckedBox.ToString();
            }
            catch (Exception e)
            {
                OnTakePhotoFailed(e.Message);
                FontAwesomeLabelText = FontAwesomeIcon.EmptyBox.ToString();
            }
        }

        async Task ExecuteSaveButtonCommand(string username, string password)
        {
            if (FontAwesomeLabelText.Equals(FontAwesomeIcon.EmptyBox.ToString()))
            {
                OnSaveFailed("Photo Required for Facial Recognition");
                return;
            }

            var isUserNamePasswordValid = await DependencyService.Get<ILogin>().SetPasswordForUsername(username, password).ConfigureAwait(false);
            if (isUserNamePasswordValid)
                OnSaveSuccessfullyCompleted();
            else
                OnSaveFailed("Username / Password Empty");
        }

        async Task ExecuteCancelButtonCommand()
        {
            await WaitForNewUserSignUpPageToDisappear().ConfigureAwait(false);

            if (!_facialRecognitionUserGUID.Equals(default(Guid)))
                await FacialRecognitionService.RemoveExistingFace(_facialRecognitionUserGUID).ConfigureAwait(false);
        }

        async Task WaitForNewUserSignUpPageToDisappear()
        {
            while (Application.Current.MainPage.Navigation.ModalStack.OfType<NewUserSignUpPage>().Any())
            {
                await Task.Delay(1000).ConfigureAwait(false);
            }
        }

        void OnTakePhotoFailed(string errorMessage) =>
            _takePhotoFailedEventManager?.HandleEvent(this, errorMessage, nameof(TakePhotoFailed));

        void OnSaveFailed(string errorMessage) =>
            _saveFailedEventManager?.HandleEvent(this, errorMessage, nameof(SaveFailed));

        void OnSaveSuccessfullyCompleted() =>
            _saveSuccessfullyCompletedEventManager?.HandleEvent(this, EventArgs.Empty, nameof(SaveSuccessfullyCompleted));
        #endregion
    }
}
