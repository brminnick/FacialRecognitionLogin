
using Android.OS;
using Android.App;
using Android.Content.PM;

namespace FacialRecognitionLogin.Droid
{
    [Activity(Label = "FacialRecognitionLogin.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            EntryCustomReturn.Forms.Plugin.Android.CustomReturnEntryRenderer.Init();

            LoadApplication(new App());
        }
    }
}
