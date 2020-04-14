using Xamarin.Forms;

namespace FacialRecognitionLogin
{
    public class App : Application
    {
        public App()
        {
            Device.SetFlags(new[] { "Markup_Experimental" });

            var loginPage = new LoginPage();

            MainPage = new NavigationPage(loginPage)
            {
                BarBackgroundColor = Color.FromHex("#3498db"),
                BarTextColor = Color.White,
            };

            NavigationPage.SetHasNavigationBar(loginPage, false);
        }
    }
}
