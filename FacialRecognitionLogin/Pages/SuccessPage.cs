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

            _logoutButton = new Button
            {
                TextColor = Color.White,
                Text = "Logout",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };

            Content = _logoutButton;
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


