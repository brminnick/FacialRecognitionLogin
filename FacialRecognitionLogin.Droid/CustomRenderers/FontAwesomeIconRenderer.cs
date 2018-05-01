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
        #region Constructors
        public FontAwesomeIconRenderer(Context context) : base(context)
        {
        }
        #endregion

        #region Properties
        Context CurrentContext => CrossCurrentActivity.Current.Activity;
        #endregion

        #region Methods
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement is null)
                Control.Typeface = Typeface.CreateFromAsset(CurrentContext.Assets, "FontAwesome.ttf");
        }
        #endregion
    }
}
