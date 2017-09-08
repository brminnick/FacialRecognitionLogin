using System;
using System.Threading.Tasks;

using Xamarin.Forms;

using EntryCustomReturn.Forms.Plugin.Abstractions;

namespace FacialRecognitionLogin
{
    public class LoginPage : BaseContentPage<LoginViewModel>
    {
        #region Constant Fields
        const double _relativeLayoutPadding = 10;
        readonly RelativeLayout _relativeLayout;
        readonly Image _logo;
        readonly StyledButton _loginButton, _newUserSignUpButton;
        readonly StyledEntry _usernameEntry, _passwordEntry;
        readonly Label _logoSlogan;
        #endregion

        #region Fields
        string _logoFileImageSource;
        bool isInitialized = false;
        #endregion

        #region Contrucotrs
        public LoginPage()
        {
            BackgroundColor = Color.FromHex("#3498db");
            Padding = GetPagePadding();

            _logo = new Image { Source = "xamarin_logo" };

            _logoSlogan = new StyledLabel
            {
                Opacity = 0,
                Text = "Delighting Developers."
            };
            _usernameEntry = new StyledEntry { Placeholder = "Username" };
            _usernameEntry.SetBinding(Entry.TextProperty, nameof(ViewModel.UsernameEntryText));
            CustomReturnEffect.SetReturnType(_usernameEntry, ReturnType.Next);
            CustomReturnEffect.SetReturnCommand(_usernameEntry, new Command(() => _passwordEntry.Focus()));

            _passwordEntry = new StyledEntry
            {
                Placeholder = "Password",
                IsPassword = true,
            };
            _passwordEntry.SetBinding(Entry.TextProperty, nameof(ViewModel.PasswordEntryText));
            CustomReturnEffect.SetReturnType(_passwordEntry, ReturnType.Done);
            CustomReturnEffect.SetReturnCommand(_passwordEntry, new Command(Unfocus));

            _loginButton = new StyledButton(Borders.Thin) { Text = "Login" };
            _loginButton.SetBinding(Button.CommandProperty, nameof(ViewModel.LoginButtonTappedCommand));

            _newUserSignUpButton = new StyledButton(Borders.None) { Text = "Sign-up" };


            Func<RelativeLayout, double> getNewUserButtonWidth = (p) => _newUserSignUpButton.Measure(p.Width, p.Height).Request.Width;
            Func<RelativeLayout, double> getLogoSloganWidth = (p) => _logoSlogan.Measure(p.Width, p.Height).Request.Width;

            _relativeLayout = new RelativeLayout();
            _relativeLayout.Children.Add(
                _logo,
                xConstraint: Constraint.Constant(100),
                yConstraint: Constraint.Constant(250),
                widthConstraint: Constraint.RelativeToParent(p => p.Width - 200)
            );

            _relativeLayout.Children.Add(
                _logoSlogan,
                xConstraint: Constraint.RelativeToParent(p => (p.Width / 2) - (getLogoSloganWidth(p) / 2)),
                yConstraint: Constraint.Constant(125)
            );

            _relativeLayout.Children.Add(
                _usernameEntry,
                xConstraint: Constraint.Constant(40),
                yConstraint: Constraint.RelativeToView(_logoSlogan, (p, v) => v.Y + v.Height + _relativeLayoutPadding),
                widthConstraint: Constraint.RelativeToParent(p => p.Width - 80)
            );
            _relativeLayout.Children.Add(
                _passwordEntry,
                xConstraint: Constraint.Constant(40),
                yConstraint: Constraint.RelativeToView(_usernameEntry, (p, v) => v.Y + v.Height + _relativeLayoutPadding),
                widthConstraint: Constraint.RelativeToParent(p => p.Width - 80)
            );

            _relativeLayout.Children.Add(
                _loginButton,
                xConstraint: Constraint.Constant(40),
                yConstraint: Constraint.RelativeToView(_passwordEntry, (p, v) => v.Y + v.Height + _relativeLayoutPadding),
                widthConstraint: Constraint.RelativeToParent(p => p.Width - 80)
            );
            _relativeLayout.Children.Add(
                _newUserSignUpButton,
                xConstraint: Constraint.RelativeToParent(p => (p.Width / 2) - (getNewUserButtonWidth(p) / 2)),
                yConstraint: Constraint.RelativeToView(_loginButton, (p, v) => v.Y + _loginButton.Height + 15)
            );

            Content = new ScrollView { Content = _relativeLayout };
        }
        #endregion

        #region Methods
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (!isInitialized)
            {
                AnimateLoginPage();
                Navigation.InsertPageBefore(new FirstPage(), this);
            }
        }

        protected override void SubscribeEventHandlers()
        {
			PhotoHelpers.NoCameraDetected += HandleNoCameraDetected;
            _newUserSignUpButton.Clicked += HandleNewUserSignUpButtonClicked;
            ViewModel.LoginFailed += HandleLoginFailed;
        }

        protected override void UnsubscribeEventHandlers()
        {
            PhotoHelpers.NoCameraDetected -= HandleNoCameraDetected;
			_newUserSignUpButton.Clicked -= HandleNewUserSignUpButtonClicked;
            ViewModel.LoginFailed -= HandleLoginFailed;
        }

        void AnimateLoginPage()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(500);
                await _logo?.TranslateTo(0, -_relativeLayout.Height * 0.3 - 10, 250);
                await _logo?.TranslateTo(0, -_relativeLayout.Height * 0.3 + 5, 100);
                await _logo?.TranslateTo(0, -_relativeLayout.Height * 0.3, 50);

                await _logo?.TranslateTo(0, -200 + 5, 100);
                await _logo?.TranslateTo(0, -200, 50);

                await Task.WhenAll(_logoSlogan?.FadeTo(1, 5),
                                    _newUserSignUpButton?.FadeTo(1, 250),
                                   _usernameEntry?.FadeTo(1, 250),
                                   _passwordEntry?.FadeTo(1, 250),
                                   _loginButton?.FadeTo(1, 249));

                isInitialized = true;
            });
        }

        Thickness GetPagePadding()
        {
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    return new Thickness(0, 20, 0, 0);
                case Device.iOS:
                    return new Thickness(0, 0, 0, 0);
                default:
                    throw new Exception("Platform Unsupported");
            }
        }

        void HandleNewUserSignUpButtonClicked(object sender, EventArgs e) =>
           Navigation.PushModalAsync(new NewUserSignUpPage());

        void HandleLoginFailed(object sender, string errorMessage) =>
            Device.BeginInvokeOnMainThread(async () => await DisplayAlert("Error", errorMessage, "Okay"));

        void HandleNoCameraDetected(object sender, EventArgs e) =>
            Device.BeginInvokeOnMainThread(async () => await DisplayAlert("Error", "Camera Unavailable", "OK"));
        #endregion
    }
}