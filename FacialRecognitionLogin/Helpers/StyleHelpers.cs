using System;

using Xamarin.Forms;

namespace FacialRecognitionLogin
{
	public static class StyleHelpers
	{
		public static string GetFontFamily()
		{
			switch (Device.RuntimePlatform)
			{
				case Device.iOS:
					return "AppleSDGothicNeo-Light";
				case Device.Android:
					return "Droid Sans Mono";
				default:
					throw new Exception("Platform Not Supported");
			}
		}
	}
}
