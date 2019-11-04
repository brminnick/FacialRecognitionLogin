using System;
using Xamarin.Forms;

namespace FacialRecognitionLogin
{
    public class App : Application
    {
        //Use a blank ContentPage as a placeholder until OnStart calls InitializeMainPage
        public App()
        {
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
