using UIKit;
using Foundation;

namespace FacialRecognitionLogin.iOS
{
    [Register(nameof(AppDelegate))]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        UIVisualEffectView _blurWindow;

        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            global::Xamarin.Forms.Forms.Init();

            LoadApplication(new App());

            return base.FinishedLaunching(uiApplication, launchOptions);
        }

        public override void OnActivated(UIApplication uiApplication)
        {
            base.OnActivated(uiApplication);

            RemoveBlurOverlay();
        }

        public override void OnResignActivation(UIApplication uiApplication)
        {
            base.OnResignActivation(uiApplication);

            AddBlurOverlay();
        }

        void AddBlurOverlay()
        {
            using (var blurEffect = UIBlurEffect.FromStyle(UIBlurEffectStyle.Light))
            {
                _blurWindow = new UIVisualEffectView(blurEffect)
                {
                    Frame = UIApplication.SharedApplication.KeyWindow.RootViewController.View.Bounds
                };
            }

            UIApplication.SharedApplication.KeyWindow.RootViewController.View.AddSubview(_blurWindow);
        }

        void RemoveBlurOverlay()
        {
            _blurWindow?.RemoveFromSuperview();
            _blurWindow?.Dispose();
            _blurWindow = null;
        }
    }
}
