using Xamarin.Forms;

namespace FacialRecognitionLogin
{
    public class FontAwesomeIcon : Label
    {
        public const char CheckedBox = '\uf046', EmptyBox = '\uf096';
        const string _typeface = "FontAwesome";

        public FontAwesomeIcon() => FontFamily = _typeface;
    }
}
