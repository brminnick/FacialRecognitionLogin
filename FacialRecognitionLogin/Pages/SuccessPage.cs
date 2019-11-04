using System;

using Xamarin.Forms;

namespace FacialRecognitionLogin
{
    public class SuccessPage : ContentPage
    {
        public SuccessPage()
        {
            BackgroundColor = Color.FromHex("#2980b9");

            Padding = GetPagePadding();
            Title = "Success";

            var successLabel = new Label
            {
                TextColor = Color.White,
                Text = "Login Successful"
            };

            var logoutButton = new StyledButton(Borders.Thin, 1) { Text = "Logout" };
            logoutButton.Clicked += HandleLogoutButtonClicked;

            Content = new StackLayout
            {
                Spacing = 35,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Children = {
                    successLabel,
                    logoutButton
                }
            };
        }

        void HandleLogoutButtonClicked(object sender, EventArgs e)
        {
            var loginPage = new LoginPage();

            Application.Current.MainPage = new NavigationPage(loginPage)
            {
                BarBackgroundColor = Color.FromHex("#3498db"),
                BarTextColor = Color.White,
            };

            NavigationPage.SetHasNavigationBar(loginPage, false);
        }

        Thickness GetPagePadding() => Device.RuntimePlatform switch
        {
            Device.iOS => new Thickness(10, 0, 10, 20),
            Device.Android => new Thickness(10, 0, 10, 5),
            _ => throw new Exception("Platform Not Supported"),
        };
    }
}


