using Xamarin.Forms;
using System;

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
			FontFamily = App.GetDefaultFontFamily();

			switch (border)
			{
				case Borders.None:
					break;

				case Borders.Thin:
					CornerRadius = 3;
					BorderColor = Color.White;
					BorderWidth = 1;
					break;

                default:
                    throw new NotSupportedException($"{nameof(Borders)}.{border.ToString()} Not Supported");
			}
		}
	}
}