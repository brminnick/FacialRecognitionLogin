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
        readonly WeakEventManager _saveSuccessfullyCompletedEventManager = new WeakEventManager();
        readonly WeakEventManager<string> _takePhotoFailedEventManager = new WeakEventManager<string>();
        readonly WeakEventManager<string> _saveFailedEventManager = new WeakEventManager<string>();

        Guid _facialRecognitionUserGUID;
        ICommand? _takePhotoButtonCommand, _saveButtonCommand, _cancelButtonCommand, _deleteGuidCommand;

        string _usernameEntryText = string.Empty,
            _passwordEntryText = string.Empty,
            _fontAwesomeLabelText = FontAwesomeIcon.EmptyBox.ToString();

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

        public ICommand CancelButtonCommand => _cancelButtonCommand ??= new AsyncCommand(ExecuteCancelButtonCommand);
        public ICommand TakePhotoButtonCommand => _takePhotoButtonCommand ??= new AsyncCommand(() => ExecuteTakePhotoButtonCommand(UsernameEntryText, PasswordEntryText));
        public ICommand SaveButtonCommand => _saveButtonCommand ??= new AsyncCommand(() => ExecuteSaveButtonCommand(UsernameEntryText, PasswordEntryText));
        public ICommand DeleteGuidCommand => _deleteGuidCommand ??= new AsyncCommand(ExecuteDeleteGuidCommand);

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

        async Task ExecuteTakePhotoButtonCommand(string username, string password)
        {
            if (IsUsernamePasswordValid(username, password))
            {
                OnTakePhotoFailed("Username / Password Empty");
                return;
            }

            var mediaFile = await PhotoService.GetMediaFileFromCamera().ConfigureAwait(false);
            if (mediaFile is null)
                return;

            try
            {
                _facialRecognitionUserGUID = await FacialRecognitionService.AddNewFace(username, mediaFile.GetStream()).ConfigureAwait(false);
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
            }
            else if (IsUsernamePasswordValid(username, password))
            {
                OnSaveFailed("Username / Password Empty");
            }
            else
            {
                try
                {
                    await SecureStorageService.SaveLogin(username, password).ConfigureAwait(false);

                    OnSaveSuccessfullyCompleted();
                }
                catch (Exception e)
                {
                    DebugService.PrintException(e);
                    OnSaveFailed("Save Failed");
                }
            }
        }

        async Task ExecuteCancelButtonCommand()
        {
            await WaitForNewUserSignUpPageToDisappear().ConfigureAwait(false);

        }

        async Task WaitForNewUserSignUpPageToDisappear()
        {
            while (Application.Current.MainPage.Navigation.ModalStack.OfType<NewUserSignUpPage>().Any())
            {
                await Task.Delay(1000).ConfigureAwait(false);
            }
        }

        Task ExecuteDeleteGuidCommand() => _facialRecognitionUserGUID != default
                                ? FacialRecognitionService.RemoveExistingFace(_facialRecognitionUserGUID)
                                : Task.CompletedTask;

        bool IsUsernamePasswordValid(string username, string password) => string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password);

        void OnTakePhotoFailed(string errorMessage) =>
            _takePhotoFailedEventManager?.HandleEvent(this, errorMessage, nameof(TakePhotoFailed));

        void OnSaveFailed(string errorMessage) =>
            _saveFailedEventManager?.HandleEvent(this, errorMessage, nameof(SaveFailed));

        void OnSaveSuccessfullyCompleted() =>
            _saveSuccessfullyCompletedEventManager?.HandleEvent(this, EventArgs.Empty, nameof(SaveSuccessfullyCompleted));
    }
}
