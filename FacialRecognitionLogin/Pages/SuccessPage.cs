using System;

using Xamarin.Forms;

namespace FacialRecognitionLogin
{
    public class SuccessPage : ContentPage
    {
        #region Constant Fields
        readonly Button _logoutButton;
        #endregion

        #region Constructors
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

            _logoutButton = new Button
            {
                BorderColor = Color.White,
                BorderWidth = 1,
                TextColor = Color.White,
                Text = "Logout"
            };

            Content = new StackLayout
            {
                Spacing = 35,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Children = {
                    successLabel,
                    _logoutButton
                }
            };
        }
        #endregion

        #region Methods
        protected override void OnAppearing()
        {
            base.OnAppearing();

            _logoutButton.Clicked += HandleLogoutButtonClicked;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _logoutButton.Clicked -= HandleLogoutButtonClicked;
        }

        void HandleLogoutButtonClicked(object sender, EventArgs e) => App.InitializeMainPage();

        Thickness GetPagePadding()
        {
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    return new Thickness(10, 0, 10, 20);
                case Device.Android:
                    return new Thickness(10, 0, 10, 5);
                default:
                    throw new Exception("Platform Not Supported");
            }
        }
        #endregion
    }
}


