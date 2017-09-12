using Android.Graphics;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using FacialRecognitionLogin;
using FacialRecognitionLogin.Droid;

[assembly: ExportRenderer(typeof(FontAwesomeIcon), typeof(FontAwesomeIconRenderer))]
namespace FacialRecognitionLogin.Droid
{
    public class FontAwesomeIconRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement == null)
                Control.Typeface = Typeface.CreateFromAsset(Forms.Context.Assets, "FontAwesome.ttf");
        }
    }
}
