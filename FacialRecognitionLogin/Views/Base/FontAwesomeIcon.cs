using Xamarin.Forms;

namespace FacialRecognitionLogin
{
    public class FontAwesomeIcon : Label
    {
        const string _typeface = "FontAwesome";

        public FontAwesomeIcon() => FontFamily = _typeface;
    }
}
