using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Xamarin.Forms;

using EntryCustomReturn.Forms.Plugin.Abstractions;

namespace FacialRecognitionLogin
{
    public class LoginPage : ContentPage
    {
        #region Constant Fields
        const double _relativeLayoutPadding = 10;
        readonly RelativeLayout _relativeLayout;
		readonly Image _logo;
		readonly StyledButton _loginButton, _newUserSignUpButton;
		readonly StyledEntry _loginEntry, _passwordEntry;
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
            _loginEntry = new StyledEntry { Placeholder = "Username" };
            CustomReturnEffect.SetReturnType(_loginEntry, ReturnType.Next);
            CustomReturnEffect.SetReturnCommand(_loginEntry, new Command(() => _passwordEntry.Focus()));

            _passwordEntry = new StyledEntry
            {
                Placeholder = "Password",
                IsPassword = true,
            };
            CustomReturnEffect.SetReturnType(_passwordEntry, ReturnType.Go);
            CustomReturnEffect.SetReturnCommand(_passwordEntry, new Command(() => HandleLoginButtonClicked(_passwordEntry, EventArgs.Empty)));

            _loginButton = new StyledButton(Borders.Thin) { Text = "Login" };
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
                _loginEntry,
                xConstraint: Constraint.Constant(40),
                yConstraint: Constraint.RelativeToView(_logoSlogan, (p, v) => v.Y + v.Height + _relativeLayoutPadding),
                widthConstraint: Constraint.RelativeToParent(p => p.Width - 80)
            );
            _relativeLayout.Children.Add(
                _passwordEntry,
                xConstraint: Constraint.Constant(40),
                yConstraint: Constraint.RelativeToView(_loginEntry, (p, v) => v.Y + v.Height + _relativeLayoutPadding),
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

            _loginButton.Clicked += HandleLoginButtonClicked;
            _newUserSignUpButton.Clicked += HandleNewUserSignUpButtonClicked;

            if (!isInitialized)
            {
                AnimateLoginPage();
                Navigation.InsertPageBefore(new FirstPage(), this);
            }
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
                                   _loginEntry?.FadeTo(1, 250),
                                   _passwordEntry?.FadeTo(1, 250),
                                   _loginButton?.FadeTo(1, 249));

                isInitialized = true;
            });
        }

        void HandleNewUserSignUpButtonClicked(object sender, EventArgs e) =>
            Navigation.PushModalAsync(new NewUserSignUpPage());


        async void HandleLoginButtonClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_loginEntry.Text) || string.IsNullOrEmpty(_passwordEntry.Text))
                await DisplayAlert("Error", "You must enter a username and password.", "Okay");
            else
                await Login(_loginEntry.Text, _passwordEntry.Text);
        }

        async Task Login(string userName, string passWord)
        {
            var success = await DependencyService.Get<ILogin>().CheckLogin(userName, passWord);

            if (success)
            {
                await Navigation.PopAsync();
            }
            else
            {
                var signUp = await DisplayAlert("Invalid Login", "Sorry, we didn't recoginize the username or password. Feel free to sign up for free if you haven't!", "Sign up", "Try again");

                if (signUp)
                    await Navigation.PushModalAsync(new NewUserSignUpPage());
            }
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
        #endregion
    }
}