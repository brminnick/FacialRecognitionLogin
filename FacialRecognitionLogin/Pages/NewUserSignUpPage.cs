using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace FacialRecognitionLogin
{
    public class NewUserSignUpPage : BaseMediaContentPage<NewUserSignUpViewModel>
    {
        readonly StyledButton _saveUsernameButton, _cancelButton, _takePhotoButton;

        public NewUserSignUpPage()
        {
            ViewModel.SaveFailed += HandleSaveFailed;
            ViewModel.TakePhotoFailed += HandleTakePhotoFailed;
            ViewModel.SaveSuccessfullyCompleted += HandleSaveSuccessfullyCompleted;

            BackgroundColor = Color.FromHex("2980b9");

            var passwordEntry = new StyledEntry(1)
            {
                Placeholder = "Password",
                IsPassword = true,
                HorizontalOptions = LayoutOptions.Fill,
                HorizontalTextAlignment = TextAlignment.End,
                VerticalOptions = LayoutOptions.Fill,
                PlaceholderColor = Color.FromHex("749FA8"),
                ReturnType = ReturnType.Done
            };
            passwordEntry.ReturnCommand = new Command(() => passwordEntry.Unfocus());
            passwordEntry.SetBinding(Xamarin.Forms.Entry.TextProperty, nameof(NewUserSignUpViewModel.PasswordEntryText));

            var usernameEntry = new StyledEntry(1)
            {
                Placeholder = "Username",
                HorizontalOptions = LayoutOptions.Fill,
                HorizontalTextAlignment = TextAlignment.End,
                PlaceholderColor = Color.FromHex("749FA8"),
                ReturnType = ReturnType.Next,
                ReturnCommand = new Command(() => passwordEntry.Focus())
            };
            usernameEntry.SetBinding(Xamarin.Forms.Entry.TextProperty, nameof(NewUserSignUpViewModel.UsernameEntryText));

            _saveUsernameButton = new StyledButton(Borders.Thin, 1)
            {
                Text = "Save User",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.EndAndExpand
            };
            _saveUsernameButton.SetBinding(IsEnabledProperty, nameof(NewUserSignUpViewModel.IsInternetConnectionInactive));
            _saveUsernameButton.SetBinding(Button.CommandProperty, nameof(NewUserSignUpViewModel.SaveButtonCommand));

            _cancelButton = new StyledButton(Borders.Thin, 1)
            {
                Text = "Cancel",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End
            };
            _cancelButton.Clicked += HandleCancelButtonClicked;
            _cancelButton.SetBinding(IsEnabledProperty, nameof(NewUserSignUpViewModel.IsInternetConnectionInactive));

            _takePhotoButton = new StyledButton(Borders.Thin, 1)
            {
                Text = "Take Photo",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.EndAndExpand
            };
            _takePhotoButton.SetBinding(IsEnabledProperty, nameof(NewUserSignUpViewModel.IsInternetConnectionInactive));
            _takePhotoButton.SetBinding(Button.CommandProperty, nameof(NewUserSignUpViewModel.TakePhotoButtonCommand));

            var isFacialRecognitionCompletedDescriptionLabel = new StyledLabel
            {
                Text = "Facial Recognition Completed",
                VerticalTextAlignment = TextAlignment.Center
            };

            var isFacialRecognitionCompletedLabel = new FontAwesomeIcon
            {
                TextColor = Color.White,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center
            };
            isFacialRecognitionCompletedLabel.SetBinding(Label.TextProperty, nameof(NewUserSignUpViewModel.FontAwesomeLabelText));

            if (Device.RuntimePlatform is Device.iOS)
                isFacialRecognitionCompletedLabel.SetBinding(IsVisibleProperty, nameof(NewUserSignUpViewModel.IsInternetConnectionInactive));

            var activityIndicator = new ActivityIndicator
            {
                Color = Color.White,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.EndAndExpand,
            };
            activityIndicator.SetBinding(IsVisibleProperty, nameof(NewUserSignUpViewModel.IsInternetConnectionActive));
            activityIndicator.SetBinding(ActivityIndicator.IsRunningProperty, nameof(NewUserSignUpViewModel.IsInternetConnectionActive));

            var facialRecognitionStackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center,

                Children ={
                    isFacialRecognitionCompletedDescriptionLabel,
                    isFacialRecognitionCompletedLabel
                }
            };

            var stackLayout = new StackLayout
            {
                Padding = new Thickness(20, 50, 20, 20),
                VerticalOptions = LayoutOptions.FillAndExpand,

                Children =
                {
                    new Label
                    {
                        Text = "Please enter username",
                        TextColor = Color.White,
                        HorizontalOptions = LayoutOptions.Start
                    },

                    usernameEntry,

                    new Label
                    {
                        Text = "Please enter password",
                        TextColor = Color.White,
                        HorizontalOptions = LayoutOptions.Start
                    },

                    passwordEntry,
                    _takePhotoButton,
                    facialRecognitionStackLayout,
                }
            };

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    facialRecognitionStackLayout.Children.Add(activityIndicator);
                    break;
                case Device.Android:
                    stackLayout.Children.Add(activityIndicator);
                    break;
                default:
                    throw new NotSupportedException("Device Runtime Unsupported");
            }
            stackLayout.Children.Add(_saveUsernameButton);
            stackLayout.Children.Add(_cancelButton);

            Content = new Xamarin.Forms.ScrollView { Content = stackLayout };

            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            ViewModel.DeleteGuidCommand.Execute(null);
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            _cancelButton.WidthRequest = width - 40;
            _saveUsernameButton.WidthRequest = width - 40;
            _takePhotoButton.WidthRequest = width - 40;

            base.LayoutChildren(x, y, width, height);
        }

        void HandleSaveSuccessfullyCompleted(object sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Success", "New User Created", "OK");
                await Navigation.PopModalAsync();
            });
        }

        void HandleSaveFailed(object sender, string errorMessage) =>
            MainThread.BeginInvokeOnMainThread(async () => await DisplayAlert("Error", errorMessage, "OK"));

        void HandleCancelButtonClicked(object sender, EventArgs e) =>
            MainThread.BeginInvokeOnMainThread(async () => await Navigation.PopModalAsync());

        void HandleTakePhotoFailed(object sender, string errorMessage) =>
            MainThread.BeginInvokeOnMainThread(async () => await DisplayAlert("Error", errorMessage, "OK"));
    }
}