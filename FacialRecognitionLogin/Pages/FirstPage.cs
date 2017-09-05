using System;

using Xamarin.Forms;

using EntryCustomReturn.Forms.Plugin.Abstractions;

namespace FacialRecognitionLogin
{
    public class FirstPage : ContentPage
    {
        #region Constant Fields
        #endregion

        #region Constructors
        public FirstPage()
        {
            BackgroundColor = Color.FromHex("#2980b9");

            Padding = GetPagePadding();
            Title = "First Page";

            Content = new Label
            {
                Text = "Success!",
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                TextColor = Color.White
            };
        }
        #endregion

        #region Methods
        Thickness GetPagePadding()
        {
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    return new Thickness(10, 20, 10, 5);
                case Device.Android:
                    return new Thickness(10, 0, 10, 5);
                default:
                    throw new Exception("Platform Not Supported");
            }
        }
        #endregion
    }
}


