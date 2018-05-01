using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

using Akavache;

using Xamarin.Forms;

using FacialRecognitionLogin.Droid;

[assembly: Dependency(typeof(Login_Droid))]
namespace FacialRecognitionLogin.Droid
{
    public class Login_Droid : ILogin
    {
        #region Constant Fields
        const string _username = "username";
        const string _password = "password";
        #endregion

        public async Task<bool> SetPasswordForUsername(string username, string password)
        {
            await BlobCache.UserAccount.InsertObject(_username, username);
            await BlobCache.UserAccount.InsertObject(_password, password);

            return true;
        }

        public async Task<bool> CheckLogin(string username, string password)
        {
            string usernameFromSecureStorage = null;
            string passwordFromSecureStorage = null;

            try
            {
                usernameFromSecureStorage = await BlobCache.UserAccount.GetObject<string>(_username);
            }
            catch (Exception)
            {
                return false;
            }

            try
            {
                passwordFromSecureStorage = await BlobCache.UserAccount.GetObject<string>(_password);
            }
            catch (Exception)
            {
                return false;
            }

            if (usernameFromSecureStorage is null || passwordFromSecureStorage is null)
                return false;

            if (password == passwordFromSecureStorage &&
                username == usernameFromSecureStorage.ToString())
            {
                return true;
            }

            return false;
        }
    }
}