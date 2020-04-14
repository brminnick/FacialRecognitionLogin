using System;
using Xamarin.Essentials;

namespace FacialRecognitionLogin
{
    public abstract class BaseMediaContentPage<T> : BaseContentPage<T> where T : BaseViewModel, new()
    {
        protected BaseMediaContentPage()
        {
            MediaService.PermissionsDenied += HandlePermissionsDenied;
            MediaService.NoCameraDetected += HandleNoPhotoDetected;
        }

        void HandlePermissionsDenied(object sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                var isAlertAccepted = await DisplayAlert("Open Settings?", "Storage and Camera Permission Need To Be Enabled", "Ok", "Cancel");
                if (isAlertAccepted)
                    AppInfo.ShowSettingsUI();
            });
        }

        void HandleNoPhotoDetected(object sender, EventArgs e) =>
            MainThread.BeginInvokeOnMainThread(async () => await DisplayAlert("Error", "Camera Not Available", "OK"));
    }
}

