using System;
using Xamarin.Forms;

namespace FacialRecognitionLogin
{
    public static class FontConstants
    {
        public static string DefaultFontFamily => GetDefaultFontFamily();

        static string GetDefaultFontFamily() => Device.RuntimePlatform switch
        {
            Device.iOS => "AppleSDGothicNeo-Light",
            Device.Android => "Droid Sans Mono",
            _ => throw new NotSupportedException("Platform Not Supported"),
        };
    }
}
