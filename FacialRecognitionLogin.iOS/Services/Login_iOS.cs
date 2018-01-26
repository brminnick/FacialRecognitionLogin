using System.Threading.Tasks;

using Foundation;

using Xamarin.Forms;

using FacialRecognitionLogin.iOS;

[assembly: Dependency(typeof(Login_iOS))]
namespace FacialRecognitionLogin.iOS
{
    public class Login_iOS : ILogin
    {
        #region Constant Fields
        const string _serviceId = "FacialRecognitionLogin";
        const string _username = "username";
        #endregion

        #region Methods
        public Task<bool> SetPasswordForUsername(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return Task.FromResult(false);

            KeychainHelpers.SetPasswordForUsername(username, password, _serviceId, Security.SecAccessible.Always, true);
            NSUserDefaults.StandardUserDefaults.SetString(username, _username);
            NSUserDefaults.StandardUserDefaults.SetBool(true, "hasLogin");
            NSUserDefaults.StandardUserDefaults.Synchronize();

            return Task.FromResult(true);
        }

        public Task<bool> CheckLogin(string username, string password)
        {
            var usernameFromKeyChain = NSUserDefaults.StandardUserDefaults.ValueForKey(new NSString(_username));
            var passwordFromKeyChain = KeychainHelpers.GetPasswordForUsername(username, _serviceId, true);

            if (usernameFromKeyChain == null || passwordFromKeyChain == null)
                return Task.FromResult(false);
            
            if (password == passwordFromKeyChain.ToString() &&
                username == usernameFromKeyChain.ToString())
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
        #endregion
    }
}

