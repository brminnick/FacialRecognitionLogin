using Android.Widget;
using Android.Graphics;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using FacialRecognitionLogin;
using FacialRecognitionLogin.Droid;

[assembly: ExportRenderer(typeof(StyledEntry), typeof(StyledEntryRenderer))]
namespace FacialRecognitionLogin.Droid
{
	public class StyledEntryRenderer : EntryRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				var droidEditText = Control as EditText;
				droidEditText.SetHintTextColor(Xamarin.Forms.Color.White.ToAndroid());

				Typeface font = Typeface.Create("Droid Sans Mono", TypefaceStyle.Normal);
				droidEditText.Typeface = font;
			}
		}
	}
}