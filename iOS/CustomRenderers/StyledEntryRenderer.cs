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
		UITextField nativeTextField;
		bool isInitialized;

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == "IsEnabled")
			{
				if (!nativeTextField.Enabled)
					nativeTextField.TextColor = UIColor.White;
				else
					nativeTextField.TextColor = UIColor.Blue;
			}
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null && !isInitialized)
			{
				var formsEntry = e.NewElement as StyledEntry;
				nativeTextField = Control as UITextField;
				nativeTextField.Font = UIFont.FromName("AppleSDGothicNeo-Light", 18);
				nativeTextField.TextColor = UIColor.White;

				if (!string.IsNullOrEmpty(formsEntry.Placeholder))
					nativeTextField.AttributedPlaceholder = new NSAttributedString(formsEntry.Placeholder, UIFont.FromName("AppleSDGothicNeo-Light", 18), UIColor.White);

				isInitialized = true;
			}
		}
	}
}