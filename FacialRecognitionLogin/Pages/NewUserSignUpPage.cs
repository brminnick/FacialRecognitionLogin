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
        readonly StackLayout _layout;
        #endregion

        #region Constructos
        public NewUserSignUpPage()
        {
            BackgroundColor = Color.FromHex("#2980b9");

            _layout = new StackLayout
            {
                Padding = new Thickness(20, 50, 20, 20),
                VerticalOptions = LayoutOptions.FillAndExpand,
            };

            _usernameEntry = new UnderlinedEntry(1)
            {
                Placeholder = "Username",
                HorizontalOptions = LayoutOptions.Fill,
                HorizontalTextAlignment = TextAlignment.End,
                PlaceholderColor = Color.FromHex("749FA8"),
            };
            _usernameEntry.SetBinding(Entry.TextProperty, nameof(ViewModel.UsernameEntryText));
            CustomReturnEffect.SetReturnType(_usernameEntry, ReturnType.Next);
            CustomReturnEffect.SetReturnCommand(_usernameEntry, new Command(() => _passwordEntry.Focus()));

            _passwordEntry = new UnderlinedEntry(1)
            {
                Placeholder = "Password",
                IsPassword = true,
                HorizontalOptions = LayoutOptions.Fill,
                HorizontalTextAlignment = TextAlignment.End,
                VerticalOptions = LayoutOptions.Fill,
                PlaceholderColor = Color.FromHex("749FA8")
            };
            _passwordEntry.SetBinding(Entry.TextProperty, nameof(ViewModel.PasswordEntryText));
            CustomReturnEffect.SetReturnType(_passwordEntry, ReturnType.Go);
            CustomReturnEffect.SetReturnCommand(_passwordEntry, new Command(() => HandleSaveUsernameButtonClicked(_saveUsernameButton, EventArgs.Empty)));

            _saveUsernameButton = new BorderedButton(Borders.Thin, 1)
            {
                Text = "Save Username",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.EndAndExpand
            };

            _cancelButton = new BorderedButton(Borders.Thin, 1)
            {
                Text = "Cancel",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End
            };

            _takePhotoButton = new StyledButton(Borders.Thin, 1)
            {
                Text = "Take Photo",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.EndAndExpand
            };

            _layout.Children.Add(
                new Label
                {
                    Text = "Please enter username",
                    TextColor = Color.White,
                    HorizontalOptions = LayoutOptions.Start
                }
            );
            _layout.Children.Add(_usernameEntry);
            _layout.Children.Add(
                new Label
                {
                    Text = "Please enter password",
                    TextColor = Color.White,
                    HorizontalOptions = LayoutOptions.Start
                }
            );
            _layout.Children.Add(_passwordEntry);
            _layout.Children.Add(_takePhotoButton);
            _layout.Children.Add(_saveUsernameButton);
            _layout.Children.Add(_cancelButton);

            Content = new ScrollView { Content = _layout };
        }
        #endregion

        #region Methods
        protected override void SubscribeEventHandlers()
        {
            _cancelButton.Clicked += HandleCancelButtonClicked;
			ViewModel.TakePhotoFailed += HandleTakePhotoFailed;
			PhotoHelpers.NoCameraDetected += HandleNoPhotoDetected;
            _saveUsernameButton.Clicked += HandleSaveUsernameButtonClicked;
        }

        protected override void UnsubscribeEventHandlers()
        {
            _cancelButton.Clicked -= HandleCancelButtonClicked;
            ViewModel.TakePhotoFailed -= HandleTakePhotoFailed;
            PhotoHelpers.NoCameraDetected -= HandleNoPhotoDetected;
			_saveUsernameButton.Clicked -= HandleSaveUsernameButtonClicked;
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            _cancelButton.WidthRequest = width - 40;
            _saveUsernameButton.WidthRequest = width - 40;

            base.LayoutChildren(x, y, width, height);
        }

        async void HandleSaveUsernameButtonClicked(object sender, EventArgs e)
        {
            var success = await DependencyService.Get<ILogin>().SetPasswordForUsername(_usernameEntry.Text, _passwordEntry.Text);

            if (success)
                await Navigation.PopModalAsync();
            else
                await DisplayAlert("Error", "You must enter a username and a password", "Okay");
        }

        void HandleNoPhotoDetected(object sender, EventArgs e) =>
            Device.BeginInvokeOnMainThread(async () => await DisplayAlert("Error", "Camera Not Available", "OK"));

        void HandleCancelButtonClicked(object sender, EventArgs e) =>
            Device.BeginInvokeOnMainThread(async () => await Navigation.PopModalAsync());

        void HandleTakePhotoFailed(object sender, string errorMessage) =>
            Device.BeginInvokeOnMainThread(async () => await DisplayAlert("Error", errorMessage, "OK"));
        #endregion

        #region Classes
        class BorderedButton : StyledButton
        {
            public BorderedButton(Borders border, double opacity) : base(border, opacity)
            {
                BorderRadius = 3;
                TextColor = Color.White;
                BorderColor = Color.White;
                BorderWidth = 1;
            }
        }

        class UnderlinedEntry : StyledEntry
        {
            public UnderlinedEntry(double opacity) : base(opacity)
            {
                BackgroundColor = Color.Transparent;
                HeightRequest = 40;
                TextColor = Color.White;
                PlaceholderColor = Color.White;
            }
        }
        #endregion
    }
}