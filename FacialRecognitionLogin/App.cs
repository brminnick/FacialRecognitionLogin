using Xamarin.Forms;

namespace FacialRecognitionLogin
{
    public class App : Application
    {
        public App() => InitializeMainPage();

        public static void InitializeMainPage()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var loginPage = new LoginPage();

                Current.MainPage = new NavigationPage(loginPage)
                {
                    BarBackgroundColor = Color.FromHex("#3498db"),
                    BarTextColor = Color.White,
                };

                NavigationPage.SetHasNavigationBar(loginPage, false);
            });
        }
    }
}
