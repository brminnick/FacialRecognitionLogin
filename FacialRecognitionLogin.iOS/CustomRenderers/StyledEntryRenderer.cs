using UIKit;
using Foundation;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using FacialRecognitionLogin;

using FacialRecognitionLogin.iOS;

[assembly: ExportRenderer(typeof(StyledEntry), typeof(StyledEntryRenderer))]

namespace FacialRecognitionLogin.iOS
{
    public class StyledEntryRenderer : EntryRenderer
    {
        #region Constant Fields
        const int _defaultFontSize = 18;
        #endregion

        #region Methods
        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName is nameof(View.IsEnabled))
            {
                if (!Control.Enabled)
                    Control.TextColor = UIColor.White;
                else
                    Control.TextColor = UIColor.Blue;
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null && !Control.Font.FamilyName.Equals(App.GetDefaultFontFamily()))
            {
                var formsEntry = e.NewElement as StyledEntry;

                Control.Font = UIFont.FromName(App.GetDefaultFontFamily(), _defaultFontSize);
                Control.TextColor = UIColor.White;

                if (!string.IsNullOrEmpty(formsEntry.Placeholder))
                    Control.AttributedPlaceholder = new NSAttributedString(formsEntry.Placeholder, UIFont.FromName(App.GetDefaultFontFamily(), _defaultFontSize), UIColor.White);
            }
        }
        #endregion
    }
}