using System;

namespace FacialRecognitionLogin
{
    public class LoginFailedEventArgs : EventArgs
    {
        public LoginFailedEventArgs(string errorMessage, bool shouldDisplaySignUpPrompt) =>
            (ErrorMessage, ShouldDisplaySignUpPrompt) = (errorMessage, shouldDisplaySignUpPrompt);

        public string ErrorMessage { get; }
        public bool ShouldDisplaySignUpPrompt { get; }
    }
}
