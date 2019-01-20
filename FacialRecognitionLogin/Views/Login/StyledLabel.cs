using Xamarin.Forms;

namespace FacialRecognitionLogin
{
	public class StyledLabel : Label
	{
		public StyledLabel()
		{
			TextColor = Color.White;
			FontFamily = App.GetDefaultFontFamily();
			FontSize = 14;
		}
	}
}

