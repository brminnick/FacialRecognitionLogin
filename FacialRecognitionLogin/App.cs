using System;
using Xamarin.Forms;

namespace FacialRecognitionLogin
{
    public class App : Application
    {
        //Use a blank ContentPage as a placeholder until OnStart calls InitializeMainPage
        public App() => MainPage = new ContentPage();

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

        public static string GetDefaultFontFamily()
        {
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    return "AppleSDGothicNeo-Light";
                case Device.Android:
                    return "Droid Sans Mono";
                default:
                    throw new NotSupportedException("Platform Not Supported");
            }
        }

        protected override void OnStart()
        {
            base.OnStart();

            InitializeMainPage();
        }
    }
}
