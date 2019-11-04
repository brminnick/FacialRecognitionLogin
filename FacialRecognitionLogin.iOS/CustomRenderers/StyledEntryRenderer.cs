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
        const int _defaultFontSize = 18;

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

            if (e.NewElement != null && !Control.Font.FamilyName.Equals(FontConstants.DefaultFontFamily))
            {
                var newEntry = e.NewElement;

                Control.Font = UIFont.FromName(FontConstants.DefaultFontFamily, _defaultFontSize);
                Control.TextColor = UIColor.White;

                if (!string.IsNullOrEmpty(newEntry.Placeholder))
                    Control.AttributedPlaceholder = new NSAttributedString(newEntry.Placeholder, UIFont.FromName(FontConstants.DefaultFontFamily, _defaultFontSize), UIColor.White);
            }
        }
    }
}