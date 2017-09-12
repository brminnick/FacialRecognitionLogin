using System;

using Xamarin.Forms;

using EntryCustomReturn.Forms.Plugin.Abstractions;

namespace FacialRecognitionLogin
{
    public class NewUserSignUpPage : BaseContentPage<NewUserSignUpViewModel>
    {
        #region Constant Fields
        readonly StyledButton _saveUsernameButton, _cancelButton, _takePhotoButton;
        readonly StyledEntry _usernameEntry, _passwordEntry;
        #endregion

        #region Constructos
        public NewUserSignUpPage()
        {
            BackgroundColor = Color.FromHex("2980b9");

            _usernameEntry = new StyledEntry(1)
            {
                Placeholder = "Username",
                HorizontalOptions = LayoutOptions.Fill,
                HorizontalTextAlignment = TextAlignment.End,
                PlaceholderColor = Color.FromHex("749FA8"),
            };
            _usernameEntry.SetBinding(Entry.TextProperty, nameof(ViewModel.UsernameEntryText));
            CustomReturnEffect.SetReturnType(_usernameEntry, ReturnType.Next);
            CustomReturnEffect.SetReturnCommand(_usernameEntry, new Command(() => _passwordEntry.Focus()));

            _passwordEntry = new StyledEntry(1)
            {
                Placeholder = "Password",
                IsPassword = true,
                HorizontalOptions = LayoutOptions.Fill,
                HorizontalTextAlignment = TextAlignment.End,
                VerticalOptions = LayoutOptions.Fill,
                PlaceholderColor = Color.FromHex("749FA8")
            };
            _passwordEntry.SetBinding(Entry.TextProperty, nameof(ViewModel.PasswordEntryText));
            CustomReturnEffect.SetReturnType(_passwordEntry, ReturnType.Done);
            CustomReturnEffect.SetReturnCommand(_passwordEntry, new Command(Unfocus));

            _saveUsernameButton = new StyledButton(Borders.Thin, 1)
            {
                Text = "Save Username",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.EndAndExpand
            };
            _saveUsernameButton.SetBinding(IsEnabledProperty, nameof(ViewModel.IsInternetConnectionInactive));
            _saveUsernameButton.SetBinding(Button.CommandProperty, nameof(ViewModel.SaveButtonCommand));

            _cancelButton = new StyledButton(Borders.Thin, 1)
            {
                Text = "Cancel",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End
            };
            _cancelButton.SetBinding(IsEnabledProperty, nameof(ViewModel.IsInternetConnectionInactive));
            _cancelButton.SetBinding(Button.CommandProperty, nameof(ViewModel.CancelButtonCommand));

            _takePhotoButton = new StyledButton(Borders.Thin, 1)
            {
                Text = "Take Photo",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.EndAndExpand
            };
            _takePhotoButton.SetBinding(IsEnabledProperty, nameof(ViewModel.IsInternetConnectionInactive));
            _takePhotoButton.SetBinding(Button.CommandProperty, nameof(ViewModel.TakePhotoButtonCommand));

            var isFacialRecognitionCompletedDescriptionLabel = new StyledLabel { Text = "Facial Recognition Completed" };

            var isFacialRecognitionCompletedLabel = new FontAwesomeIcon
            {
                TextColor = Color.White,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center
            };
            isFacialRecognitionCompletedLabel.SetBinding(IsVisibleProperty, nameof(ViewModel.IsInternetConnectionInactive));
            isFacialRecognitionCompletedLabel.SetBinding(Label.TextProperty, nameof(ViewModel.FontAwesomeLabelText));

            var activityIndicator = new ActivityIndicator
            {
                Color = Color.White,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.EndAndExpand,
            };
            activityIndicator.SetBinding(IsVisibleProperty, nameof(ViewModel.IsInternetConnectionActive));
            activityIndicator.SetBinding(ActivityIndicator.IsRunningProperty, nameof(ViewModel.IsInternetConnectionActive));

            var stackLayout = new StackLayout
            {
                Padding = new Thickness(20, 50, 20, 20),
                VerticalOptions = LayoutOptions.FillAndExpand,

                Children ={
                    new Label
                    {
                        Text = "Please enter username",
                        TextColor = Color.White,
                        HorizontalOptions = LayoutOptions.Start
                    },

                    _usernameEntry,

                    new Label
                    {
                        Text = "Please enter password",
                        TextColor = Color.White,
                        HorizontalOptions = LayoutOptions.Start
                    },

                    _passwordEntry,
                    _takePhotoButton,
                    new StackLayout{
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.Center,
                        Children={
                            isFacialRecognitionCompletedDescriptionLabel,
                            isFacialRecognitionCompletedLabel,
                            activityIndicator
                        }},
                    _saveUsernameButton,
                    _cancelButton
                }
            };

            Content = new ScrollView { Content = stackLayout };
        }
        #endregion

        #region Methods
        protected override void OnAppearing()
        {
            base.OnAppearing();

            Device.BeginInvokeOnMainThread(() => _usernameEntry.Focus());
        }

        protected override void SubscribeEventHandlers()
        {
            ViewModel.SaveFailed += HandleSaveFailed;
            _cancelButton.Clicked += HandleCancelButtonClicked;
            ViewModel.TakePhotoFailed += HandleTakePhotoFailed;
            PhotoService.NoCameraDetected += HandleNoPhotoDetected;
            ViewModel.SaveSuccessfullyCompleted += HandleSaveSuccessfullyCompleted;
        }

        protected override void UnsubscribeEventHandlers()
        {
            ViewModel.SaveFailed -= HandleSaveFailed;
            _cancelButton.Clicked -= HandleCancelButtonClicked;
            ViewModel.TakePhotoFailed -= HandleTakePhotoFailed;
            PhotoService.NoCameraDetected -= HandleNoPhotoDetected;
            ViewModel.SaveSuccessfullyCompleted -= HandleSaveSuccessfullyCompleted;
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            _cancelButton.WidthRequest = width - 40;
            _saveUsernameButton.WidthRequest = width - 40;
            _takePhotoButton.WidthRequest = width - 40;

            base.LayoutChildren(x, y, width, height);
        }

        void HandleSaveSuccessfullyCompleted(object sender, EventArgs e) =>
            Device.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Success", "New User Created", "OK");
                await Navigation.PopModalAsync();
            });

        void HandleSaveFailed(object sender, string errorMessage) =>
            Device.BeginInvokeOnMainThread(async () => await DisplayAlert("Error", errorMessage, "OK"));

        void HandleNoPhotoDetected(object sender, EventArgs e) =>
            Device.BeginInvokeOnMainThread(async () => await DisplayAlert("Error", "Camera Not Available", "OK"));

        void HandleCancelButtonClicked(object sender, EventArgs e) =>
            Device.BeginInvokeOnMainThread(async () => await Navigation.PopModalAsync());

        void HandleTakePhotoFailed(object sender, string errorMessage) =>
            Device.BeginInvokeOnMainThread(async () => await DisplayAlert("Error", errorMessage, "OK"));
        #endregion
    }
}