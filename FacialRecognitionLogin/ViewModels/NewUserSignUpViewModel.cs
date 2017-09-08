using System;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

namespace FacialRecognitionLogin
{
    public class NewUserSignUpViewModel : BaseViewModel
    {
        #region Fields
        string _usernameEntryText, _passwordEntryText;
        ICommand _takePhotoButtonCommand;
        #endregion

        #region Events
        public event EventHandler<string> TakePhotoFailed;
        #endregion

        #region Properties
        public ICommand TakePhotoButtonCommand => _takePhotoButtonCommand ??
            (_takePhotoButtonCommand = new Command(async () => await ExecuteTakePhotoButtonCommand()));

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
        async Task ExecuteTakePhotoButtonCommand()
        {
            if (string.IsNullOrWhiteSpace(UsernameEntryText))
            {
                OnTakePhotoFailed("Username Empty");
                return;
            }

            var photoStream = await PhotoHelpers.GetPhotoStreamFromCamera();
            if (photoStream == null)
                return;

            try
            {
                await FacialRecognitionHelpers.AddNewFace(UsernameEntryText, photoStream);
            }
            catch(Exception e)
            {
                OnTakePhotoFailed(e.Message);
            }
        }

        void OnTakePhotoFailed(string errorMessage) =>
            TakePhotoFailed?.Invoke(this, errorMessage);
        #endregion
    }
}
