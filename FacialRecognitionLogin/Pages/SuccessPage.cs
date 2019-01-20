using System;

using Xamarin.Forms;

namespace FacialRecognitionLogin
{
	public class SuccessPage : ContentPage
	{
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
		#endregion

		#region Methods
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


