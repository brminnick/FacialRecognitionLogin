using Xamarin.Forms;

namespace FacialRecognitionLogin
{
	public enum Borders
	{
		None,
		Thin
	}

	public class StyledButton : Button
	{
		public StyledButton(Borders border, double opacity = 0)
		{
			BackgroundColor = Color.Transparent;
			TextColor = Color.White;
			FontSize = 18;
			Opacity = opacity;
			FontFamily = StyleHelpers.GetFontFamily();

			switch (border)
			{
				case Borders.None:
					break;
				case Borders.Thin:
					CornerRadius = 3;
					BorderColor = Color.White;
					BorderWidth = 1;
					break;
			}
		}
	}
}