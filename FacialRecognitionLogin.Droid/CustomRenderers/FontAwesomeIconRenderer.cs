using Android.Content;
using Android.Graphics;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using FacialRecognitionLogin;
using FacialRecognitionLogin.Droid;

using Plugin.CurrentActivity;

[assembly: ExportRenderer(typeof(FontAwesomeIcon), typeof(FontAwesomeIconRenderer))]
namespace FacialRecognitionLogin.Droid
{
    public class FontAwesomeIconRenderer : LabelRenderer
    {
        public FontAwesomeIconRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement is null && Control != null)
                Control.Typeface = Typeface.CreateFromAsset(CrossCurrentActivity.Current.AppContext.Assets, "FontAwesome.ttf");
        }
    }
}
