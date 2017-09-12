using System;

namespace FacialRecognitionLogin
{
    public class LoginFailedEventArgs : EventArgs
    {
        readonly string _errorMessage;
        readonly bool _shouldDisplaySignUpPrompt;

        public LoginFailedEventArgs(string errorMessage, bool shouldDisplaySignUpPrompt)
        {
            _errorMessage = errorMessage;
            _shouldDisplaySignUpPrompt = shouldDisplaySignUpPrompt;
        }

        public string ErrorMessage => _errorMessage;
        public bool ShouldDisplaySignUpPrompt => _shouldDisplaySignUpPrompt;
    }
}
