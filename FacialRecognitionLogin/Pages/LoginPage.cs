using System;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace FacialRecognitionLogin
{
    public class LoginPage : BaseMediaContentPage<LoginViewModel>
    {
        const double _relativeLayoutPadding = 10;

        readonly RelativeLayout _relativeLayout;
        readonly Image _logo;
        readonly StyledButton _loginButton, _newUserSignUpButton;
        readonly StyledEntry _usernameEntry, _passwordEntry;
        readonly Label _logoSlogan;

        public LoginPage()
        {
            ViewModel.LoginFailed += HandleLoginFailed;
            ViewModel.LoginApproved += HandleLoginApproved;

            BackgroundColor = Color.FromHex("#3498db");
            Padding = GetPagePadding();

            _logo = new Image { Source = "xamarin_logo" };

            _logoSlogan = new StyledLabel
            {
                Opacity = 0,
                Text = "Delighting Developers.",
            };
            _usernameEntry = new StyledEntry
            {
                Placeholder = "Username",
                ReturnType = ReturnType.Next,
                ReturnCommand = new Command(() => _passwordEntry.Focus())
            };
            _usernameEntry.SetBinding(Xamarin.Forms.Entry.TextProperty, nameof(LoginViewModel.UsernameEntryText));

            _passwordEntry = new StyledEntry
            {
                Placeholder = "Password",
                IsPassword = true,
                ReturnType = ReturnType.Done
            };
            _passwordEntry.SetBinding(Xamarin.Forms.Entry.TextProperty, nameof(LoginViewModel.PasswordEntryText));
            _passwordEntry.SetBinding(Xamarin.Forms.Entry.ReturnCommandProperty, nameof(LoginViewModel.LoginButtonTappedCommand));

            _loginButton = new StyledButton(Borders.Thin) { Text = "Login" };
            _loginButton.SetBinding(IsEnabledProperty, nameof(LoginViewModel.IsInternetConnectionInactive));
            _loginButton.SetBinding(Button.CommandProperty, nameof(LoginViewModel.LoginButtonTappedCommand));

            _newUserSignUpButton = new StyledButton(Borders.None) { Text = "Sign-up" };
            _newUserSignUpButton.Clicked += HandleNewUserSignUpButtonClicked;
            _newUserSignUpButton.SetBinding(IsEnabledProperty, nameof(LoginViewModel.IsInternetConnectionInactive));

            var activityIndicator = new ActivityIndicator { Color = Color.White };
            activityIndicator.SetBinding(IsVisibleProperty, nameof(LoginViewModel.IsInternetConnectionActive));
            activityIndicator.SetBinding(ActivityIndicator.IsRunningProperty, nameof(LoginViewModel.IsInternetConnectionActive));

            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);

            _relativeLayout = new RelativeLayout();
            _relativeLayout.Children.Add(_logo,
                Constraint.Constant(100),
                Constraint.Constant(250),
                Constraint.RelativeToParent(p => p.Width - 200));

            _relativeLayout.Children.Add(_logoSlogan,
                Constraint.RelativeToParent(p => (p.Width / 2) - (getLogoSloganWidth(p) / 2)),
                Constraint.Constant(125));

            _relativeLayout.Children.Add(_usernameEntry,
                Constraint.Constant(40),
                Constraint.RelativeToView(_logoSlogan, (p, v) => v.Y + v.Height + _relativeLayoutPadding),
                Constraint.RelativeToParent(p => p.Width - 80));

            _relativeLayout.Children.Add(_passwordEntry,
                Constraint.Constant(40),
                Constraint.RelativeToView(_usernameEntry, (p, v) => v.Y + v.Height + _relativeLayoutPadding),
                Constraint.RelativeToParent(p => p.Width - 80));

            _relativeLayout.Children.Add(_loginButton,
                Constraint.Constant(40),
                Constraint.RelativeToView(_passwordEntry, (p, v) => v.Y + v.Height + _relativeLayoutPadding),
                Constraint.RelativeToParent(p => p.Width - 80));

            _relativeLayout.Children.Add(_newUserSignUpButton,
                Constraint.RelativeToParent(p => (p.Width / 2) - (getNewUserButtonWidth(p) / 2)),
                Constraint.RelativeToView(_loginButton, (parent, view) => view.Y + _loginButton.Height + 15));

            _relativeLayout.Children.Add(activityIndicator,
                Constraint.RelativeToParent(parent => parent.Width / 2 - getActivityIndicatorWidth(parent) / 2),
                Constraint.RelativeToParent(parent => parent.Height / 2 - getActivityIndicatorHeight(parent) / 2));

            Content = new Xamarin.Forms.ScrollView { Content = _relativeLayout };

            double getNewUserButtonWidth(RelativeLayout parent) => _newUserSignUpButton.Measure(parent.Width, parent.Height).Request.Width;
            double getLogoSloganWidth(RelativeLayout parent) => _logoSlogan.Measure(parent.Width, parent.Height).Request.Width;
            double getActivityIndicatorHeight(RelativeLayout parent) => activityIndicator.Measure(parent.Width, parent.Height).Request.Height;
            double getActivityIndicatorWidth(RelativeLayout parent) => activityIndicator.Measure(parent.Width, parent.Height).Request.Width;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (!Navigation.NavigationStack.OfType<SuccessPage>().Any())
            {
                await AnimateLoginPage();
                Navigation.InsertPageBefore(new SuccessPage(), this);
            }
        }

        Task AnimateLoginPage()
        {
            return Device.InvokeOnMainThreadAsync(async () =>
            {
                await Task.Delay(500);
                await _logo.TranslateTo(0, -_relativeLayout.Height * 0.3 - 10, 250);
                await _logo.TranslateTo(0, -_relativeLayout.Height * 0.3 + 5, 100);
                await _logo.TranslateTo(0, -_relativeLayout.Height * 0.3, 50);

                await _logo.TranslateTo(0, -200 + 5, 100);
                await _logo.TranslateTo(0, -200, 50);

                await Task.WhenAll(_logoSlogan.FadeTo(1, 5),
                                    _newUserSignUpButton.FadeTo(1, 250),
                                   _usernameEntry.FadeTo(1, 250),
                                   _passwordEntry.FadeTo(1, 250),
                                   _loginButton.FadeTo(1, 249));
            });
        }

        Thickness GetPagePadding() => Device.RuntimePlatform switch
        {
            Device.Android => new Thickness(0, 44, 0, 0),
            Device.iOS => new Thickness(0, 0, 0, 0),
            _ => throw new Exception("Platform Unsupported"),
        };

        void HandleLoginFailed(object sender, LoginFailedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (e.ShouldDisplaySignUpPrompt)
                {
                    if (await DisplayAlert("Error", e.ErrorMessage, "Sign Up", "Cancel"))
                        await OpenNewUserSignUpPage();
                }
                else
                {
                    await DisplayAlert("Error", e.ErrorMessage, "OK");
                }
            });
        }

        Task OpenNewUserSignUpPage() => Device.InvokeOnMainThreadAsync(() => Navigation.PushModalAsync(new NewUserSignUpPage()));

        async void HandleNewUserSignUpButtonClicked(object sender, EventArgs e) => await OpenNewUserSignUpPage();

        void HandleLoginApproved(object sender, EventArgs e) =>
            Device.BeginInvokeOnMainThread(async () => await Navigation.PopAsync());

        void HandleNoCameraDetected(object sender, EventArgs e) =>
            Device.BeginInvokeOnMainThread(async () => await DisplayAlert("Error", "Camera Unavailable", "OK"));
    }
}