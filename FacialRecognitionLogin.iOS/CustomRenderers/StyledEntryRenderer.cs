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
        #region Fields
        UITextField _nativeTextField;
        bool _isInitialized;
        #endregion

        #region Methods
        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "IsEnabled")
            {
                if (!_nativeTextField.Enabled)
                    _nativeTextField.TextColor = UIColor.White;
                else
                    _nativeTextField.TextColor = UIColor.Blue;
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null && !_isInitialized)
            {
                var formsEntry = e.NewElement as StyledEntry;
                _nativeTextField = Control as UITextField;
                _nativeTextField.Font = UIFont.FromName("AppleSDGothicNeo-Light", 18);
                _nativeTextField.TextColor = UIColor.White;

                if (!string.IsNullOrEmpty(formsEntry.Placeholder))
                    _nativeTextField.AttributedPlaceholder = new NSAttributedString(formsEntry.Placeholder, UIFont.FromName("AppleSDGothicNeo-Light", 18), UIColor.White);

                _isInitialized = true;
            }
        }
        #endregion
    }
}