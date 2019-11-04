using Xamarin.Forms;

namespace FacialRecognitionLogin
{
    public class StyledEntry : Xamarin.Forms.Entry
    {
        public StyledEntry(double opacity = 0)
        {
            BackgroundColor = Color.Transparent;
            HeightRequest = 40;
            TextColor = Color.White;
            Opacity = opacity;
            PlaceholderColor = Color.White;
        }
    }
}
