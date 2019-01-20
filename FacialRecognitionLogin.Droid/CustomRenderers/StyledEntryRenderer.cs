using Android.Content;
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
        public StyledEntryRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                Control.SetHintTextColor(Xamarin.Forms.Color.White.ToAndroid());
                Control.Typeface = Typeface.Create(App.GetDefaultFontFamily(), TypefaceStyle.Normal);
            }
        }
    }
}