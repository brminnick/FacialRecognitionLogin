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

        protected override void OnStart()
        {
            base.OnStart();

            InitializeMainPage();
        }
    }
}
