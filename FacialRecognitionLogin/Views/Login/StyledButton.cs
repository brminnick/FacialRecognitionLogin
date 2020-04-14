using System;
using Xamarin.Forms;
using Xamarin.Forms.Markup;

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
            FontFamily = FontConstants.DefaultFontFamily;

            this.FillExpand();

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