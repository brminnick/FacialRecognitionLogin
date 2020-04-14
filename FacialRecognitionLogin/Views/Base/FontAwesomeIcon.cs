using Xamarin.Forms;

namespace FacialRecognitionLogin
{
    public class FontAwesomeIcon : Label
    {
        public const char CheckedBox = '\uf046', EmptyBox = '\uf096';

        public FontAwesomeIcon() => FontFamily = FontFamilyConstants.FontAwesome;
    }
}
